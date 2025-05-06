using System;
using System.Collections.Generic;

namespace TickSyncAPI.Models;

public partial class PasswordResetRequest
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string SecretCode { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }
}
