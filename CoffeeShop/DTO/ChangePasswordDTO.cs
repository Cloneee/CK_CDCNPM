namespace CoffeeShop.DTO
{
    public class ChangePasswordDTO
    {
        public string Username { get; set; }
        public string currentPassword { get; set; }
        public string newPassword { get; set; }

        public string confirmPassword { get; set; }
    }
}
