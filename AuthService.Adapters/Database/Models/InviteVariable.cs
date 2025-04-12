namespace AuthService.Adapters.Database.Models
{
    public class InviteVariable : AbstractVariable
    {
        public int InviteId { get; set; }
        public Invite Invite { get; set; } = default!;
    }
}
