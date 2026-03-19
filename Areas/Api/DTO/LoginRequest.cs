namespace JewelryStore.Areas.Api.DTO
{
    public class LoginRequest
    {
        public string Username { get; set; }   // Email OR Mobile
        public string Password { get; set; }
    }
    public class ForgotPasswordRequest
    {
        public string Email { get; set; }   // Email OR Mobile
        public int Otp { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public class LoginResult
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
