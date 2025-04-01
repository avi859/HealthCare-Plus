using System;
using System.Security.Cryptography;

namespace AngularAuthApi.Helpers
{
  public class PasswordHasher
  {
    private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();
    private static readonly int SaltSize = 16;
    private static readonly int HashSize = 32; // Using SHA-256, so hash size is 32
    private static readonly int Iterations = 10000;

    public static string HashPassword(string password)
    {
      // Generate a cryptographically secure salt
      byte[] salt = new byte[SaltSize];
      rng.GetBytes(salt);

      // Use SHA-256 as the hash algorithm for better security
      using (var key = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
      {
        byte[] hash = key.GetBytes(HashSize);

        // Combine salt and hash into one byte array
        byte[] hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        // Convert the combined byte array to a Base64 string
        string base64Hash = Convert.ToBase64String(hashBytes);
        return base64Hash;
      }
    }

    // Method to verify the password
    public static bool VerifyPassword(string password, string base64Hash)
    {
      // Convert the Base64 string back to a byte array
      byte[] hashBytes = Convert.FromBase64String(base64Hash);

      // Extract the salt from the hash (first SaltSize bytes)
      byte[] salt = new byte[SaltSize];
      Array.Copy(hashBytes, 0, salt, 0, SaltSize);

      // Use the same hash algorithm and iteration count for verification
      using (var key = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
      {
        byte[] hash = key.GetBytes(HashSize);

        // Compare the computed hash with the one stored in hashBytes (after the salt part)
        for (int i = 0; i < HashSize; i++)
        {
          if (hashBytes[i + SaltSize] != hash[i])
            return false;
        }
      }

      return true;
    }
  }
}
