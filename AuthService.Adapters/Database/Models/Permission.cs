using System.ComponentModel;

namespace AuthService.Adapters.Database.Models
{
    public enum Permission
    {
        /// <summary>
        /// Право на всё
        /// </summary>
        [Description("Право на всё")]
        RootPermission = 70000,
    }
}
