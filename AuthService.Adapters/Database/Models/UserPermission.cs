using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Adapters.Database.Models
{
    public class UserPermanentPermission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Permission PermissionId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Archived { get; set; }
        public bool Temporal { get; set; }
        public bool Inheritable { get; set; }
        public bool Prohibition { get; set; }
    }
}
