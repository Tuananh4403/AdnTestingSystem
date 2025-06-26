using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public enum UserRole { Guest, Customer, Staff, Manager, Admin }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public bool IsEmailConfirmed { get; set; } = false;
        public string? EmailVerificationCode { get; set; }
        public DateTime? VerificationCodeExpiresAt { get; set; }
        public UserRole Role { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public UserProfile? Profile { get; set; }
    }

}
