using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeterinaryClinic.API.DTOs.AiDiagnosis
{
    public class AIDiagnosisRequestDto
    {
        public string PetName { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public string Observations { get; set; } = string.Empty;
        public List<string> LabResults { get; set; } = new();
    }
}
