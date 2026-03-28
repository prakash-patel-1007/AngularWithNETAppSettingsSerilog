using AngularWithNET.Features.Auth.Services;
using Xunit;

namespace AngularWithNET.Tests
{
    public class PasswordServiceTests
    {
        private readonly PasswordService _sut = new PasswordService();

        [Fact]
        public void HashPassword_ReturnsNonEmptyHash()
        {
            var hash = _sut.HashPassword("test123");
            Assert.False(string.IsNullOrEmpty(hash));
        }

        [Fact]
        public void HashPassword_DifferentCallsProduceDifferentHashes()
        {
            var hash1 = _sut.HashPassword("same");
            var hash2 = _sut.HashPassword("same");
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            var hash = _sut.HashPassword("secret");
            Assert.True(_sut.VerifyPassword(hash, "secret"));
        }

        [Fact]
        public void VerifyPassword_WrongPassword_ReturnsFalse()
        {
            var hash = _sut.HashPassword("secret");
            Assert.False(_sut.VerifyPassword(hash, "wrong"));
        }

        [Fact]
        public void VerifyPassword_EmptyPassword_ReturnsFalse()
        {
            var hash = _sut.HashPassword("secret");
            Assert.False(_sut.VerifyPassword(hash, ""));
        }
    }
}
