using System.Security.Cryptography;
using System.Text;
namespace Server.Backend.Secure
{
    public class TokenMaker
    {
        public  Task<string> MakeSha256Hash(string string_for_hash)
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
    }
}
