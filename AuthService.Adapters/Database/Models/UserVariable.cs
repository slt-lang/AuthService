namespace AuthService.Adapters.Database.Models
{
    public class UserVariable : AbstractVariable
    {
        public int UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
