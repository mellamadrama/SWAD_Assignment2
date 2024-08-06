// See https://aka.ms/new-console-template for more information

using SWAD_Assignment2;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Formats.Asn1;
using System.Globalization;
using System.Xml.Linq;

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

// populate insurance list 
static List<Insurance> LoadInsuranceFromCSV(string insuranceCsvFilePath, Dictionary<int, string> companyDictionary)
{
    var insuranceList = new List<Insurance>();

    foreach (var line in File.ReadLines(insuranceCsvFilePath).Skip(1))
    {
        var values = line.Split(",");
        int branchNo = Convert.ToInt32(values[0].Trim());
        string carPlateNo = values[1].Trim();
        int carOwnerId = Convert.ToInt32(values[2].Trim());
        DateTime expiryDate = Convert.ToDateTime(values[3].Trim());

        if (companyDictionary.TryGetValue(branchNo, out var companyName))
        {
            var insurance = new Insurance
            {
                BranchNo = branchNo,
                CompanyName = companyName,
                CarPlateNo = carPlateNo,
                CarOwnerId = carOwnerId,
                ExpiryDate = expiryDate
            };

            insuranceList.Add(insurance);
        }
    }
    return insuranceList;
}

static Dictionary<int, string> LoadCompanyDictionary(string icCsvFilePath)
{
    var companyDictionary = new Dictionary<int, string>();

    foreach (var line in File.ReadLines(icCsvFilePath).Skip(1))
    {
        var values = line.Split(",");
        int branchNo = Convert.ToInt32(values[0].Trim());
        string companyName = values[1].Trim();

        if (!companyDictionary.ContainsKey(branchNo))
        {
            companyDictionary.Add(branchNo, companyName);
        }
    }

    return companyDictionary;
}

// populate car list
static List<Car> LoadCarsFromCSV(string carCsvFilePath, List<string> dates, List<Insurance> insuranceList, Dictionary<string, List<string>> photoList)
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
        int mileage = Convert.ToInt32(values[5].Trim());
        string availability = values[6].Trim();
        string status = values[7].Trim();
        float charge = float.Parse(values[8].Trim());

        Insurance insurance = null;
        if (status == "Y")
        {
            insurance = insuranceList.FirstOrDefault(i => i.CarPlateNo == licensePlate && i.CarOwnerId == ownerId);
        }

        List<Booking> bookings = new List<Booking>(); // empty list of bookings 

        List<string> carPhotos = photoList.ContainsKey(licensePlate) ? photoList[licensePlate] : new List<string>();

        var car = new Car(ownerId, licensePlate, carMake, model, year, mileage, availability, status, charge, insurance, bookings, carPhotos);

        car.AvailableDates = new List<string>(dates);

        if (values.Length > 9)
        {
            car.AvailableDates = values[9].Split(';').ToList();
        }

        if (values.Length > 10)
        {
            car.UnavailableDates = values[10].Split(';').ToList();
        }

        cars.Add(car);
    }

    return cars;
}

static Dictionary<string, List<string>> LoadPhotoListFromCSV(string photoCsvFilePath)
{
    var photoList = new Dictionary<string, List<string>>();

    foreach (var line in File.ReadLines(photoCsvFilePath).Skip(1)) 
    {
        var values = line.Split(',');
        string licensePlate = values[0].Trim();
        var photos = new List<string>();

        for (int i = 1; i <= 5; i++)
        {
            if (!string.IsNullOrEmpty(values[i].Trim()) && values[i].Trim() != "nil")
            {
                photos.Add(values[i].Trim());
            }
        }

        photoList[licensePlate] = photos;
    }

    return photoList;
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

static List<string> ReadLocationsFromCsv(string locationsCsvFilePath)
{
    var locations = new List<string>();

    foreach (var line in File.ReadLines(locationsCsvFilePath).Skip(1))
    {
        var values = (line.Split(",")).ToList();
        string code = values[0].Trim();
        string add = values[1].Trim();
        string country = values[2].Trim();

        locations.Add($"{code} {add} {country}");
    }
    return locations;
}

static List<PaymentMethod> LoadPaymentMethodsFromCSV(string paymentMethodFilePath)
{
    var paymentMethods = new List<PaymentMethod>();

    foreach (var line in File.ReadLines(paymentMethodFilePath).Skip(1))
    {
        var values = line.Split(',');

        string paymentMethodType = values[0].Trim();
        string cardNum = values[1].Trim();
        string name = values[2].Trim();
        double balance = Convert.ToDouble(values[3].Trim());

        string bank = values[4].Trim();

        switch (paymentMethodType)
        {
            case "DebitCard":
                paymentMethods.Add(new DebitCard(cardNum, name, balance, bank));
                break;
            case "DigitalWallet":
                paymentMethods.Add(new DigitalWallet(name, bank, balance));
                break;
            case "CreditCard":
                paymentMethods.Add(new CreditCard(cardNum, name, balance, bank));
                break;
        }
    }

    return paymentMethods;
}

// List of car makes
List<string> carMakes = new List<string>
{
    "toyota","honda","ford","chevrolet","nissan","bmw","mercedes-benz","volkswagen","audi","hyundai","kia","mazda","subaru","lexus","jaguar","porsche","land rover",
    "volvo","tesla","ferrari","lamborghini","bentley","rolls-royce","maserati","aston martin","alfa romeo","peugeot","renault","citroën","fiat"
};

string usercsvFilePath = "Users_Data.csv";
string carCsvFilePath = "Car_List.csv";
string icCsvFilePath = "Insurance_Company_List.csv";
string insuranceCsvFilePath = "Insurance_list.csv";
string datesCsvFilePath = "DateTimeSlots.csv";
string paymenthMethodFilePath = "PaymentMethods.csv";
string locationsCsvFilePath = "iCar_Locations.csv";
string photoCsvFilePath = "Photo_List.csv";

var users = LoadUsersFromCSV(usercsvFilePath);
var dates = LoadDateListFromCSV(datesCsvFilePath);
var companyDictionary = LoadCompanyDictionary(icCsvFilePath);
var insuranceList = LoadInsuranceFromCSV(insuranceCsvFilePath, companyDictionary);
var paymentMethods = LoadPaymentMethodsFromCSV(paymenthMethodFilePath);
var photoList = LoadPhotoListFromCSV(photoCsvFilePath);
var cars = LoadCarsFromCSV(carCsvFilePath, dates, insuranceList, photoList);
var locations = ReadLocationsFromCsv(locationsCsvFilePath);

User user = null;
int emailAttempts = 0;
bool emailValid = false;
string email = string.Empty;

User login()
{
    Console.Write("========Welcome! Please login below.========");
    Console.WriteLine();
    Console.WriteLine();

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
        Console.WriteLine();
        Console.WriteLine("Too many failed attempts. System will exit.");
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
                return user;
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
    return null;
}

user = login();

if (user != null)
{
    displayUserDetails(user);

    if (user is Car_Owner carOwner)
    {
        while (true)
        {
            displayCarOwnerMainMenu();

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewCars(cars, carOwner);
                    break;
                case "2":
                    RegisterCar(cars, insuranceList, carMakes);
                    break;
                case "3":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
        static void ViewCars(List<Car> cars, Car_Owner carOwner) 
        {
            var ownerCars = cars.Where(c => c.CarOwnerId == carOwner.Id).ToList();
            Console.WriteLine();
            Console.WriteLine("====Cars Owned====");
            foreach (var car in ownerCars)
            {
                Console.WriteLine($"{"License Plate:",-14} {car.LicensePlate,-9} {"Make:",-5} {car.CarMake,-15} {"Model:",-6} {car.Model,-9} {"Year:",-5} {car.Year,-6} {"Mileage:",-8} {car.Mileage} {"Availability:", -15} {car.Availability} {"Insurance Status:", -5} {car.InsuranceStatus}");
            }
        }

        static bool IsValidFileType(string fileName)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
            string fileExtension = System.IO.Path.GetExtension(fileName);
            return allowedExtensions.Contains(fileExtension);
        }

        static void AppendCarToCSV(string carCsvFilePath, Car newCar)
        {
            using (var writer = new StreamWriter(carCsvFilePath, true))
            {
                string line = $"{newCar.CarOwnerId},{newCar.LicensePlate},{newCar.CarMake},{newCar.Model},{newCar.Year},{newCar.Mileage},{newCar.Availability},{newCar.InsuranceStatus}";
                writer.WriteLine(line);
            }
        }

        static void AppendPhotosToCSV(string photoCsvFilePath, string carPlateNo, List<string> photoFiles)
        {
            const int MaxPhotos = 5;
            var p = new List<string>(photoFiles);

            while (p.Count < MaxPhotos)
            {
                p.Add("nil");
            }

            if (p.Count > MaxPhotos)
            {
                p = p.Take(MaxPhotos).ToList();
            }
            string photoList = string.Join(",", p);

            using (var writer = new StreamWriter(photoCsvFilePath, true))
            {
                writer.WriteLine($"{carPlateNo},{photoList}");
            }
        }

        static void RegisterCar(List<Car> cars, List<Insurance> insuranceList, List<string> carMakes)
        {
            Console.WriteLine();

            // prompt car owner for car details
            Console.WriteLine("========Please enter car details========");

            // Validate Car Plate Number
            string carPlateNo;
            while (true)
            {
                Console.Write("Enter Car Plate Number: ");
                carPlateNo = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(carPlateNo))
                {
                    if (cars.Any(c => c.LicensePlate == carPlateNo))
                    {
                        Console.Write("Are you sure? This car plate number has already been registered. (Exit/Edit to re-enter Car plate number): ");
                        string response = Console.ReadLine().Trim().ToLower();
                        if (response == "exit")
                        {
                            break;
                        }
                        else if (response == "edit")
                        {
                            continue;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please type 'Exit' or 'Edit'.");
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Car Plate Number cannot be empty.");
                }
            }

            // Validate Car Make
            string carMake;
            while (true)
            {
                Console.Write("Enter Car Make: ");
                carMake = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(carMake) && carMakes.Contains(carMake.ToLower()))
                {
                    break;
                }
                Console.WriteLine("Invalid input. Car Make cannot be empty or must be a valid car make.");
            }
            // Validate Car Model
            string carModel;
            while (true)
            {
                Console.Write("Enter Car Model: ");
                carModel = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(carModel))
                {
                    break;
                }
                Console.WriteLine("Invalid input. Car Model cannot be empty.");
            }

            // Validate Car Year
            int year;
            while (true)
            {
                Console.Write("Enter Car Year: ");
                string yearInput = Console.ReadLine();
                if (int.TryParse(yearInput, out year) && year > 1885 && year <= DateTime.Now.Year)
                {
                    break;
                }
                Console.WriteLine($"Invalid input. Car Year must be between 1886 and {DateTime.Now.Year}.");
            }

            // Validate Car Mileage
            int carMileage;
            while (true)
            {
                Console.Write("Enter Car Mileage (miles): ");
                string mileageInput = Console.ReadLine();
                if (int.TryParse(mileageInput, out carMileage) && carMileage >= 0)
                {
                    break;
                }
                Console.WriteLine("Invalid input. Car Mileage must be a non-negative integer.");
            }

            // Car Availability
            string availability = "Available";

            // Validate photos
            List<string> photoFiles = new List<string>();

            Console.WriteLine();
            Console.WriteLine("========Please upload images of the car!========");
            
            string photoFile;
            while (true)
            {
                if (photoFiles.Count >= 5)
                {
                    Console.WriteLine("You have reached the maximum number of photo uploads (5).");
                    break;
                }

                Console.Write("Upload Photo (jpg/png/jpeg/pdf) or type 'done' to finish: ");
                photoFile = Console.ReadLine().Trim().ToLower();

                if (photoFile == "done")
                {
                    break;
                }

                if (IsValidFileType(photoFile))
                {
                    photoFiles.Add(photoFile);
                    Console.WriteLine($"Photo '{photoFile}' uploaded successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid file type. Please upload a file with a valid extension (jpg, png, jpeg, pdf).");
                }
            }

            // Process the uploaded photos
            Console.WriteLine();
            Console.WriteLine("====Uploaded photos====");
            foreach (var file in photoFiles)
            {
                Console.WriteLine(file);
            }

            // Set Car Owner Charge (per hour)
            Console.WriteLine();
            Console.WriteLine("====Set Hourly Charge ($)====");
            float charge;
            while(true)
            {
                Console.Write("Enter Hourly Charge ($): ");
                string Charge = Console.ReadLine();

                if (float.TryParse(Charge, NumberStyles.Float, CultureInfo.InvariantCulture, out charge))
                {
                    // Round the charge to 2 decimal places
                    charge = (float)Math.Round(charge, 2);

                    // Ensure the charge has 2 decimal places for display
                    Console.WriteLine($"Hourly Charge Set: ${charge:F2}");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
            
            // Check if car plate number exists in the insuranceList
            string insuranceStatus = insuranceList.Any(i => i.CarPlateNo == carPlateNo) ? "Y" : "X";

            Console.WriteLine();
            Console.WriteLine("========Car details summary========");
            Console.WriteLine($"Car Plate Number: {carPlateNo}");
            Console.WriteLine($"Car Make: {carMake}");
            Console.WriteLine($"Car Model: {carModel}");
            Console.WriteLine($"Year: {year}");
            Console.WriteLine($"Car Mileage: {carMileage}");
            Console.WriteLine($"Availability: {availability}");
            Console.WriteLine($"Hourly Charge: ${charge}");
            if (insuranceStatus == "Y")
            {
                Console.WriteLine($"Insurance Status: {insuranceStatus}");
                var insurance = insuranceList.FirstOrDefault(i => i.CarPlateNo == carPlateNo);
                if (insurance != null)
                {
                    Console.WriteLine($"Insurance Company: {insurance.CompanyName}");
                    Console.WriteLine($"Branch Number: {insurance.BranchNo}");
                    Console.WriteLine($"Expiry Date: {insurance.ExpiryDate.ToShortDateString()}");
                }
                else
                {
                    Console.WriteLine("No insurance details found for this car plate number.");
                }
            }
            else
            {
                Console.WriteLine($"Insurance Status: {insuranceStatus}");
                Console.WriteLine("No insurance details found for this car plate number.");
            }

            Console.WriteLine("====Uploaded photos:====");
            foreach (var file in photoFiles)
            {
                Console.WriteLine(file);
            }

            // comfirmation for car registration
            while (true)
            {
                Console.WriteLine();
                Console.Write("Are you sure you want to register this car? (yes/no) ");
                string response = Console.ReadLine().Trim().ToLower();

                if (response == "yes")
                {
                    var newCar = new Car
                    {
                        LicensePlate = carPlateNo,
                        CarMake = carMake,
                        Model = carModel,
                        Year = year,
                        Mileage = carMileage,
                        Availability = availability,
                        InsuranceStatus = insuranceStatus,
                        Charge = charge,
                    };
                    cars.Add(newCar);
                    AppendCarToCSV("Car_List.csv", newCar);
                    AppendPhotosToCSV("Photo_List.csv", carPlateNo, photoFiles);
                    Console.WriteLine("====Car Successfully Registered!====");
                    break;
                }
                else if (response == "no")
                {
                    Console.WriteLine("====Car registration canceled.====");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please type 'yes' or 'no'.");
                }
            }
        }
    }
    else if (user is Renter renter)
    {

        while (true)
        {
            displayRenterMainMenu();

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
                    returnCar();
                    break;
                case "5":
                    PickUpCar(renter);
                    break;
                case "6":
                    Console.WriteLine("Logging out...");
                    Console.WriteLine();
                    user = login();
                    displayUserDetails(user);
                    break;
                case "7": 
                    MakePayment();
                    break;
                case "0":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

        static void BookCar(List<Car> cars)
        {
            bool redoBooking = true;
            while (redoBooking)
            {
                redoBooking = false;

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
                var originalAvailableDates = new List<string>(availableDates);
                var originalUnavailableDates = new List<string>(selectedCar.UnavailableDates);

                Console.WriteLine();
                Console.WriteLine("Available Dates:");
                foreach (var date in availableDates)
                {
                    Console.WriteLine(date);
                }

                bool filteringCompleted = false;
                while (!filteringCompleted)
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
                        filteringCompleted = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter 'Y' for yes or 'N' for no.");
                    }
                }

                string startDateTime = "";
                string endDateTime = "";

                while (true)
                {
                    Console.WriteLine("Please enter the start date and time slot for your booking (yyyy-MM-dd hh:mm tt): ");
                    startDateTime = Console.ReadLine();

                    if (availableDates.Contains(startDateTime) && !selectedCar.UnavailableDates.Contains(startDateTime) && availableDates.IndexOf(startDateTime) != availableDates.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid start date and time or it is unavailable, or it is the last available date. Please try again.");
                    }
                }

                while (true)
                {
                    Console.WriteLine("Please enter the end date and time slot for your booking (yyyy-MM-dd hh:mm tt): ");
                    endDateTime = Console.ReadLine();

                    if (availableDates.Contains(endDateTime) && !selectedCar.UnavailableDates.Contains(endDateTime) && availableDates.IndexOf(endDateTime) != 0)
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
                        Console.WriteLine("Invalid end date and time or it is unavailable, or it is the first available date. Please try again.");
                    }
                }

                int startIndex = availableDates.FindIndex(date => date == startDateTime);
                int endIndex = availableDates.FindIndex(date => date == endDateTime);

                if (startIndex != -1 && endIndex != -1 && startIndex <= endIndex)
                {
                    for (int i = startIndex; i <= endIndex; i++)
                    {
                        selectedCar.UnavailableDates.Add(availableDates[i]);
                    }

                    availableDates.RemoveRange(startIndex, endIndex - startIndex + 1);
                }

                Console.WriteLine();
                Console.WriteLine("Do you want to pick up the car yourself or have it delivered? Enter 'P' for pickup or 'D' for delivery: ");
                string pickupOrDelivery = Console.ReadLine().Trim().ToUpper();

                PickUpMethod pickUpMethod = null;
                decimal deliveryFee = 0;

                if (pickupOrDelivery == "P")
                {
                    Pickup selfpickup = new Pickup
                    {
                        DateTimePickup = DateTime.ParseExact(startDateTime, "yyyy-MM-dd hh:mm tt", null)
                    };

                    var locations = ReadLocationsFromCsv("iCar_Locations.csv");

                    Console.WriteLine("List of Pickup Locations:");
                    for (int i = 0; i < locations.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {locations[i]}");
                    }

                    while (true)
                    {
                        Console.WriteLine("Enter the number of the location where you want to pick up the car: ");
                        if (int.TryParse(Console.ReadLine(), out int locationIndex) && locationIndex >= 1 && locationIndex <= locations.Count)
                        {
                            selfpickup.PickupLocation = locations[locationIndex - 1];
                            pickUpMethod = selfpickup;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid location number. Please try again.");
                        }
                    }

                }
                else if (pickupOrDelivery == "D")
                {
                    DeliverCar delivery = new DeliverCar
                    {
                        DateTimeDeliver = DateTime.ParseExact(startDateTime, "yyyy-MM-dd hh:mm tt", null)
                    };

                    while (true)
                    {
                        Console.WriteLine("Enter the delivery location in this format: Postal Code, Address, Country: ");
                        string deliveryLocation = Console.ReadLine();

                        string[] parts = deliveryLocation.Split(',');
                        if (parts.Length == 3 && parts[2].Trim().Equals("Singapore", StringComparison.OrdinalIgnoreCase))
                        {
                            string postalCode = parts[0].Trim();

                            if (postalCode.Length == 6 && postalCode.All(char.IsDigit))
                            {
                                delivery.DeliveryLocation = deliveryLocation;
                                pickUpMethod = delivery;
                                deliveryFee += 10;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid postal code. It must be exactly 6 digits long and contain only numbers. Please try again.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid location. Ensure the country is 'Singapore' and the format is correct. Please try again.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter 'P' for pickup or 'D' for delivery.");
                    return;
                }

                Console.WriteLine();
                Console.WriteLine("Do you want to return the car yourself or have it picked up? Enter 'S' for self-return or 'D' for delivery return: ");
                string returnMethodChoice = Console.ReadLine().Trim().ToUpper();

                ReturnMethod returnMethod = null;

                if (returnMethodChoice == "S")
                {
                    SelfReturn selfReturn = new SelfReturn
                    {
                        DateTimeReturn = DateTime.ParseExact(endDateTime, "yyyy-MM-dd hh:mm tt", null)
                    };

                    Console.WriteLine("Please select a return location:");

                    var locations = ReadLocationsFromCsv("iCar_Locations.csv");

                    for (int i = 0; i < locations.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {locations[i]}");
                    }

                    while (true)
                    {
                        Console.WriteLine("Enter the number of the location where you want to return the car: ");
                        if (int.TryParse(Console.ReadLine(), out int locationIndex) && locationIndex >= 1 && locationIndex <= locations.Count)
                        {
                            selfReturn.ICarReturnLocation = locations[locationIndex - 1];
                            returnMethod = selfReturn;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid location number. Please try again.");
                        }
                    }
                }
                else if (returnMethodChoice == "D")
                {
                    DeliveryReturn deliveryReturn = new DeliveryReturn
                    {
                        DateTimeReturnDelivery = DateTime.ParseExact(endDateTime, "yyyy-MM-dd hh:mm tt", null),
                        AdditionalCharge = new AdditionalCharge() 
                    };

                    while (true)
                    {
                        Console.WriteLine("Enter the return delivery location in this format: Postal Code, Address, Country: ");
                        string returnLocation = Console.ReadLine();

                        string[] parts = returnLocation.Split(',');
                        if (parts.Length == 3 && parts[2].Trim().Equals("Singapore", StringComparison.OrdinalIgnoreCase))
                        {
                            string postalCode = parts[0].Trim();

                            if (postalCode.Length == 6 && postalCode.All(char.IsDigit))
                            {
                                deliveryReturn.ReturnLocation = returnLocation;
                                returnMethod = deliveryReturn;
                                deliveryFee += 10;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid postal code. It must be exactly 6 digits long and contain only numbers. Please try again.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid location. Ensure the country is 'Singapore' and the format is correct. Please try again.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter 'S' for self-return or 'D' for delivery return.");
                    return;
                }

                DateTime startTime = DateTime.ParseExact(startDateTime, "yyyy-MM-dd hh:mm tt", null);
                DateTime endTime = DateTime.ParseExact(endDateTime, "yyyy-MM-dd hh:mm tt", null);
                TimeSpan duration = endTime - startTime;
                decimal totalHours = (decimal)duration.TotalHours;
                decimal totalCharge = totalHours * (decimal)selectedCar.Charge;
                decimal totalDeliveryFee = deliveryFee;

                Console.WriteLine();
                Console.WriteLine("Booking successfully made!");
                Console.WriteLine();

                Console.WriteLine("Booking Details:");
                Console.WriteLine($"Car License Plate: {selectedCar.LicensePlate}");
                Console.WriteLine($"Booking Start Date and Time: {startDateTime}");
                Console.WriteLine($"Booking End Date and Time: {endDateTime}");
                Console.WriteLine();

                if (pickUpMethod is Pickup pickup)
                {
                    Console.WriteLine("Pickup & Return");
                    Console.WriteLine($"Pickup Method: Self-Pickup");
                    Console.WriteLine($"Pickup Date and Time: {pickup.DateTimePickup}");
                    Console.WriteLine($"Pickup Location: {pickup.PickupLocation}");
                }
                else if (pickUpMethod is DeliverCar deliverCar)
                {
                    Console.WriteLine("Pickup & Return");
                    Console.WriteLine($"Pickup Method: Delivery");
                    Console.WriteLine($"Delivery Date and Time: {deliverCar.DateTimeDeliver}");
                    Console.WriteLine($"Delivery Location: {deliverCar.DeliveryLocation}");
                }

                if (returnMethod is SelfReturn selfReturnMethod)
                {
                    Console.WriteLine($"Return Method: Self-Return");
                    Console.WriteLine($"Return Date and Time: {selfReturnMethod.DateTimeReturn}");
                    Console.WriteLine($"Return Location: {selfReturnMethod.ICarReturnLocation}");
                }
                else if (returnMethod is DeliveryReturn deliveryReturnMethod)
                {
                    Console.WriteLine($"Return Method: Delivery Return");
                    Console.WriteLine($"Return Date and Time: {deliveryReturnMethod.DateTimeReturnDelivery}");
                    Console.WriteLine($"Return Location: {deliveryReturnMethod.ReturnLocation}");
                }

                Console.WriteLine("Amount to pay");
                Console.WriteLine($"Total Charge: {totalCharge:C}");
                Console.WriteLine($"Total Delivery Fee: {totalDeliveryFee:C}");
                Console.WriteLine($"Final Total: {(totalCharge + totalDeliveryFee):C}");

                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine("Would you like to: [C]onfirm the booking, [R]edo the booking, or [E]xit and cancel the booking?");
                    string choice = Console.ReadLine().ToUpper();

                    if (choice == "C")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Booking Confirmed!");
                        Booking booking = new Booking
                        {
                            BookingId = Guid.NewGuid().ToString(),
                            StartDate = startTime,
                            EndDate = endTime,
                            Status = "Confirmed",
                            PickUpMethod = pickUpMethod,
                            ReturnMethod = returnMethod,
                            Payment = new Payment(),
                            Car = selectedCar
                        };
                        redoBooking = false;
                        break;
                    }
                    else if (choice == "R")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Redoing the booking...");
                        selectedCar.AvailableDates = new List<string>(originalAvailableDates);
                        selectedCar.UnavailableDates = new List<string>(originalUnavailableDates);
                        redoBooking = true;
                        break;
                    }
                    else if (choice == "E")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Booking Canceled. Exiting.");
                        selectedCar.AvailableDates = new List<string>(originalAvailableDates);
                        selectedCar.UnavailableDates = new List<string>(originalUnavailableDates);
                        redoBooking = false;
                        return;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Invalid choice. Please enter 'C', 'R', or 'E'.");
                    }
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

// display 
void displayRenterMainMenu() {
    Console.WriteLine();
    Console.WriteLine("1. Book a Car");
    Console.WriteLine("2. View Booking History");
    Console.WriteLine("3. View Payment History");
    Console.WriteLine("4. Return Car");
    Console.WriteLine("5. Pick up car");
    Console.WriteLine("6. Logout");
    Console.WriteLine("7. payment just to test i will remove this later");
    Console.WriteLine("0. Exit");
    Console.WriteLine("Choose an option:");
}
// i done alr u can test if u want

void displayCarOwnerMainMenu()
{
    Console.WriteLine();
    Console.WriteLine("========Menu========");
    Console.WriteLine("1. View Cars Owned");
    Console.WriteLine("2. Register Car");
    Console.WriteLine("3. Exit");
    Console.Write("Choose an Option: ");
}

void displayUserDetails(User user)
{
    Console.WriteLine();
    Console.WriteLine($"========Welcome, {user.FullName}========");
    Console.WriteLine($"Role: {user.GetRole()}");
    Console.WriteLine($"Date of Birth: {user.DateOfBirth.ToShortDateString()}");

    if (user.GetRole() == "Renter")
    {
        Renter renter = ((Renter)user);
        Console.WriteLine($"Licence: {renter.LicenseNum}");
        Console.WriteLine($"License Status: {renter.LicenseStatus}");
        Console.WriteLine($"Demerit Points: {renter.DemeritPoints}");
    }
    else if (user.GetRole() == "Car Owner")
    {
        Car_Owner carOwner = ((Car_Owner)user);
        Console.WriteLine($"Licence: {carOwner.License}");
    }
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
    SelfReturn returnMethod = new SelfReturn();
    DateTime retDateTime = DateTime.Now;
    returnMethod.DateTimeReturn = retDateTime;
    booking.ReturnMethod = returnMethod;
    DateTime endDate = booking.EndDate;
    if (retDateTime > endDate)
    {
        double penaltyFee = calculatePenaltyFee(retDateTime, endDate, booking);
        totalReturnFee += penaltyFee;
        booking.updatePenaltyFee(penaltyFee);
        string PenaltyFee = "Penalty Fee for late return: " + penaltyFee;
        display(PenaltyFee);
    }
    string damages = promptCheckForDamages();
    updateDamages(damages);
    if (totalReturnFee > 0)
    {
        booking.updateTotalFees(totalReturnFee);
        MakePayment();
    }
    else
    {
        string NoFees = "No outstanding fees.";
        display(NoFees);
    }
    string status = "Completed";
    booking.updateBookingStatus(status);
    string message = "Return " + status;
    display(message);
}

// get ongoing bookings
Booking getOngoingBooking(Renter user)
{
    Booking ongoingBooking = null;
    foreach (Booking booking in user.Bookings)
    {
        if (booking.Status == "Picked Up") 
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
    penaltyFee =Math.Round(penaltyFee, 2);
    return penaltyFee;
}

string promptCheckForDamages()
{
    Console.WriteLine("Please check for damages. If there are damages, enter 'Has Damages'. Else enter 'No Damages'.");
    string damages = Console.ReadLine();
    while (damages != "Has Damages" && damages != "No Damages")
    {
        Console.WriteLine("Invalid input. Try again.");
        Console.WriteLine("Please check for damages. If there are damages, enter 'Has Damages'. Else enter 'No Damages'.");
        damages = Console.ReadLine();
    }
    return damages;
}

void updateDamages(string damages)
{
    if (damages == "Has Damages")
    {
        reportAccident();
    }
}

void reportAccident() { }

void PickUpCar(Renter user)
{
    // Mock data
    string startPick = "2024-12-03";
    DateTime startPickDate = DateTime.ParseExact(startPick, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    string returnDate = "2024-12-07";
    DateTime returnDateDT = DateTime.ParseExact(returnDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

    // Initialize Pickup and SelfReturn
    PickUpMethod pickUp = new Pickup(startPickDate, "ABC Location");
    AdditionalCharge additionalCharge = new AdditionalCharge(0, 0);
    ReturnMethod returnMethod = new SelfReturn(returnDateDT, "ABC Location", additionalCharge);

    // Payment methods
    List<PaymentMethod> paymentMethods = new List<PaymentMethod>();
    DigitalWallet digitalWallet = new DigitalWallet("JOHN DOE", "PAYPAL", 10000);
    paymentMethods.Add(digitalWallet);

    // Payment details
    Payment payment = new Payment(returnDateDT, 123, additionalCharge, paymentMethods);
    DateTime bookingDate = DateTime.ParseExact("2024-03-20", "yyyy-MM-dd", CultureInfo.InvariantCulture);

    // Initialize Car and Booking
    Car car = new Car();
    Booking booking = new Booking("1", bookingDate, bookingDate, "Picked Up", pickUp, returnMethod, payment, car);

    // Add mock data to the user's list of bookings
    user.Bookings.Add(booking);

    Console.WriteLine("Pickup confirmed.");
}




// make payment 
void MakePayment() {
    string startPick = "2024-12-03";
    DateTime startPickDate = DateTime.ParseExact(startPick, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    string returnDate = "2024-12-07";
    DateTime returnDateDT = DateTime.ParseExact(returnDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

    Pickup pickUp = new Pickup(startPickDate, "ABC Location");
    AdditionalCharge ac = new AdditionalCharge(0, 0);
    SelfReturn ret = new SelfReturn(returnDateDT, "ABC Location", ac);

    List<PaymentMethod> pmList = new List<PaymentMethod>();
    DigitalWallet cc = new DigitalWallet("JOHN DOE", "PAYPAL", 10000);
    pmList.Add(cc);

    Payment payment = new Payment(returnDateDT, 123, ac, pmList); 
    DateTime bookingDate = DateTime.ParseExact("2024-03-20", "yyyy-MM-dd", CultureInfo.InvariantCulture);

    Car car = new Car();
    Booking booking = new Booking("1", bookingDate, bookingDate, "Pending", pickUp, ret, payment, car);

    displayBooking(booking);
    string name = user.FullName;

    Console.WriteLine("Confirm payment amount? (yes/no) ");
    string res = Console.ReadLine();

    if (res != "yes")
    {
        Console.WriteLine("Payment Cancelled.");
        return;
    }

    (PaymentMethod selectedPaymentMethod, double accBalance) = validatePaymentMethod();

    while (accBalance < booking.Payment.TotalFee)
    {
        Console.WriteLine("Balance insufficient! Please choose another payment method!");
        (selectedPaymentMethod, accBalance) = validatePaymentMethod();
    }


    if (booking.Status == "Pending")
    {
        selectedPaymentMethod.DeductBalance(booking.Payment.TotalFee);

        string status = "Confirmed";
        booking.updateBookingStatus(status);
    }
    else if (booking.Status == "Confirmed")
    {
        double additionalCharge = booking.Payment.AdditionalCharge.PenaltyFee + booking.Payment.AdditionalCharge.DamageFee;

        selectedPaymentMethod.DeductBalance(additionalCharge);
    }

    displayBooking(booking);

    sendReceipt((Renter)user);
}

// validate payment method exists
(PaymentMethod, double) validatePaymentMethod()
{
    PaymentMethod selectedPaymentMethod = null;

    while (selectedPaymentMethod == null)
    {
        Console.WriteLine("Proceed with payment");

        Console.WriteLine("Select Payment Method: ");
        Console.WriteLine("1. Digital Wallet");
        Console.WriteLine("2. Debit Card");
        Console.WriteLine("3. Credit Card");

        string method = Console.ReadLine();

        while (method != "1" && method != "2" && method != "3")
        {
            Console.WriteLine("Select Payment Method: ");
            Console.WriteLine("1. Digital Wallet");
            Console.WriteLine("2. Debit Card");
            Console.WriteLine("3. Credit Card");

            method = Console.ReadLine();

            if (method != "1" && method != "2" && method != "3")
            {
                Console.WriteLine("Invalid input! Please try again.");
            }
        }

        if (method == "1")
        {
            Console.WriteLine("Enter wallet type:");
            string walletType = Console.ReadLine();
            selectedPaymentMethod = paymentMethods
                .OfType<DigitalWallet>()
                .FirstOrDefault(dw => dw.Type == walletType);

            if (selectedPaymentMethod == null)
            {
                Console.WriteLine("No matching Digital Wallet found. Please try again.");
                continue;
            }

            var digitalWallet = (DigitalWallet)selectedPaymentMethod;
            string cardName;
            do
            {
                Console.WriteLine("Enter card name: ");
                cardName = Console.ReadLine();
                if (digitalWallet.Name != cardName)
                {
                    Console.WriteLine("Card name does not match user's name. Please try again.");
                    selectedPaymentMethod = null;
                    continue;
                }
            } while (cardName != digitalWallet.Name);

            return (digitalWallet, digitalWallet.Balance);
        }
        else if (method == "2")
        {
            string cardNum;
            do
            {
                Console.WriteLine("Enter card number: ");
                cardNum = Console.ReadLine();
                if (cardNum.Length != 16)
                {
                    Console.WriteLine("Invalid card number! It must be 16 digits.");
                }
            } while (cardNum.Length != 16);

            selectedPaymentMethod = paymentMethods
                .OfType<DebitCard>()
                .FirstOrDefault(dc => dc.CardNum == cardNum);

            if (selectedPaymentMethod == null)
            {
                Console.WriteLine("No matching Debit Card found. Please try again.");
                continue;
            }

            var debitCard = (DebitCard)selectedPaymentMethod;
            string cardName;
            do
            {
                Console.WriteLine("Enter card name: ");
                cardName = Console.ReadLine();
                if (debitCard.CardName != cardName)
                {
                    Console.WriteLine("Card name does not match user's name. Please try again.");
                    selectedPaymentMethod = null;
                    continue;
                }
            } while (cardName != debitCard.CardName);

            return (debitCard, debitCard.Balance);
        }
        else if (method == "3")
        {
            string cardNum;
            do
            {
                Console.WriteLine("Enter card number: ");
                cardNum = Console.ReadLine();
                if (cardNum.Length != 16 || !long.TryParse(cardNum, out _))
                {
                    Console.WriteLine("Invalid card number! It must be 16 digits.");
                }
            } while (cardNum.Length != 16 || !long.TryParse(cardNum, out _));

            selectedPaymentMethod = paymentMethods
                .OfType<CreditCard>()
                .FirstOrDefault(cc => cc.CardNum == cardNum);

            if (selectedPaymentMethod == null)
            {
                Console.WriteLine("No matching Credit Card found. Please try again.");
                continue;
            }

            var creditCard = (CreditCard)selectedPaymentMethod;
            string cardName;
            do
            {
                Console.WriteLine("Enter card name: ");
                cardName = Console.ReadLine();

                if (creditCard.CardName != cardName)
                {
                    Console.WriteLine("Card name does not match user's name. Please try again.");
                    selectedPaymentMethod = null;
                    continue;
                }
            } while (cardName != creditCard.CardName);
            return (creditCard, creditCard.Balance);
        }
    }

    return (null, 0); 
}

// send receipt method
void sendReceipt(Renter user)
{
    Console.WriteLine("How would you like receipt to be sent?");
    Console.WriteLine("1. Email");
    Console.WriteLine("2. Phone number");
    string res = Console.ReadLine();

    while (res != "1" && res != "2")
    {
        Console.WriteLine("Invalid option! Please select another option!");
        Console.WriteLine("How would you like receipt to be sent?");
        Console.WriteLine("1. Email");
        Console.WriteLine("2. Phone number");
        res = Console.ReadLine();
    }

    if (res == "1")
    {
        Console.WriteLine("Receipt sent via email to " + user.Email);
    }
    else
    {
        Console.WriteLine("Receipt sent via SMS to " + user.ContactNum);
    }
}

// display current booking details
void displayBooking(Booking currentBooking) {
    Console.WriteLine("BookingID: " + currentBooking.BookingId);
    Console.WriteLine("Start Date: " + currentBooking.StartDate.ToString());
    Console.WriteLine("End Date:  " + currentBooking.EndDate.ToString());
    Console.WriteLine("Pick Up Method: " + currentBooking.PickUpMethod);
    Console.WriteLine("Return Method: " + currentBooking.ReturnMethod);
    if (currentBooking.Status == "Pending")
    {
        Console.WriteLine("Fees owed: " + currentBooking.Payment.TotalFee);
    }
    else
    {
        Console.WriteLine("Fees owed: " + currentBooking.Payment.AdditionalCharge);
    }
};

//return from desired location [empty]
void returnFromDesiredLocation() { }
