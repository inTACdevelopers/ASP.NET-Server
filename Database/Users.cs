﻿using Microsoft.EntityFrameworkCore;
using Server.Backend.Secure;
using Server.Models;
using Server.Services.Registration;
using Npgsql;
using Server.Backend;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Server.Services.Authorization;
using Basetypes;
using Server.Backend.Config;
using Server.Backend.Exeptions;
using System.Diagnostics.Metrics;

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


                    //   await CreateUserPostTable(user_token);
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

                var selected_user = db.Users.Where(user => user.Login == login && user.Password == password).FirstOrDefault();

                if (selected_user == null)
                {
                    _logger.LogWarning("No such User");
                    return Task.FromResult(new UserResponse()
                    {
                        State = "No such User",
                        Code = 401,
                    });
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

        // Пусть будет, но EF позволяет не делать
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

        // return id of new Post
        // Пусть останется как заготовка, не уверен, что entity будет эффективен 
        // на огромном массиве данных в БД
        public static Task<int> UpdateUserPostSequence(string user_token)
        {
            return Task.FromResult(0);
        }

        public static async Task CreateUserPostSession(int user_id)
        {
            string session_name = TokenMaker.GetPostSessionName(user_id);


            string create_session_table_command = $"CREATE TABLE post_session_{session_name} " +
            "AS SELECT * FROM posts ORDER BY weight DESC LIMIT 500 --";

            string alter = $"ALTER TABLE post_session_{session_name} ADD COLUMN position serial --";

            try
            {
                await using (var conn = new NpgsqlConnection(new ConfigManager().GetConnetion()))
                {
                    await conn.OpenAsync();

                    await using (var command = new NpgsqlCommand(create_session_table_command, conn))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                    await using (var command = new NpgsqlCommand(alter, conn))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                throw new DataBaseExeption($"Error while creating {session_name} Post Session Table\n\nFull: {ex.Message}");
            }
        }

        public static async Task DropUserPostSession(int user_id)
        {
            string session_name = TokenMaker.GetPostSessionName(user_id);

            string drop_session_table = $"DROP TABLE post_session_{session_name}--";

            try
            {
                await using (var conn = new NpgsqlConnection(new ConfigManager().GetConnetion()))
                {
                    await conn.OpenAsync();

                    await using (var command = new NpgsqlCommand(drop_session_table, conn))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                }
            }
            catch (Exception ex)
            {

                throw new DataBaseExeption($"Error while droping {session_name} Post Session Table\n\nFull: {ex.Message}");
            }

        }

        public static ICollection<Models.Post> GetAllUserPosts(User user)
        {
            using (IntacNetRuContext db = new IntacNetRuContext())
            {
                return db.Users.Where(u => u.Id == user.Id).First().Posts;
            }
        }
    }
}
