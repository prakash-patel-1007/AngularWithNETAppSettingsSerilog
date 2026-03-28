using System;
using System.ComponentModel.DataAnnotations;

namespace AngularWithNET.Domain
{
    public class RefreshToken
    {
        public int Id { get; set; }

        [Required]
        public string TokenHash { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }
        public int? ReplacedByTokenId { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt.HasValue;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
