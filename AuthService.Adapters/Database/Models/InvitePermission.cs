namespace AuthService.Adapters.Database.Models
{
    public class InvitePermission : AbstractPermission
    {
        public int InviteId { get; set; }
        public Invite Invite { get; set; } = default!;
    }
}
