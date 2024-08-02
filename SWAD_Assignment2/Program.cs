// See https://aka.ms/new-console-template for more information

using SWAD_Assignment2;
using System.ComponentModel;
using System.Formats.Asn1;
using System.Globalization;

//class Program
//{
//    static List<User> LoadUsersFromCSV(string filePath)
//    {
//        var users = new List<User>();

//        foreach (var line in File.ReadLines(filePath))
//        {
//            var values = line.Split(',');
//            int id = int.Parse(values[0].Trim());
//            string fullName = values[1].Trim();
//            int contactNum = int.Parse(values[2].Trim().Replace("-", ""));
//            string email = values[3].Trim();
//            int password = int.Parse(values[4].Trim());
//            DateTime dateOfBirth = DateTime.ParseExact(values[5].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
//            string role = values[6].Trim();

//            switch (role)
//            {
//                case "Admin":
//                    users.Add(new iCar_Admin(id, fullName, contactNum, email, password, dateOfBirth));
//                    break;
//                case "Car Owner":
//                    users.Add(new Car_Owner(id, fullName, contactNum, email, password, dateOfBirth, license));
//                    break;
//                case "Renter":
//                    users.Add(new Renter(id, fullName, contactNum, email, password, dateOfBirth));
//                    break;
//            }
//        }

//        return users;
//    }

//    static void Main(string[] args)
//    {
//        string csvFilePath = "users.csv"; // Path to your CSV file
//        var users = LoadUsersFromCSV(csvFilePath);

//        // Login process
//        Console.WriteLine("Enter email:");
//        string email = Console.ReadLine();

//        Console.WriteLine("Enter password:");
//        int password = int.Parse(Console.ReadLine());

//        User user = users.FirstOrDefault(u => u.Authenticate(email, password));

//        if (user != null)
//        {
//            Console.WriteLine($"Welcome, {user.FullName}");
//            Console.WriteLine($"Role: {user.GetRole()}");
//            Console.WriteLine($"Date of Birth: {user.DateOfBirth.ToShortDateString()}");
//        }
//        else
//        {
//            Console.WriteLine("Invalid email or password.");
//        }
//    }
//}