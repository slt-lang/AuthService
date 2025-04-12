namespace AuthService.Adapters.Database.Models
{
    public class UserPermission : AbstractPermission
    {
        public int UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
