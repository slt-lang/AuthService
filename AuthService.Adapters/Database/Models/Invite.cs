namespace AuthService.Adapters.Database.Models
{
    public class Invite
    {
        public int Id { get; set; }
        public string Link { get; set; } = default!;
        public int UserId { get; set; }
        /// <summary>
        /// От кого унаследует базовые права, иначе будет без прав
        /// </summary>
        public int? InheritanceUserId { get; set; }
        public DateTime Ttl { get; set; }

        public virtual User User { get; set; } = default!;
        public virtual User? InheritanceUser { get; set; } = default!;

        public virtual ICollection<InvitePermission> Permissions { get; set; } = default!;
        public virtual ICollection<InviteVariable> Variables { get; set; } = default!;
    }
}
