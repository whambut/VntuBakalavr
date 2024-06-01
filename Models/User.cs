namespace AuthApp1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Паролі мають зберігатися у хешованому вигляді
    }
}