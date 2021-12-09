using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Domain.Entities;

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
        
        public static double ToPercentage(double number)
        {
            return Math.Round(number, 4, MidpointRounding.AwayFromZero) * 100;
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

        public static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            // distance between latitudes and longitudes
            double dLat = (Math.PI / 180) * (lat2 - lat1);
            double dLon = (Math.PI / 180) * (lon2 - lon1);

            // convert to radians
            lat1 = (Math.PI / 180) * (lat1);
            lat2 = (Math.PI / 180) * (lat2);

            // apply formulae
            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Pow(Math.Sin(dLon / 2), 2) * Math.Cos(lat1) * Math.Cos(lat2);
            double rad = 6371;
            double c = 2 * Math.Asin(Math.Sqrt(a));
            return rad * c;
        }
        
        public class AddressComparer : IComparer<Address>
        {
            private readonly double _userLatitude;
            private readonly double _userLongitude;

            public AddressComparer(double userLatitude, double userLongitude)
            {
                _userLatitude = userLatitude;
                _userLongitude = userLongitude;
            }

            private readonly CultureInfo _culture = new ("en-US");
            public int Compare(Address? x, Address? y)
            {
                if (ReferenceEquals(x, y)) return 0;
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (ReferenceEquals(null, y)) return 1;
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (ReferenceEquals(null, x)) return -1;
            
                var xCoordinate = x.AddressCoordinate.Split(",");
                double xLatitude = Convert.ToDouble(xCoordinate[0], _culture);
                double xLongitude = Convert.ToDouble(xCoordinate[1],_culture);
                
                var yCoordinate = y.AddressCoordinate.Split(",");
                double yLatitude = Convert.ToDouble(yCoordinate[0], _culture);
                double yLongitude = Convert.ToDouble(yCoordinate[1], _culture);
            
                var distanceFromX = Haversine(_userLatitude, _userLongitude, 
                    xLatitude, xLongitude);
                
                var distanceFromY = Haversine(_userLatitude, _userLongitude, 
                    yLatitude, yLongitude);

                return distanceFromX.CompareTo(distanceFromY);
            }
        }
    }
}