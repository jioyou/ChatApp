namespace ChatApp.Models
{
    public class ChatUser
    {
        public string UserId { get; set; }//User.FindFirst(ClaimTypes.NameIdentifier).Value是String类型
        public User User { get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
        public UserRole Role { get; set; }
    }
}
