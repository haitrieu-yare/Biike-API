using System;
using System.Linq;
using System.Text;

namespace Application
{
    public static class ApplicationUtils
    {
        public static int CalculateLastPage(int totalRecord, int limit)
        {
            var remain = totalRecord % limit;
            var lastPage = (totalRecord - remain) / limit;

            if (remain > 0) lastPage++;

            return lastPage;
        }

        public static string GetFullNameAbbreviation(string fullname)
        {
            string[] fullNameArray = fullname.Split(" ");
            fullNameArray = fullNameArray.TakeLast(2).ToArray();
            return string.Join("+", fullNameArray);
        }

        public static string GetRandomColor()
        {
            return Color.ColorList[new Random().Next(Color.ColorList.Count)];
        }
        
        private static readonly Random Random = new();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
        
        public static string HmacSha256Digest(string message, string secret)
        {
            ASCIIEncoding encoding = new();
            byte[] keyBytes = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            System.Security.Cryptography.HMACSHA256 cryptographer = new(keyBytes);

            byte[] bytes = cryptographer.ComputeHash(messageBytes);

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}