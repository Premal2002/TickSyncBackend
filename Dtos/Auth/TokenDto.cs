﻿using System.ComponentModel.DataAnnotations;

namespace TickSyncAPI.Dtos.Auth
{
    public class TokenDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string JwtToken { get; set; } = null!;
    }
}
