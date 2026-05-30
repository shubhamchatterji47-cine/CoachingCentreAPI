using System.ComponentModel.DataAnnotations;

namespace CoachingAPI.DTOs.AdmissionDTOs
{
    public class UpdateAdmissionStatusDto
    {
        [Required] public string Status { get; set; } = "";
        public string? AdminNotes { get; set; }
    }
}
