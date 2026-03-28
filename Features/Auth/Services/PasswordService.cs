using AngularWithNET.Domain;
using Microsoft.AspNetCore.Identity;

namespace AngularWithNET.Features.Auth.Services
{
    public class PasswordService
    {
        private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();

        public string HashPassword(string password)
        {
            return _hasher.HashPassword(null, password);
        }

        public bool VerifyPassword(string hash, string provided)
        {
            var result = _hasher.VerifyHashedPassword(null, hash, provided);
            return result == PasswordVerificationResult.Success
                || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
