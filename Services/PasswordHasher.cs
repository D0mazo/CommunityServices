using System.Security.Cryptography;
using System.Text;
using System;

namespace CommunityServices.Services
{
    public static class PasswordHasher
    {
        // Paprasta SHA256 (mokymosi projektui OK). Realiose sistemose naudok BCrypt/Argon2.
        public static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes); // uppercase hex
        }

        public static bool Verify(string input, string hash) =>
            string.Equals(Hash(input), hash, StringComparison.OrdinalIgnoreCase);
    }
}
