using Microsoft.EntityFrameworkCore;
using Server.Backend.Secure;
using Server.Models;
using Server.Services.Registration;
using Npgsql;
using Server.Backend;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

    }
}
