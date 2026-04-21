using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shared.Helpers
{
    public class UserService
    {
        public void RegisterUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Username and password are required.");
            }

            // Hash the password using UtilityService
            //string hashedPassword = UtilityService.HashPassword(password);

            // Save the user to the database (pseudo code)
            //SaveUserToDatabase(username, hashedPassword);
        }

        private void SaveUserToDatabase(string username, string hashedPassword)
        {
            // Example pseudo-code to save the user
            Console.WriteLine($"User '{username}' saved with hashed password: {hashedPassword}");
        }
    }
}
