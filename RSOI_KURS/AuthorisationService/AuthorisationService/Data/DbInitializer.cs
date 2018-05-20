using AuthorisationService.Functions;
using AuthorisationService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorisationService.Data
{
    public class DbInitializer
    {
        public static void Initialize(UserContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            var users = new User[]
            {
                new User{ Login = "Vova", Password = Hasher.GetHashString("sad"), Role = "Admin"},
                new User{ Login = "Georg", Password = Hasher.GetHashString("happy"), Role = "User"}
            };
            foreach (User s in users)
            {
                context.Users.Add(s);
            }
            context.SaveChanges();
        }
    }
}
