// See https://aka.ms/new-console-template for more information

using SWAD_Assignment2;
using System.ComponentModel;
using System.Formats.Asn1;
using System.Globalization;
static List<User> LoadUsersFromCSV(string usercsvFilePath)
{
    var users = new List<User>();

    foreach (var line in File.ReadLines(usercsvFilePath))
    {
        var values = line.Split(',');
        int id = Convert.ToInt32(values[0].Trim());
        string fullName = values[1].Trim();
        int contactNum = Convert.ToInt32(values[2].Trim());
        string email = values[3].Trim();
        int password = Convert.ToInt32(values[4].Trim());
        DateTime dateOfBirth = DateTime.ParseExact(values[5].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        string role = values[values.Length - 1].Trim();
        int licence = values.Length > 6 && (role == "Car Owner" || role == "Renter") ? int.Parse(values[6].Trim()) : 0;
        string licenseStatus = values.Length > 7 && role == "Renter" ? values[7].Trim() : null;
        int demeritPoints = values.Length > 8 && role == "Renter" ? int.Parse(values[8].Trim()) : 0;

        switch (role)
        {
            case "Admin":
                users.Add(new iCar_Admin(id, fullName, contactNum, email, password, dateOfBirth));
                break;
            case "Car Owner":
                users.Add(new Car_Owner(id, fullName, contactNum, email, password, dateOfBirth, licence));
                break;
            case "Renter":
                users.Add(new Renter(id, fullName, contactNum, email, password, dateOfBirth, licence, licenseStatus, demeritPoints));
                break;
        }
    }

    return users;
}

static List<Car> LoadCarsFromCSV(string carCsvFilePath)
{
    var cars = new List<Car>();

    foreach (var line in File.ReadLines(carCsvFilePath))
    {
        var values = line.Split(',');
        int ownerId = Convert.ToInt32(values[0].Trim());
        string licensePlate = values[1].Trim();
        string carMake = values[2].Trim();
        string model = values[3].Trim();
        int year = Convert.ToInt32(values[4].Trim());
        string mileage = values[5].Trim();
        string availability = values[6].Trim();

        cars.Add(new Car(ownerId, licensePlate, carMake, model, year, mileage, availability));
    }

    return cars;
}

string usercsvFilePath = "Users_Data.csv";
string carCsvFilePath = "Car_List.csv";

var users = LoadUsersFromCSV(usercsvFilePath);
var cars = LoadCarsFromCSV(carCsvFilePath);

// Login process
Console.Write("Welcome! Please login below.");
Console.WriteLine();
Console.WriteLine();

User user = null;
int emailAttempts = 0;
bool emailValid = false;
string email = string.Empty;

while (emailAttempts < 3)
{
    Console.WriteLine("Enter email:");
    email = Console.ReadLine();
    if (users.Any(u => u.Email == email))
    {
        emailValid = true;
        break;
    }
    else
    {
        emailAttempts++;
        if (emailAttempts < 3)
        {
            Console.WriteLine($"Invalid email. You have {3 - emailAttempts} attempts left.");
            Console.WriteLine();
        }
    }
}

if (!emailValid)
{
    Console.WriteLine() ;
    Console.WriteLine("Too many failed attempts. System will exit.");
    return;
}

int passwordAttempts = 0;

while (passwordAttempts < 3)
{
    Console.WriteLine();
    Console.WriteLine("Enter password:");
    int password;
    string passwordInput = Console.ReadLine();

    if (!int.TryParse(passwordInput, out password))
    {
        passwordAttempts++;
        if (passwordAttempts < 3)
        {
            Console.WriteLine($"Invalid input. Please enter a numeric password. You have {3 - passwordAttempts} attempts left.");
        }
    }
    else
    {
        user = users.FirstOrDefault(u => u.Email == email && u.Password == password);

        if (user != null)
        {
            break;
        }
        else
        {
            passwordAttempts++;
            if (passwordAttempts < 3)
            {
                Console.WriteLine($"Invalid password. You have {3 - passwordAttempts} attempts left.");
            }
        }
    }
}


if (user != null)
{
    Console.WriteLine();
    Console.WriteLine($"Welcome, {user.FullName}");
    Console.WriteLine($"Role: {user.GetRole()}");
    Console.WriteLine($"Date of Birth: {user.DateOfBirth.ToShortDateString()}");

    if (user is Car_Owner carOwner)
    {
        Console.WriteLine($"Licence: {carOwner.License}");
        var ownerCars = cars.Where(c => c.CarOwnerId == carOwner.Id).ToList();
        Console.WriteLine();
        Console.WriteLine("Cars Owned:");
        foreach (var car in ownerCars)
        {
            Console.WriteLine($"{"License Plate:",-14} {car.LicensePlate,-9} {"Make:",-5} {car.CarMake,-15} {"Model:",-6} {car.Model,-9} {"Year:",-5} {car.Year,-6} {"Mileage:",-8} {car.Mileage}");
        }
    }
    else if (user is Renter renter)
    {
        Console.WriteLine($"Licence: {renter.LicenseNum}");
        Console.WriteLine($"License Status: {renter.LicenseStatus}");
        Console.WriteLine($"Demerit Points: {renter.DemeritPoints}");

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("1. Book a Car");
            Console.WriteLine("2. View Booking History");
            Console.WriteLine("3. View Payment History");
            Console.WriteLine("4. Exit");
            Console.WriteLine("Choose an option:");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    BookCar(cars);
                    break;
                case "2":
                    ViewBookingHistory(renter);
                    break;
                case "3":
                    ViewPaymentHistory(renter);
                    break;
                case "4":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

        static void BookCar(List<Car> cars)
        {
            Console.WriteLine();
            Console.WriteLine("List of Cars to Rent:");
            foreach (var car in cars)
            {
                Console.WriteLine($"{"License Plate:",-14} {car.LicensePlate,-9} {"Make:",-5} {car.CarMake,-15} {"Model:",-6} {car.Model,-9} {"Year:",-5} {car.Year,-6} {"Mileage:",-8} {car.Mileage,-14} {"Availability:",-13} {car.Availability}");
            }

            Car selectedCar = null;

            while (selectedCar == null)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter the license plate of your selected car: ");
                string licensePlate = Console.ReadLine();

                selectedCar = cars.FirstOrDefault(car => car.LicensePlate.Equals(licensePlate, StringComparison.OrdinalIgnoreCase));

                if (selectedCar == null)
                {
                    Console.WriteLine("Invalid license plate. Please try again.");
                }
            }

            string bookingInput = string.Empty;

            while (!string.Equals(bookingInput, "Make Booking", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Enter 'Make Booking' to start the booking: ");
                bookingInput = Console.ReadLine();

                if (!string.Equals(bookingInput, "Make Booking", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Invalid input. Please enter 'Make Booking' exactly as instructed.");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Car Details:");
            Console.WriteLine($"{"License Plate:",-14} {selectedCar.LicensePlate,-9} {"Make:",-5} {selectedCar.CarMake,-15} {"Model:",-6} {selectedCar.Model,-9} {"Year:",-5} {selectedCar.Year,-6} {"Mileage:",-8} {selectedCar.Mileage,-14} {"Availability:",-13} {selectedCar.Availability}");
        }

        static void ViewBookingHistory(Renter renter)
        {
            Console.WriteLine("Booking History:");
        }

        static void ViewPaymentHistory(Renter renter)
        {
            Console.WriteLine("Payment History:");
        }
    }
}
else
{
    Console.WriteLine("Too many failed attempts. System will exit.");
}
