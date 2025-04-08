namespace AuthService.Adapters.Database.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public DateTime RegisterDate { get; set; }
        public bool IsTemplate { get; set; }
        public int InviteCreatorId { get; set; }
    }

    public class Invite
    {
        public int Id { get; set; }
        public Guid Link { get; set; }
        public int UserId { get; set; }
    }
}
