namespace API.DTOs
{
    public class NewUserDTO
    {
        public required string UserName { get; set; }

        public required string Password { get; set; }

        public required string ConfirmPassword { get; set; }

        public required string Email { get; set; }
    }
}
