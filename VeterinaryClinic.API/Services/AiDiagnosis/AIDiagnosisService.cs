using System.Net.Http.Headers;
using System.Text.Json;
using VeterinaryClinic.API.DTOs.AiDiagnosis;

namespace VeterinaryClinic.API.Services.AiDiagnosis
{
    public class AIDiagnosisService : IAIDiagnosisService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIDiagnosisService> _logger;

        public AIDiagnosisService(HttpClient httpClient, IConfiguration configuration, ILogger<AIDiagnosisService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AIDiagnosisResponseDto> GenerateAsync(AIDiagnosisRequestDto dto, CancellationToken cancellationToken = default)
        {
            var apiKey = _configuration["OpenAI:ApiKey"];
            var model = _configuration["OpenAI:Model"] ?? "gpt-4o-mini";

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("OpenAI API key is missing.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var systemPrompt =
                "Ești un asistent veterinar AI. Analizează datele medicale și rezultatele de laborator ale animalului și oferă o estimare orientativă. " +
                "Nu oferi un diagnostic definitiv — acesta aparține medicului veterinar. " +
                "Dacă datele sunt insuficiente, menționează că este nevoie de consult suplimentar. " +
                "Returnează EXCLUSIV un obiect JSON valid, fără text suplimentar, fără markdown, fără backtick-uri.";

            var userPrompt = $"""
            Date pacient:
            Nume: {dto.PetName}
            Specie: {dto.Species}
            Rasă: {dto.Breed}
            Vârstă: {dto.Age} ani

            Fișă medicală:
            Diagnostic existent: {(string.IsNullOrWhiteSpace(dto.Diagnosis) ? "Neprecizat" : dto.Diagnosis)}
            Tratament: {(string.IsNullOrWhiteSpace(dto.Treatment) ? "Neprecizat" : dto.Treatment)}
            Observații: {(string.IsNullOrWhiteSpace(dto.Observations) ? "Nicio observație" : dto.Observations)}

            Rezultate laborator:
            {string.Join("\n", dto.LabResults.Select(x => "- " + x))}

            Răspunde DOAR cu un obiect JSON cu câmpurile: possibleCauses (array de string-uri), urgencyLevel (Scăzut/Mediu/Ridicat/Critic), recommendedNextSteps (string), confidence (număr 0-1), disclaimer (string).
            """;

            var request = new
            {
                model,
                messages = new object[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                response_format = new { type = "json_object" },
                temperature = 0.3,
                max_tokens = 1000
            };

            var response = await _httpClient.PostAsJsonAsync(
                "https://api.groq.com/openai/v1/chat/completions",
                request,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("OpenAI request failed: {Error}", error);
                throw new ApplicationException($"OpenAI request failed: {error}");
            }

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            // Chat Completions răspunde în choices[0].message.content
            var outputText = document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrWhiteSpace(outputText))
                throw new ApplicationException("OpenAI returned empty response.");

            var result = JsonSerializer.Deserialize<AIDiagnosisResponseDto>(
                outputText,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? throw new ApplicationException("Could not deserialize AI response.");
        }
    }
}