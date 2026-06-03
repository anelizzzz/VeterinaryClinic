using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
                "Ești un asistent veterinar. Analizează fișa medicală și oferă doar o estimare orientativă, nu un diagnostic definitiv. " +
                "Returnează strict JSON conform schemei cerute. Nu inventa informații lipsă. " +
                "Dacă datele sunt insuficiente, spune că este nevoie de consult suplimentar.";

            var userPrompt = $"""
            Date pacient:
            Nume: {dto.PetName}
            Specie: {dto.Species}
            Rasă: {dto.Breed}
            Vârstă: {dto.Age}

            Fișă medicală:
            Diagnostic: {dto.Diagnosis}
            Tratament: {dto.Treatment}
            Observații: {dto.Observations}

            Rezultate laborator:
            {string.Join("\n", dto.LabResults.Select(x => "- " + x))}
            """;

            var request = new
            {
                model,
                input = new object[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                response_format = new
                {
                    type = "json_schema",
                    json_schema = new
                    {
                        name = "ai_diagnosis_response",
                        strict = true,
                        schema = new
                        {
                            type = "object",
                            properties = new
                            {
                                possibleCauses = new
                                {
                                    type = "array",
                                    items = new { type = "string" }
                                },
                                urgencyLevel = new { type = "string" },
                                recommendedNextSteps = new { type = "string" },
                                confidence = new { type = "number" },
                                disclaimer = new { type = "string" }
                            },
                            required = new[]
                            {
                                "possibleCauses",
                                "urgencyLevel",
                                "recommendedNextSteps",
                                "confidence",
                                "disclaimer"
                            },
                            additionalProperties = false
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/responses", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("OpenAI request failed: {Error}", error);
                throw new ApplicationException($"OpenAI request failed: {error}");
            }

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            var outputText = document.RootElement
                .GetProperty("output")[0]
                .GetProperty("content")[0]
                .GetProperty("text")
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

