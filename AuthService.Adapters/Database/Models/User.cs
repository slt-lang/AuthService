namespace AuthService.Adapters.Database.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string? PasswordHash { get; set; }
        public DateTime RegisterDate { get; set; }
        public bool Enabled { get; set; }
        public bool IsTemplate { get; set; }

        //public virtual User? InvitedBy { get; set; } = default!;
        public virtual ICollection<UserPermission> Permissions { get; set; } = default!;
        public virtual ICollection<UserVariable> Variables { get; set; } = default!;
    }
}
