namespace AuthService.Adapters.Database.Models
{
    public class Invite
    {
        public int Id { get; set; }
        public string Link { get; set; } = default!;
        public int? Uses { get; set; }
        public int? MaxUses { get; set; }
        public int UserId { get; set; }
        public int? InheritanceUserId { get; set; }
        public DateTime Ttl { get; set; }

        public virtual ICollection<InvitePermission> Permissions { get; set; } = default!;
        public virtual ICollection<InviteVariable> Variables { get; set; } = default!;
    }
}
