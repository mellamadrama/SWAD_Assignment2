// See https://aka.ms/new-console-template for more information

using SWAD_Assignment2;
using System.ComponentModel;
using System.Formats.Asn1;
using System.Globalization;
static List<User> LoadUsersFromCSV(string usercsvFilePath)
{
    var users = new List<User>();

    foreach (var line in File.ReadLines(usercsvFilePath).Skip(1))
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
        List<Car> cars = new List<Car>(); //temporary placeholder under assumption that all current car owners do not already have a list of cars registered
        List<Booking> bookings = new List<Booking>(); //temporary placeholder under assumption that all current renters do not have existing bookings
        string licenseStatus = values.Length > 7 && role == "Renter" ? values[7].Trim() : null;
        int demeritPoints = values.Length > 8 && role == "Renter" ? int.Parse(values[8].Trim()) : 0;

        switch (role)
        {
            case "Admin":
                users.Add(new iCar_Admin(id, fullName, contactNum, email, password, dateOfBirth));
                break;
            case "Car Owner":
                users.Add(new Car_Owner(id, fullName, contactNum, email, password, dateOfBirth, licence, cars));
                break;
            case "Renter":
                users.Add(new Renter(id, fullName, contactNum, email, password, dateOfBirth, licence, licenseStatus, demeritPoints, bookings));
                break;
        }
    }

    return users;
}

static List<Car> LoadCarsFromCSV(string carCsvFilePath, List<string> dates)
{
    var cars = new List<Car>();

    foreach (var line in File.ReadLines(carCsvFilePath).Skip(1))
    {
        var values = line.Split(',');
        int ownerId = Convert.ToInt32(values[0].Trim());
        string licensePlate = values[1].Trim();
        string carMake = values[2].Trim();
        string model = values[3].Trim();
        int year = Convert.ToInt32(values[4].Trim());
        string mileage = values[5].Trim();
        string availability = values[6].Trim();
        Insurance insurance = new Insurance(); //idk what this is for so this is just here to prevent the error
        List<Booking> bookings = new List<Booking>(); //empty list of bookings 

        var car = new Car(ownerId, licensePlate, carMake, model, year, mileage, availability, insurance, bookings);

        car.AvailableDates = new List<string>(dates);

        if (values.Length > 7)
        {
            car.AvailableDates = values[7].Split(';').ToList();
        }

        if (values.Length > 8)
        {
            car.UnavailableDates = values[8].Split(';').ToList();
        }

        cars.Add(car);
    }

    return cars;
}
static List<string> LoadDateListFromCSV(string datesCsvFilePath)
{
    var dates = new List<string>();

    foreach (var line in File.ReadLines(datesCsvFilePath).Skip(1))
    {

        var values = line.Split(',');
        string date = values[0].Trim();
        string time = values[1].Trim();

        dates.Add($"{date} {time}");
    }

    return dates;
}


// not done
//static List<Insurance> LoadInsuranceFromCSV(string insuranceCsvFilePath)
//{
//    var insurance = new List<Car> ();

//    foreach (var line in File.ReadLines (insuranceCsvFilePath))
//    {
//        var values = line.Split (",");

//    }
//}

string usercsvFilePath = "Users_Data.csv";
string carCsvFilePath = "Car_List.csv";
string icCsvFilePath = "Insurance_Company_List.csv";
string insuranceCsvFilePath = "Insurance_list.csv";
string datesCsvFilePath = "DateTimeSlots.csv";

var users = LoadUsersFromCSV(usercsvFilePath);
var dates = LoadDateListFromCSV(datesCsvFilePath);
var cars = LoadCarsFromCSV(carCsvFilePath, dates);

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

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("1. Register Car");
            Console.WriteLine("2. Exit");
            Console.WriteLine("Choose an Option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RegisterCar(cars);
                    break;
                case "2":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

        static void RegisterCar(List<Car> cars)
        {
            Console.WriteLine();

            // prompt car owner for car details
            Console.WriteLine("Please enter car details");

            // Validate Car Make
            string carMake;
            while (true)
            {
                Console.WriteLine("Enter Car Make: ");
                carMake = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(carMake))
                {
                    break;
                }
                Console.WriteLine("Invalid input. Car Make cannot be empty.");
            }
            // Validate Car Model
            string carModel;
            while (true)
            {
                Console.WriteLine("Enter Car Model: ");
                carModel = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(carModel))
                {
                    break;
                }
                Console.WriteLine("Invalid input. Car Model cannot be empty.");
            }
            // Validate Car Mileage
            int carMileage;
            while (true)
            {
                Console.WriteLine("Enter Car Mileage: ");
                string mileageInput = Console.ReadLine();
                if (int.TryParse(mileageInput, out carMileage) && carMileage >= 0)
                {
                    break;
                }
                Console.WriteLine("Invalid input. Car Mileage must be a non-negative integer.");
            }

            // Validate Car Year
            int year;
            while (true)
            {
                Console.WriteLine("Enter Car Year: ");
                string yearInput = Console.ReadLine();
                if (int.TryParse(yearInput, out year) && year > 1885 && year <= DateTime.Now.Year)
                {
                    break;
                }
                Console.WriteLine($"Invalid input. Car Year must be between 1886 and {DateTime.Now.Year}.");
            }

            // Validate Car Plate Number
            string carPlateNo;
            while (true)
            {
                Console.WriteLine("Enter Car Plate Number: ");
                carPlateNo = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(carPlateNo))
                {
                    break;
                }
                Console.WriteLine("Invalid input. Car Plate Number cannot be empty.");
            }
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

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Press Enter to start the booking: ");
                bookingInput = Console.ReadLine();

                if (string.IsNullOrEmpty(bookingInput))
                {
                    break;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Invalid input. Please press Enter to start the booking.");
                }
            }


            Console.WriteLine();
            Console.WriteLine("Car Details:");
            Console.WriteLine($"{"License Plate:",-14} {selectedCar.LicensePlate,-9} {"Make:",-5} {selectedCar.CarMake,-15} {"Model:",-6} {selectedCar.Model,-9} {"Year:",-5} {selectedCar.Year,-6} {"Mileage:",-8} {selectedCar.Mileage,-14} {"Availability:",-13} {selectedCar.Availability}");

            var availableDates = selectedCar.AvailableDates.Except(selectedCar.UnavailableDates).ToList();

            Console.WriteLine();
            Console.WriteLine("Available Dates:");
            foreach (var date in availableDates)
            {
                Console.WriteLine(date);
            }

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Do you want to filter the available time slots by selecting a specific month? ‘Y’ or ‘N’");
                string filterChoice = Console.ReadLine().Trim().ToUpper();
                Console.WriteLine();

                if (filterChoice == "Y")
                {
                    Console.WriteLine("Enter the month (1-12) you want to filter:");
                    if (int.TryParse(Console.ReadLine(), out int month) && month >= 1 && month <= 12)
                    {
                        var filteredDates = availableDates.Where(date => DateTime.TryParse(date, out DateTime dt) && dt.Month == month).ToList();

                        Console.WriteLine($"Available dates for month {month}:");
                        foreach (var date in filteredDates)
                        {
                            Console.WriteLine(date);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid month. Please enter a number between 1 and 12.");
                    }
                }
                else if (filterChoice == "N")
                {
                    string startDateTime = "";
                    string endDateTime = "";

                    while (true)
                    {
                        Console.WriteLine("Please enter the start date and time slot for your booking (yyyy-MM-dd, hh:mm tt): ");
                        startDateTime = Console.ReadLine();

                        if (availableDates.Contains(startDateTime) && !selectedCar.UnavailableDates.Contains(startDateTime))
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid start date and time or it is unavailable. Please try again.");
                        }
                    }

                    while (true)
                    {
                        Console.WriteLine("Please enter the end date and time slot for your booking (yyyy-MM-dd hh:mm tt): ");
                        endDateTime = Console.ReadLine();

                        if (availableDates.Contains(endDateTime) && !selectedCar.UnavailableDates.Contains(endDateTime))
                        {
                            DateTime startDate = DateTime.ParseExact(startDateTime, "yyyy-MM-dd hh:mm tt", null);
                            DateTime endDate = DateTime.ParseExact(endDateTime, "yyyy-MM-dd hh:mm tt", null);

                            if (endDate > startDate)
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("End date and time must be after the start date and time. Please try again.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid end date and time or it is unavailable. Please try again.");
                        }
                    }

                    var bookingDates = availableDates
                        .Where(date => string.Compare(date, startDateTime) >= 0 && string.Compare(date, endDateTime) <= 0)
                        .ToList();

                    selectedCar.UnavailableDates.AddRange(bookingDates);

                    Console.WriteLine("Booking confirmed for the following dates and times:");
                    foreach (var date in bookingDates)
                    {
                        Console.WriteLine(date);
                    }

                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter 'Y' for yes or 'N' for no.");
                }
            }
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

//display method
void display(string message)
{
    Console.WriteLine(message);
}

//return car
void returnCar()
{
    Console.WriteLine("Select: \n[1] Return to iCar Station \n[2] Return from Desired Location");
    string option = Console.ReadLine();
    if (option == "1")
    {
        returnToiCarStation();
    }
    else if (option == "2")
    {
        returnFromDesiredLocation();
    }
    else
    {
        Console.WriteLine("Not a valid option. Returning to Main Screen.");
    }
}

// return to iCar Station
void returnToiCarStation()
{
    double totalReturnFee = 0;
    Booking booking = getOngoingBooking((Renter)user);
    SelfReturn returnMethod = (SelfReturn) booking.ReturnMethod;
    DateTime retDateTime = DateTime.Now;
    returnMethod.DateTimeReturn = retDateTime;
    DateTime endDate = booking.EndDate;
    if (retDateTime > endDate)
    {
        double penaltyFee = calculatePenaltyFee(retDateTime, endDate, booking);
        totalReturnFee += penaltyFee;
        booking.updatePenaltyFee(penaltyFee);
        string message = "Penalty Fee for late return: " + penaltyFee;
    }
    promptCheckForDamages();
    //not done yet
    
}

// get ongoing bookings
Booking getOngoingBooking(Renter user)
{
    Booking ongoingBooking = null;
    foreach (Booking booking in user.Bookings)
    {
        if (booking.PickUpMethod != null)
        {
            ongoingBooking = booking;
        }
        else continue;
    }
    return ongoingBooking;
}

//calculate penalty fee
double calculatePenaltyFee(DateTime retDateTime, DateTime endDate, Booking ongoingBooking)
{
    double penaltyFee = 0;
    TimeSpan overTime = retDateTime - endDate;
    double totalFee = ongoingBooking.Payment.TotalFee; //get current total cost of booking
    penaltyFee = totalFee * 0.20 * overTime.Hours;
    return penaltyFee;
}

void promptCheckForDamages()
{
    Console.WriteLine("Please check for damages. If there are damages, enter ");
}

//return from desired location [empty]
void returnFromDesiredLocation() { }