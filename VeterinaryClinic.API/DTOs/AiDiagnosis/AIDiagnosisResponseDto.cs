using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeterinaryClinic.API.DTOs.AiDiagnosis
{
    public class AIDiagnosisResponseDto
    {
        public List<string> PossibleCauses { get; set; } = new();
        public string UrgencyLevel { get; set; } = string.Empty;
        public string RecommendedNextSteps { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string Disclaimer { get; set; } = string.Empty;
    }
}
