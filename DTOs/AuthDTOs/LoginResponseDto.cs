namespace CoachingAPI.DTOs.AuthDTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public int? RoleEntityId { get; set; } // TeacherId or StudentId
    }
}
