﻿namespace AuthService.Adapters.Database.Models
{
    public class UserPermission : AbstractPermission
    {
        public User User { get; set; } = default!;
    }
}
