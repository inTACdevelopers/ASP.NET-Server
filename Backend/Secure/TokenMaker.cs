using Server.Models;
using System.Security.Cryptography;
using System.Text;
namespace Server.Backend.Secure
{
    public class TokenMaker
    {
        private static Task<string> MakeSha256Hash(string string_for_hash)
        {
            return  Task.Run(() => 
            {
                var crypt = new SHA256Managed();
                string hash = String.Empty;
                byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(string_for_hash));
                foreach (byte theByte in crypto)
                {
                    hash += theByte.ToString("x2");
                }
                return hash;
            });
        }
        
        public static string GetUserToken(string login,string password)
        {
            return MakeSha256Hash(login + password).Result;
        }

        public static string GetPostToken(int post_id, string post_title)
        {
            return MakeSha256Hash(post_id.ToString() + post_title).Result;
        }

        public static string GetPostSessionName(int user_id)
        {
             return MakeSha256Hash(user_id.ToString()).Result;
        }
    }
}
