namespace AuthService.Adapters.Database.Models
{
    public class UserVariable : AbstractVariable
    {
        public User User { get; set; } = default!;
    }
}
