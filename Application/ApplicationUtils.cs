using System;
using System.Linq;

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
    }
}