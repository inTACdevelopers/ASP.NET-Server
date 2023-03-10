using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Database
{
    public class Users
    {
        public static async Task<int> RegNewUser(SingUpRequest request)
        {
            using (IntacNetRuContext db = new IntacNetRuContext())
            {
                try
                {
                    var birth_date = request.BirthDate.ToDateTime();
                    var birth_dateonly = new DateOnly(birth_date.Year, birth_date.Month, birth_date.Day);
                    User user = new User()
                    {
                        Name = request.Name,
                        Surname = request.Surname,
                        Login = request.Login,
                        Password = request.Password,
                        Company = request.Company,
                        BirthDate = birth_dateonly,

                    };

                    await db.Users.AddAsync(user);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return 1;
                }
                

                return 0;
            }
        }
    }
}
