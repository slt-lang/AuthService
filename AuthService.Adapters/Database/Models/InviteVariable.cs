namespace AuthService.Adapters.Database.Models
{
    public class InviteVariable : AbstractVariable
    {
        public Invite Invite { get; set; } = default!;
    }
}
