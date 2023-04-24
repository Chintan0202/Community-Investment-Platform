using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIPlatformMain.Entities.Models;

public partial class PasswordReset
{
    public string Email { get; set; } = null!;

    public string Token { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int Id { get; set; }

    [NotMapped]
    public string Password { get; set; }
    [NotMapped]
    public string ConfirmPassword { get; set; }
}
