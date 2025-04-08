namespace AuthService.Adapters.Database.Models
{
    public class InvitePermission : AbstractPermission
    {
        public Invite Invite { get; set; } = default!;
    }
}
