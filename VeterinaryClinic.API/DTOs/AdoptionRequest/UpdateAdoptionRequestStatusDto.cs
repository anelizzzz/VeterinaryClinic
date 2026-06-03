using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.DTOs.AdoptionRequest
{
    public class UpdateAdoptionRequestStatusDto
    {
        public AdoptionStatus Status { get; set; }
    }
}
