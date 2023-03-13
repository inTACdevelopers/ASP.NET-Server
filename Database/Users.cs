using Microsoft.EntityFrameworkCore;
using Server.Backend.Secure;
using Server.Models;
using Server.Services.Registration;
using Npgsql;
using Server.Backend;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Server.Services.Authorization;

namespace Server.Database
{
    public class Users
    {
        public static async Task<int> RegNewUser(SingUpRequest request, string user_token, ILogger<Registration> _logger)
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


                    await CreateUserPostTable(user_token);
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex.Message);
                    return 1;
                }


                return 0;
            }
        }

        public static Task<UserResponse> GetUserByLoginPassword(string login, string password, ILogger<Authorization> _logger)
        {
            using (IntacNetRuContext db = new IntacNetRuContext())
            {

                var selected_user = db.Users.Where(user => user.Login == login && user.Password == password).First();

                if (selected_user == null)
                {
                    _logger.LogWarning("No such User");
                    return Task.FromResult(new UserResponse()
                    {
                        State = "No such User",
                        Code = 401,
                    }) ;
                }

                var birth_date = selected_user.BirthDate.Value;
                var birth_dateonly = new TimeSpan(birth_date.Year, birth_date.Month, birth_date.Day);

                return Task.FromResult(new UserResponse
                {
                    State = "OK",
                    Code = 200,

                    UserType = GetUserType(selected_user),
                    Login = login,
                    Password = password,
                    Company = selected_user.Company,
                    About = selected_user.About == null ? "" : selected_user.About,
                    Name = selected_user.Name,
                    Surname = selected_user.Surname,
                    Birth = new Google.Protobuf.WellKnownTypes.Timestamp()
                    { Seconds = (long)birth_dateonly.TotalSeconds },
                    Id = selected_user.Id

                });
            }
        }

        private static async Task CreateUserPostTable(string user_token)
        {
            await using (var conn = new NpgsqlConnection(new ConfigManager().GetConnetion()))
            {
                string create_table_cmd = $"CREATE TABLE user_posts_{user_token}" +
                $" AS (SELECT * FROM posts) with no data--";

                string create_id_seq_command = $"CREATE SEQUENCE id_seq_user_posts_{user_token} " +
                           "AS BIGINT " +
                           "INCREMENT 1 " +
                           "MINVALUE 1 " +
                           "START 1 " +
                           $"OWNED BY user_posts_{user_token}.id --";


                await conn.OpenAsync();

                await using (var command = new NpgsqlCommand(create_table_cmd, conn))
                {
                    await command.ExecuteNonQueryAsync();
                }

                await using (var command = new NpgsqlCommand(create_id_seq_command, conn))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private static int GetUserType(User user)
        {
            if (user.Company == "")
                return 0;
            return 1;
        }
    }
}
