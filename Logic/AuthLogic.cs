using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using IIS.Dashboard.Models;
using System.Data.SqlTypes;

namespace IIS.Dashboard.Logic
{
    public class AuthLogic
    {
       public  static string HashPassword(string password, byte[] salt)
        {
            using (var sha256 = new SHA256Managed())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

                // Concatenate password and salt
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                // Hash the concatenated password and salt
                byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

                // Concatenate the salt and hashed password for storage
                byte[] hashedPasswordWithSalt = new byte[hashedBytes.Length + salt.Length];
                Buffer.BlockCopy(salt, 0, hashedPasswordWithSalt, 0, salt.Length);
                Buffer.BlockCopy(hashedBytes, 0, hashedPasswordWithSalt, salt.Length, hashedBytes.Length);

                return Convert.ToBase64String(hashedPasswordWithSalt);
            }
        }
        public static byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[16]; // Adjust the size based on your security requirements
                rng.GetBytes(salt);
                return salt;
            }
        }

        public static User Login(string email,string password, AppDbContext context)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new Exception("Email is null or empty");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("Password is null or empty");
            }
            var user = context.Users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            if (user == null)
            {
                throw new Exception($"Can't find user with email: {email}");
            }
            string storedHashedPassword = user.Password;
            byte[] storedSaltBytes = Encoding.UTF8.GetBytes(user.Salt);
            byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPassword = new byte[enteredPasswordBytes.Length + storedSaltBytes.Length];
            Buffer.BlockCopy(enteredPasswordBytes, 0, saltedPassword, 0, enteredPasswordBytes.Length);
            Buffer.BlockCopy(storedSaltBytes, 0, saltedPassword, enteredPasswordBytes.Length, storedSaltBytes.Length);
            string enteredPasswordHash = HashPassword(password, storedSaltBytes);
            if (enteredPasswordHash == storedHashedPassword)
            {
                return user;
            }
            else
            {
                throw new Exception ("Password is incorrect.");
            }
        }
    }
}
