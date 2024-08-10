// See https://aka.ms/new-console-template for more information

using SWAD_Assignment2;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Formats.Asn1;
using System.Globalization;
using System.Security.Cryptography;
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
                users.Add(new CarOwner(id, fullName, contactNum, email, password, dateOfBirth, licence, cars));
                break;
            case "Renter":
                users.Add(new Renter(id, fullName, contactNum, email, password, dateOfBirth, licence, licenseStatus, demeritPoints, bookings));
                break;
        }
    }

    return users;
}

// populate insurance list 
static List<Insurance> LoadInsuranceFromCSV(string insuranceCsvFilePath, Dictionary<int, InsuranceCompany> companyDictionary)
{
    var insuranceList = new List<Insurance>();

    foreach (var line in File.ReadLines(insuranceCsvFilePath).Skip(1))
    {
        var values = line.Split(",");
        int branchNo = Convert.ToInt32(values[0].Trim());
        string licencePlate = values[1].Trim();
        int carOwnerId = Convert.ToInt32(values[2].Trim());
        DateTime expiryDate = Convert.ToDateTime(values[3].Trim());

        if (companyDictionary.TryGetValue(branchNo, out var company))
        {
            var car = new Car
            {
                CarOwnerId = carOwnerId,
                LicensePlate = licencePlate
            };

            var insurance = new Insurance
            {
                ExpiryDate = expiryDate,
                Car = car,
                Company = company
            };

            insuranceList.Add(insurance);
        }
    }
    return insuranceList;
}

// Load all insurance companies
static Dictionary<int, InsuranceCompany> LoadCompanyDictionary(string icCsvFilePath)
{
    var companyDictionary = new Dictionary<int, InsuranceCompany>();

    foreach (var line in File.ReadLines(icCsvFilePath).Skip(1))
    {
        var values = line.Split(",");
        int branchNo = Convert.ToInt32(values[0].Trim());
        string companyName = values[1].Trim();
        string telephone = values[2].Trim();
        string address = values[3].Trim();
        string emailAddress = values[4].Trim();

        if (!companyDictionary.ContainsKey(branchNo))
        {
            companyDictionary.Add(branchNo, new InsuranceCompany
            {
                BranchNo = branchNo,
                CompanyName = companyName,
                Telephone = telephone,
                Address = address,
                EmailAddress = emailAddress
            });
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
        float charge = float.Parse(values[7].Trim());

        Insurance insurance = null;

        List<Booking> bookings = new List<Booking>(); // empty list of bookings 

        List<string> carPhotos = photoList.ContainsKey(licensePlate) ? photoList[licensePlate] : new List<string>();

        var car = new Car(ownerId, licensePlate, carMake, model, year, mileage, availability, charge, insurance, bookings, carPhotos);

        car.AvailableDates = new List<string>(dates);

        if (values.Length > 8)
        {
            car.AvailableDates = values[8].Split(';').ToList();
        }

        if (values.Length > 9)
        {
            car.UnavailableDates = values[9].Split(';').ToList();
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
InsuranceCompany selectedCompany = null;
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

    if (user is CarOwner carOwner)
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
                    selectRegisterCar();
                    break;
                case "3":
                    Console.WriteLine("Logging out...");
                    Console.WriteLine();
                    user = login();
                    displayUserDetails(user);
                    break;
                case "4":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
        void ViewCars(List<Car> cars, CarOwner carOwner)
        {
            var ownerCars = cars.Where(c => c.CarOwnerId == carOwner.Id).ToList();
            Console.WriteLine();
            Console.WriteLine("====Cars Owned====");
            foreach (var car in ownerCars)
            {
                Console.WriteLine($"{"License Plate:",-14} {car.LicensePlate,-9} {"Make:",-5} {car.CarMake,-15} {"Model:",-6} {car.Model,-9} {"Year:",-5} {car.Year,-6} {"Mileage:",-8} {car.Mileage} {"Availability:",-15} {car.Availability}");
            }
        }
        bool IsValidFileType(string fileName)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
            string fileExtension = System.IO.Path.GetExtension(fileName);
            return allowedExtensions.Contains(fileExtension);
        }
        void AppendCarToCSV(string carCsvFilePath, Car newCar)
        {
            using (var writer = new StreamWriter(carCsvFilePath, true))
            {
                string line = $"{newCar.CarOwnerId},{newCar.LicensePlate},{newCar.CarMake},{newCar.Model},{newCar.Year},{newCar.Mileage},{newCar.Availability},{newCar.Charge}";
                writer.WriteLine(line);
            }
        }
        void AppendPhotosToCSV(string photoCsvFilePath, string LicensePlate, List<string> photoFiles)
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
                writer.WriteLine($"{LicensePlate},{photoList}");
            }
        }
        // licence plate input
        string enterLicencePlate()
        {
            while (true)
            {
                string carPlateNo = promptLicencePlate();
                if (validateLicencePlate(carPlateNo))
                {
                    if (cars.Any(c => c.LicensePlate == carPlateNo))
                    {
                        Console.Write("This licence plate has already been registered. Type 'exit' to cancel or 'edit' to re-enter: ");
                        string response = Console.ReadLine().Trim().ToLower();
                        if (response == "exit")
                        {
                            Environment.Exit(0);
                        }
                        else if (response == "edit")
                        {
                            continue;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please type 'exit' or 'edit'.");
                        }
                    }
                    else
                    {
                        return carPlateNo;
                    }
                }
                else
                {
                    displayInvalidLicencePlate();
                }
            }
        }
        string promptLicencePlate()
        {
            Console.Write("Enter Car Plate Number: ");
            return Console.ReadLine();
        }
        bool validateLicencePlate(string licencePlate)
        {
            return !string.IsNullOrWhiteSpace(licencePlate);
        }
        void displayInvalidLicencePlate()
        {
            Console.WriteLine("Invalid input.Licence Plate cannot be empty.");
        }

        // car make input
        string enterCarMake()
        {
            while (true)
            {
                string carMake = promptCarMake();
                if (validateCarMake(carMake))
                {
                    return carMake;
                }
                else
                {
                    displayInvalidCarMake();
                }
            }
        }
        string promptCarMake()
        {
            Console.Write("Enter Car Make: ");
            return Console.ReadLine();
        }
        bool validateCarMake(string carMake)
        {
            return !string.IsNullOrWhiteSpace(carMake) && carMakes.Contains(carMake.ToLower());
        }
        void displayInvalidCarMake()
        {
            Console.WriteLine("Invalid input. Car Make cannot be empty or must be a valid car make.");
        }

        // car model input
        string enterCarModel()
        {
            while (true)
            {
                string carModel = promptCarModel();
                if (validateCarModel(carModel))
                {
                    return carModel;
                }
                else
                {
                    displayInvalidModel();
                }
            }
        }
        string promptCarModel()
        {
            Console.Write("Enter Car Model: ");
            return Console.ReadLine();
        }
        bool validateCarModel(string carModel)
        {
            return !string.IsNullOrWhiteSpace(carModel);
        }
        void displayInvalidModel()
        {
            Console.WriteLine("Invalid input. Car Model cannot be empty.");
        }


        // car year input
        string promptYear()
        {
            Console.Write("Enter Car Year: ");
            return Console.ReadLine();
        }
        int enterYear()
        {
            int year;
            while (true)
            {
                string yearInput = promptYear();
                if (int.TryParse(yearInput, out year) && validateYear(year))
                {
                    break;
                }
                displayInvalidYear();
            }
            return year;
        }
        bool validateYear(int year)
        {
            return year > 1980 && year <= DateTime.Now.Year;
        }
        void displayInvalidYear()
        {
            Console.WriteLine($"Invalid input. Car Year must be between 1980 and {DateTime.Now.Year}.");
        }

        // car mileage input    
        string promptMileage()
        {
            Console.Write("Enter Car Mileage (miles): ");
            return Console.ReadLine();
        }
        int enterMileage()
        {
            int mileage;
            while (true)
            {
                string mileageInput = promptMileage();
                if (int.TryParse(mileageInput, out mileage) && validateMileage(mileage))
                {
                    break;
                }
                displayInvalidMileage();
                promptMileage(); // Prompt again if the input is invalid
            }
            return mileage;
        }
        bool validateMileage(int mileage)
        {
            return mileage >= 0;
        }
        void displayInvalidMileage()
        {
            Console.WriteLine("Invalid input. Car Mileage must be a non-negative integer.");
        }

        // car photo input
        void promptCarPhoto()
        {
            Console.Write("Upload Photo (jpg/png/jpeg/pdf) or type 'done' to finish: ");
        }
        string uploadPhoto()
        {
            return Console.ReadLine().Trim().ToLower();
        }
        bool validatePhoto(string photoFile)
        {
            return IsValidFileType(photoFile);
        }
        void displayInvalidPhoto()
        {
            Console.WriteLine("Invalid file type. Please upload a file with a valid extension (jpg, png, jpeg, pdf).");
        }
        List<string> updatePhotoList(List<string> photoFiles, string photoFile)
        {
            photoFiles.Add(photoFile);
            return photoFiles;
        }
        void displayPhotoList(List<string> photoFiles)
        {
            Console.WriteLine("====Uploaded photos====");
            foreach (var file in photoFiles)
            {
                Console.WriteLine(file);
            }
        }

        // hourly charge input
        string promptCharge()
        {
            Console.Write("Enter Hourly Charge ($): ");
            return Console.ReadLine();
        }
        float enterCharge()
        {
            float charge;
            while (true)
            {
                string chargeInput = promptCharge();
                if (float.TryParse(chargeInput, NumberStyles.Float, CultureInfo.InvariantCulture, out charge) && validateCharge(charge))
                {
                    charge = (float)Math.Round(charge, 2); 
                    break;
                }
                displayInvalidCharge();
                promptCharge(); // Prompt again if the input is invalid
            }
            return charge;
        }
        bool validateCharge(float charge)
        {
            return charge >= 0;
        }
        void displayInvalidCharge()
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
        }

        // insurance company input 
        void displayInsuranceCompany()
        {
            Console.WriteLine();
            Console.WriteLine("====Available Insurance Companies====");
            foreach (var company in companyDictionary.Values)
            {
                Console.WriteLine($"Branch No: {company.BranchNo}, Company: {company.CompanyName}, Tel: {company.Telephone}, Address: {company.Address}, Email: {company.EmailAddress}");
            }
        }
        string promptInsuranceCompany()
        {
            Console.Write("Enter the Branch Number of your Insurance Company: ");
            return Console.ReadLine();
        }
        bool validateBranchNo(string branchInput, out int branchNo)
        {
            return int.TryParse(branchInput, out branchNo) && companyDictionary.ContainsKey(branchNo);
        }
        void displayInvalidBranchNo()
        {
            Console.WriteLine("Invalid Branch Number. Please enter a valid branch number from the list.");
        }
        int enterInsuranceCompany()
        {
            while (true)
            {
                string branchInput = promptInsuranceCompany();
                if (validateBranchNo(branchInput, out int branchNo))
                {
                    return branchNo;
                }
                else
                {
                    displayInvalidBranchNo();
                }

                if (companyDictionary.TryGetValue(branchNo, out var company))
                {
                    selectedCompany = new InsuranceCompany
                    {
                        BranchNo = branchNo,
                        CompanyName = company.CompanyName,
                        Telephone = company.Telephone,
                        Address = company.Address,
                        EmailAddress = company.EmailAddress
                    };
                }
                else
                {
                    Console.WriteLine("Invalid Branch Number. Please try again.");
                }
            }
        }

        // insurance details input
        string promptExpiryDate()
        {
            Console.Write("Enter Insurance Expiry Date (YYYY-MM-DD): ");
            return Console.ReadLine();
        }
        bool validateExpiryDate(string dateInput, out DateTime expiryDate)
        {
            return DateTime.TryParse(dateInput, out expiryDate) && expiryDate > DateTime.Now;
        }
        void displayInvalidDate()
        {
            Console.WriteLine("Invalid date. Please enter a future date.");
        }
        DateTime enterInsuranceExpiryDate()
        {
            DateTime expiryDate;
            while (true)
            {
                string dateInput = promptExpiryDate();
                if (validateExpiryDate(dateInput, out expiryDate))
                {
                    return expiryDate;
                }
                else
                {
                    displayInvalidDate();
                }
            }
        }

        // set availability 
        string setAvailability()
        {
            string availability = "Available";
            return availability;
        }

        // confirmation
        void displaySummary(string licencePlate, string carMake, string carModel, int year, int mileage,string availability, float charge)
        {
            Console.WriteLine();
            Console.WriteLine("========Car details summary========");
            Console.WriteLine($"Car Plate Number: {licencePlate}");
            Console.WriteLine($"Car Make: {carMake}");
            Console.WriteLine($"Car Model: {carModel}");
            Console.WriteLine($"Year: {year}");
            Console.WriteLine($"Car Mileage: {mileage}");
            Console.WriteLine($"Availability: {availability}");
            Console.WriteLine($"Hourly Charge: ${charge:F2}");
        }
        string promptConfirmation()
        {
            Console.WriteLine();
            Console.Write("Are you sure you want to register this car? (yes/no) ");
            return Console.ReadLine().Trim().ToLower();
        }
        void confirmRegistration(string licencePlate, string carMake, string carModel, int year, int mileage, string availability, float charge, DateTime expiryDate, List<string> photoFiles, int branchNo)
        {
            while (true)
            {
                string response = promptConfirmation();
                if (response == "yes")
                {
                    createRegistration(licencePlate, carMake, carModel, year, mileage, availability, charge, expiryDate, photoFiles, branchNo);
                    displayRegisteredCar();
                    break;
                }
                else
                {
                    displayRegistrationCancelled();
                    break;
                }
            }

        }

        void createRegistration(string licencePlate, string carMake, string carModel, int year, int mileage, string availability, float charge, DateTime expiryDate, List<string> photoFiles, int branchNo)
        {
            var newCar = new Car
            {
                CarOwnerId = carOwner.Id,
                LicensePlate = licencePlate,
                CarMake = carMake,
                Model = carModel,
                Year = year,
                Mileage = mileage,
                Availability = availability,
                Charge = charge,
            };
            cars.Add(newCar);
            AppendCarToCSV("Car_List.csv", newCar);
            AppendPhotosToCSV("Photo_List.csv", licencePlate, photoFiles);

            updateInsuranceList(newCar, branchNo, expiryDate);            
        }

        void updateInsuranceList(Car newCar, int branchNo, DateTime expiryDate)
        {
            var newInsurance = new Insurance
            {
                ExpiryDate = expiryDate,
                Car = newCar,
                Company = selectedCompany
            };
            insuranceList.Add(newInsurance);
            using (var writer = new StreamWriter("Insurance_List.csv", true))
            {
                string line = $"{branchNo},{newInsurance.Car.LicensePlate},{newInsurance.Car.CarOwnerId},{expiryDate:yyyy-MM-dd}";
                writer.WriteLine(line);
            }
        }

        void displayRegisteredCar()
        {
            Console.WriteLine("====Car Successfully Registered!====");
        }

        void displayRegistrationCancelled()
        {
            Console.WriteLine("====Car registration canceled.====");
        }


        void selectRegisterCar()
        {
            Console.WriteLine("========Please enter car details========");
            string licencePlate = enterLicencePlate();
            string carMake = enterCarMake();
            string carModel = enterCarModel();
            int year = enterYear();
            int mileage = enterMileage();

            // Upload photos
            List<string> photoFiles = new List<string>();
            string photoFile;
            Console.WriteLine("========Please upload images of the car!========");
            while (true)
            {
                if (photoFiles.Count >= 5)
                {
                    Console.WriteLine("You have reached the maximum number of photo uploads (5).");
                    break;
                }
                promptCarPhoto();
                photoFile = uploadPhoto();
                if (photoFile == "done")
                {
                    break;
                }
                if (validatePhoto(photoFile))
                {
                    updatePhotoList(photoFiles, photoFile);
                }
                else
                {
                    displayInvalidPhoto();
                }
            }
            displayPhotoList(photoFiles);

            // Set charge
            float charge = enterCharge();

            // Insurance details
            displayInsuranceCompany();
            int branchNo = enterInsuranceCompany();
            DateTime expiryDate = enterInsuranceExpiryDate();

            // Set availability
            string availability = setAvailability();

            // Display summary
            displaySummary(licencePlate, carMake, carModel, year, mileage, availability, charge);

            // Confirm registration
            confirmRegistration(licencePlate, carMake, carModel, year, mileage, availability, charge, expiryDate, photoFiles, branchNo);
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
                    if (renter.Bookings.All(booking => booking.Status != "Pending"))
                    {
                        BrowseCars();
                    }
                    else
                    {
                        Console.WriteLine("Please proceed to pay for current pending booking!");
                    }
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
                    PickUpCar();
                    break;
                case "6":
                    MakePayment();
                    break;
                case "7":
                    Console.WriteLine("Logging out...");
                    Console.WriteLine();
                    user = login();
                    displayUserDetails(user);
                    break;
                case "0":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

        static bool IsBookingTimeValid(string startDateTime, string endDateTime, Car selectedCar)
        {
            DateTime startDate, endDate;

            if (!DateTime.TryParseExact(startDateTime, "yyyy-MM-dd hh:mm tt", null, DateTimeStyles.None, out startDate) ||
                !DateTime.TryParseExact(endDateTime, "yyyy-MM-dd hh:mm tt", null, DateTimeStyles.None, out endDate))
            {
                return false;
            }

            if (startDate >= endDate)
            {
                return false;
            }

            foreach (var date in selectedCar.UnavailableDates)
            {
                DateTime unavailableDate;
                if (DateTime.TryParseExact(date, "yyyy-MM-dd hh:mm tt", null, DateTimeStyles.None, out unavailableDate))
                {
                    if (unavailableDate >= startDate && unavailableDate < endDate)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        bool HasConsecutiveDates(List<string> availableDates)
        {
            if (availableDates.Count < 2)
            {
                return false;
            }
            availableDates.Sort();

            DateTime prevDate = DateTime.Parse(availableDates[0]);
            for (int i = 1; i < availableDates.Count; i++)
            {
                DateTime currentDate = DateTime.Parse(availableDates[i]);
                if ((currentDate - prevDate).TotalDays >= 1)
                {
                    return true;
                }
                prevDate = currentDate;
            }

            return false;
        }

        void BrowseCars()
        {
            Console.WriteLine();
            Console.WriteLine("List of Cars to Rent:");
            foreach (var car in cars)
            {
                Console.WriteLine($"{"License Plate:",-14} {car.LicensePlate,-9} {"Make:",-5} {car.CarMake,-15} {"Model:",-6} {car.Model,-9} {"Year:",-5} {car.Year,-6} {"Mileage:",-8} {car.Mileage,-14} {"Availability:",-13} {car.Availability} {"Charge per hour:",-16} ${car.Charge}");
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
                else
                {
                    var availableDates = selectedCar.AvailableDates.Except(selectedCar.UnavailableDates).ToList();

                    if (!HasConsecutiveDates(availableDates))
                    {
                        Console.WriteLine("No valid booking dates available. Please select another car.");
                        selectedCar = null;
                    }
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
            MakeBooking(selectedCar);
        }

        // Charlotte Lee S10258027K
        PickUpMethod initialisePickUpMethod(string startDateTime, PickUpMethod pickUpMethod, int locationIndex)
        {
            Pickup selfpickup = new Pickup
            {
                DateTimePickup = DateTime.ParseExact(startDateTime, "yyyy-MM-dd hh:mm tt", null)
            };

            selfpickup.PickupLocation = locations[locationIndex - 1];
            pickUpMethod = selfpickup;
            return pickUpMethod;
        }

        //Charlotte Lee S10258027K
        ReturnMethod initialiseReturnMethod(string endDateTime, ReturnMethod returnMethod, int locationIndex)
        {
            SelfReturn selfReturn = new SelfReturn
            {
                DateTimeReturn = DateTime.ParseExact(endDateTime, "yyyy-MM-dd hh:mm tt", null)
            };

            selfReturn.ICarReturnLocation = locations[locationIndex - 1];
            returnMethod = selfReturn;
            return selfReturn;
        }

        //Charlotte Lee S10258027K
        (DeliverCar, double) initialiseDeliveryPickUp(string startDateTime, DeliverCar pickUpMethod, string deliveryLocation, double deliveryFee)
        {
            DeliverCar delivery = new DeliverCar
            {
                DateTimeDeliver = DateTime.ParseExact(startDateTime, "yyyy-MM-dd hh:mm tt", null)
            };
            delivery.DeliveryLocation = deliveryLocation;
            pickUpMethod = delivery;
            deliveryFee += 10;
            return (pickUpMethod, deliveryFee);
        }

        //Charlotte Lee S10258027K
        (DeliveryReturn, double) initialiseDeliveryReturn(string endDateTime, DeliveryReturn returnMethod, string returnLocation, double deliveryFee)
        {
            DeliveryReturn deliveryReturn = new DeliveryReturn
            {
                DateTimeReturnDelivery = DateTime.ParseExact(endDateTime, "yyyy-MM-dd hh:mm tt", null),
                AdditionalCharge = new AdditionalCharge()
            };
            deliveryReturn.ReturnLocation = returnLocation;
            returnMethod = deliveryReturn;
            deliveryFee += 10;
            return (returnMethod, deliveryFee);
        }

        //Charlotte Lee S10258027K
        (DateTime, DateTime, TimeSpan, double, double, double) getBookingDetails(string startDateTime, string endDateTime, Car selectedCar, double deliveryFee) {
            DateTime startTime = DateTime.ParseExact(startDateTime, "yyyy-MM-dd hh:mm tt", null);
            DateTime endTime = DateTime.ParseExact(endDateTime, "yyyy-MM-dd hh:mm tt", null);
            TimeSpan duration = endTime - startTime;
            double totalHours = (double)duration.TotalHours;
            double totalCharge = totalHours * (double)selectedCar.Charge;
            double totalDeliveryFee = deliveryFee;

            return (startTime, endTime, duration, totalHours, totalCharge, totalDeliveryFee);
        }

        //Charlotte Lee S10258027K
        void displayBookingDetails(Car selectedCar, PickUpMethod pickUpMethod, ReturnMethod returnMethod, DateTime startDateTime, DateTime endDateTime, TimeSpan duration, double totalHours, double totalCharge, double totalDeliveryFee)
        {
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
        }

        //Charlotte Lee S10258027K
        void displaySelectedCar(Car selectedCar)
        {
            Console.WriteLine();
            Console.WriteLine("Car Details:");
            Console.WriteLine($"{"License Plate:",-14} {selectedCar.LicensePlate,-9} {"Make:",-5} {selectedCar.CarMake,-15} {"Model:",-6} {selectedCar.Model,-9} {"Year:",-5} {selectedCar.Year,-6} {"Mileage:",-8} {selectedCar.Mileage,-14} {"Availability:",-13} {selectedCar.Availability} {"Charge per hour:",-16} ${selectedCar.Charge}");
        }

        //Charlotte Lee S10258027K
        void displayAvailableDates(List<string> availableDates)
        {
            Console.WriteLine();
            Console.WriteLine("Available Dates:");
            foreach (var date in availableDates)
            {
                Console.WriteLine(date);
            }
        }

        //Charlotte Lee S10258027K
        void displayBookingConfirm()
        {
            Console.WriteLine();
            Console.WriteLine("Booking Confirmed!");

            Console.WriteLine();
            Console.WriteLine("Continue to Payment");
        }

        //Charlotte Lee S10258027K
        void displayRedoBooking()
        {
            Console.WriteLine();
            Console.WriteLine("Redoing the booking...");
        }

        //Charlotte Lee S10258027K
        void displayExitBooking()
        {
            Console.WriteLine();
            Console.WriteLine("Booking Cancelled. Exiting.");
        }

        //Charlotte Lee S10258027K
        void displayRedoBookingInvalid()
        {
            Console.WriteLine();
            Console.WriteLine("Invalid choice. Please enter 'C', 'R', or 'E'.");
        }

        //Charlotte Lee S10258027K
        void displayMonthInvalid()
        {
            Console.WriteLine("Invalid month. Please enter a number between 1 and 12.");
            Console.WriteLine();
        }

        //Charlotte Lee S10258027K
        void displayInvalidFilterChoice()
        {
            Console.WriteLine("Invalid choice. Please enter 'Y' for yes or 'N' for no.");
        }

        //Charlotte Lee S10258027K
        void displayInvalidStartDateTime()
        {
            Console.WriteLine("Invalid start date and time or it is unavailable, or it is the last available date. Please try again.");
            Console.WriteLine();
        }

        //Charlotte Lee S10258027K
        void displayInvalidEndDateTime()
        {
            Console.WriteLine("Invalid end date and time or it is unavailable, or it is the first available date, or end date and time is before start date and time. Please try again.");
            Console.WriteLine();
        }

        //Charlotte Lee S10258027K
        void displayBookingValidMessage()
        {
            Console.WriteLine();
            Console.WriteLine("Booking date is valid.");
        }

        //Charlotte Lee S10258027K
        void displayBookingTimeInvalid()
        {
            Console.WriteLine();
            Console.WriteLine("Invalid booking range or it intersects with an unavailable date.");
            Console.WriteLine();
        }

        //Charlotte Lee S10258027K
        void promptFilterChoice()
        {
            Console.WriteLine();
            Console.WriteLine("Do you want to filter the available time slots by selecting a specific month? ‘Y’ or ‘N’");
        }

        //Charlotte Lee S10258027K
        string selectFilterChoice()
        {
            string filterChoice = Console.ReadLine().Trim().ToUpper();
            Console.WriteLine();
            return filterChoice;
        }

        //Charlotte Lee S10258027K
        void promptFilterMonth()
        {
            Console.WriteLine("Enter the month number 1-12 (January-December) you want to filter:");
        }

        //Charlotte Lee S10258027K
        List<string> FilteredDates(int month, List<string> availableDates)
        {
            return availableDates.Where(date => DateTime.TryParse(date, out DateTime dt) && dt.Month == month).ToList();
        }

        //Charlotte Lee S10258027K
        void displayFilteredDates(int month, List<string> filteredDates)
        {
            string monthName = new DateTime(1, month, 1).ToString("MMMM");
            Console.WriteLine();
            Console.WriteLine($"Available dates for month {month} ({monthName}):");
            foreach (var date in filteredDates)
            {
                Console.WriteLine(date);
            }
        }

        //Charlotte Lee S10258027K
        void promptStartDateTimeSlot()
        {
            Console.WriteLine("Please enter the start date and time slot for your booking (yyyy-MM-dd hh:mm tt): ");
        }

        bool validateStartDateTimeSlot(string startDateTime, List<string> availableDates, Car selectedCar)
        {
            if (!selectedCar.UnavailableDates.Contains(startDateTime) && availableDates.IndexOf(startDateTime) != availableDates.Count - 1 && availableDates.Contains(startDateTime))
            {
                return true;
            }
            return false;
        }

        //Charlotte Lee S10258027K
        void promptEndDateTimeSlot()
        {
            Console.WriteLine("Please enter the end date and time slot for your booking (yyyy-MM-dd hh:mm tt): ");
        }

        //Charlotte Lee S10258027K
        bool validateEndDateTimeSlot(string startDateTime, string endDateTime, List<string> availableDates, Car selectedCar)
        {
            DateTime startDate, endDate;

            if (DateTime.TryParseExact(startDateTime, "yyyy-MM-dd hh:mm tt", null, System.Globalization.DateTimeStyles.None, out startDate) &&
                DateTime.TryParseExact(endDateTime, "yyyy-MM-dd hh:mm tt", null, System.Globalization.DateTimeStyles.None, out endDate) && !selectedCar.UnavailableDates.Contains(endDateTime) && availableDates.IndexOf(endDateTime) != 0 && availableDates.Contains(endDateTime) && endDate > startDate)
            {
                return true;
            }
            return false;
        }

        //Charlotte Lee S10258027K
        void promptPickUpOption()
        {
            Console.WriteLine();
            Console.WriteLine("Do you want to pick up the car yourself or have it delivered? Enter 'P' for pickup or 'D' for delivery: ");
        }

        //Charlotte Lee S10258027K
        void displayiCarLocations()
        {
            Console.WriteLine();
            Console.WriteLine("List of Pickup Locations:");
            for (int i = 0; i < locations.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {locations[i]}");
            }
        }

        //Charlotte Lee S10258027K
        void promptiCarLocationChoice()
        {
            Console.WriteLine();
            Console.WriteLine("Enter the number of the location where you want to pick up the car: ");
        }

        //Charlotte Lee S10258027K
        void displayInvalidLocation()
        {
            Console.WriteLine("Invalid location number. Please try again.");
            Console.WriteLine();
        }

        //Charlotte Lee S10258027K
        void promptDeliveryLocation()
        {
            Console.WriteLine();
            Console.WriteLine("Enter the delivery location in this format: Postal Code, Address, Country: ");
        }

        //Charlotte Lee S10258027K
        string enterDeliveryLocation()
        {
            string deliveryLocation = Console.ReadLine();
            return deliveryLocation;
        }

        //Charlotte Lee S10258027K
        void displayInvalidPostalCode()
        {
            Console.WriteLine("Invalid postal code. It must be exactly 6 digits long and contain only numbers. Please try again.");
            Console.WriteLine();
        }

        //Charlotte Lee S10258027K
        void displayInvalidCountry()
        {
            Console.WriteLine("Invalid location. Ensure the country is 'Singapore' and the format is correct. Please try again.");
                                Console.WriteLine();
        }

        //Charlotte Lee S10258027K
        void displayInvalidPickupChoice()
        {
            Console.WriteLine("Invalid choice. Please enter 'P' for pickup or 'D' for delivery.");
            Console.WriteLine();
        }

        //Charlotte Lee S10258027K
        void promptReturnOption()
        {
            Console.WriteLine("Do you want to return the car yourself or have it picked up? Enter 'S' for self-return or 'D' for delivery return: ");
        }

        //Charlotte Lee S10258027K
        void promptiCarLocationChoiceReturn()
        {
            Console.WriteLine();
            Console.WriteLine("Enter the number of the location where you want to return the car: ");
        }

        //Charlotte Lee S10258027K
        void promptReturnDeliveryLocation()
        {
            Console.WriteLine();
            Console.WriteLine("Enter the return delivery location in this format: Postal Code, Address, Country: ");
        }

        //Charlotte Lee S10258027K
        void displayInvalidReturnChoice()
        {
            Console.WriteLine("Invalid choice. Please enter 'S' for self-return or 'D' for delivery return.");
            Console.WriteLine();
        }

        //Charlotte Lee S10258027K
        void promptConfirmBooking()
        {
            Console.WriteLine();
            Console.WriteLine("Would you like to: [C]onfirm the booking, [R]edo the booking, or [E]xit and cancel the booking?");
        }

        //Charlotte Lee S10258027K
        string enterConfirmBooking()
        {
            string choice = Console.ReadLine().ToUpper();
            return choice;
        }

        //Charlotte Lee S10258027K
        void MakeBooking(Car selectedCar)
        {
            bool redoBooking = true;
            while (redoBooking)
            {
                List<string> availableDates = selectedCar.getAvailableDates();
                List<string> originalAvailableDates = new List<string>(availableDates);
                List<string> originalUnavailableDates = new List<string>(selectedCar.getUnavailableDates());

                displaySelectedCar(selectedCar);

                displayAvailableDates(availableDates);

                bool filteringCompleted = false;
                while (!filteringCompleted)
                {
                    promptFilterChoice();
                    string filterChoice = selectFilterChoice();

                    if (filterChoice == "Y")
                    {
                        while (true)
                        {
                            promptFilterMonth();
                            if (int.TryParse(Console.ReadLine(), out int month) && month >= 1 && month <= 12)
                            {
                                var filteredDates = FilteredDates(month, availableDates);

                                displayFilteredDates(month, filteredDates);
                                break;
                            }
                            else
                            {
                                displayMonthInvalid();
                            }
                        }
                    }
                    else if (filterChoice == "N")
                    {
                        filteringCompleted = true;
                    }
                    else
                    {
                        displayInvalidFilterChoice();
                    }
                }

                string startDateTime = "";
                string endDateTime = "";

                while (true)
                {
                    bool validStart = false;
                    while (validStart == false)
                    {
                        promptStartDateTimeSlot();
                        startDateTime = Console.ReadLine();

                        validStart = validateStartDateTimeSlot(startDateTime, availableDates, selectedCar);
                        if (validStart == false)
                        {
                            displayInvalidStartDateTime();
                        }
                    }

                    bool validEnd = false;
                    while (validEnd == false)
                    {
                        promptEndDateTimeSlot();
                        endDateTime = Console.ReadLine();

                        validEnd = validateEndDateTimeSlot(startDateTime, endDateTime, availableDates, selectedCar);
                        if (validEnd == false)
                        {
                            displayInvalidEndDateTime();
                        }
                    }

                    if (IsBookingTimeValid(startDateTime, endDateTime, selectedCar))
                    {
                        displayBookingValidMessage();
                        break;
                    }
                    else
                    {
                        displayBookingTimeInvalid();
                    }
                }

                selectedCar.updateCarAvailability(startDateTime, endDateTime, availableDates);

                string pickupOrDelivery = "";
                PickUpMethod pickUpMethod = null;
                double deliveryFee = 0;

                while (pickupOrDelivery != "P" && pickupOrDelivery != "D")
                {
                    promptPickUpOption();
                    pickupOrDelivery = Console.ReadLine().Trim().ToUpper();

                    if (pickupOrDelivery == "P")
                    {
                        var locations = ReadLocationsFromCsv("iCar_Locations.csv");

                        displayiCarLocations();

                        while (true)
                        {
                            promptiCarLocationChoice();
                            if (int.TryParse(Console.ReadLine(), out int locationIndex) && locationIndex >= 1 && locationIndex <= locations.Count)
                            {
                                pickUpMethod = initialisePickUpMethod(startDateTime, pickUpMethod, locationIndex);
                                break;
                            }
                            else
                            {
                                displayInvalidLocation();
                            }
                        }

                    }
                    else if (pickupOrDelivery == "D")
                    {

                        while (true)
                        {
                            promptDeliveryLocation();
                            string deliveryLocation = enterDeliveryLocation();

                            string[] parts = deliveryLocation.Split(',');
                            if (parts.Length == 3 && parts[2].Trim().Equals("Singapore", StringComparison.OrdinalIgnoreCase))
                            {
                                string postalCode = parts[0].Trim();

                                if (postalCode.Length == 6 && postalCode.All(char.IsDigit))
                                {
                                    (pickUpMethod, deliveryFee) = initialiseDeliveryPickUp(startDateTime, (DeliverCar)pickUpMethod, deliveryLocation, deliveryFee);
                                    break;
                                }
                                else
                                {
                                    displayInvalidPostalCode();
                                }
                            }
                            else
                            {
                                displayInvalidCountry();
                            }
                        }
                    }
                    else
                    {
                        displayInvalidPickupChoice();
                    }
                }
                Console.WriteLine();

                string returnMethodChoice = "";
                ReturnMethod returnMethod = null;

                while (returnMethodChoice != "S" && returnMethodChoice != "D")
                {
                    promptReturnOption();
                    returnMethodChoice = Console.ReadLine().Trim().ToUpper();

                    if (returnMethodChoice == "S")
                    {
                        var locations = ReadLocationsFromCsv("iCar_Locations.csv");

                        displayiCarLocations();

                        while (true)
                        {
                            promptiCarLocationChoiceReturn();
                            if (int.TryParse(Console.ReadLine(), out int locationIndex) && locationIndex >= 1 && locationIndex <= locations.Count)
                            {
                                returnMethod = initialiseReturnMethod(endDateTime, returnMethod, locationIndex);
                                break;
                            }
                            else
                            {
                                displayInvalidLocation();
                            }
                        }
                    }
                    else if (returnMethodChoice == "D")
                    {

                        while (true)
                        {
                            promptReturnDeliveryLocation();
                            string returnLocation = Console.ReadLine();

                            string[] parts = returnLocation.Split(',');
                            if (parts.Length == 3 && parts[2].Trim().Equals("Singapore", StringComparison.OrdinalIgnoreCase))
                            {
                                string postalCode = parts[0].Trim();

                                if (postalCode.Length == 6 && postalCode.All(char.IsDigit))
                                {
                                    (returnMethod, deliveryFee) = initialiseDeliveryReturn(endDateTime, (DeliveryReturn)returnMethod, returnLocation, deliveryFee);
                                    break;
                                }
                                else
                                {
                                    displayInvalidPostalCode();
                                }
                            }
                            else
                            {
                                displayInvalidCountry();
                            }
                        }
                    }
                    else
                    {
                        displayInvalidReturnChoice();
                    }
                }

                (DateTime startTime, DateTime endTime, TimeSpan duration, double totalHours, double totalCharge, double totalDeliveryFee) = getBookingDetails(startDateTime, endDateTime, selectedCar, deliveryFee);

                displayBookingDetails(selectedCar, pickUpMethod, returnMethod, startTime, endTime, duration, totalHours, totalCharge, totalDeliveryFee);

                while (true)
                {
                    promptConfirmBooking();
                    string choice = enterConfirmBooking();

                    if (choice == "C")
                    {
                        redoBooking = false;

                        string status = "Pending";

                        Booking newBooking = Booking.CreateBooking(startTime, endTime, status, pickUpMethod, returnMethod, totalCharge, totalDeliveryFee, selectedCar, renter);

                        displayBookingConfirm();
                        MakePayment();
                        break;
                    }
                    else if (choice == "R")
                    {
                        displayRedoBooking();

                        redoBooking = true;

                        selectedCar.resetCarAvailability(originalAvailableDates, originalUnavailableDates);
                        break;
                    }
                    else if (choice == "E")
                    {
                        displayExitBooking();

                        redoBooking = false;

                        selectedCar.resetCarAvailability(originalAvailableDates, originalUnavailableDates);
                        return;
                    }
                    else
                    {
                        displayRedoBookingInvalid();
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

        // Sudarsanam Rithika (S10257149F)
        void MakePayment()
        {
            Booking booking = getUnpaidBooking();
            if (booking == null)
            {
                displayNoUnpaidBooking();
                return;
            }

            string status = getBookingStatus(booking);

            double totalFee = 0;
            if (status == "Pending")
            {
                double bookingFee = booking.getBookingFee();
                double deliveryFee = booking.getDeliveryFee();
                totalFee = bookingFee + deliveryFee;
            }
            else if (status == "Completed" || status == "Picked Up")
            {
                double penaltyFee = booking.getPenaltyFee();
                double damageFee = booking.getDamageFee();
                totalFee = penaltyFee + damageFee;
            }

            displayBooking(booking);
            promptProceedPayment();

            string res = proceedPayment();
            if (res != "yes")
            {
                Console.WriteLine("Payment Cancelled.");
                return;
            }

            string paymentStatus;
            do
            {
                promptPaymentMethod();

                PaymentMethod selectedPaymentMethod = null;
                double accBalance = 0;

                while (selectedPaymentMethod == null)
                {
                    string choice = choosePaymentMethod();

                    // validate method input
                    while (choice != "DIGITAL WALLET" && choice != "DEBIT CARD" && choice != "CREDIT CARD")
                    {
                        Console.WriteLine("Invalid input! Please try again.");
                        Console.WriteLine("1. Digital Wallet");
                        Console.WriteLine("2. Debit Card");
                        Console.WriteLine("3. Credit Card");
                        Console.Write("Select Payment Method: ");
                        choice = choosePaymentMethod();
                    }

                    switch (choice)
                    {
                        case "DIGITAL WALLET":
                            selectedPaymentMethod = validateDigitalWalletInfo();
                            break;

                        case "DEBIT CARD":
                            selectedPaymentMethod = validateDebitCardInfo();
                            break;

                        case "CREDIT CARD":
                            selectedPaymentMethod = validateCreditCardInfo();
                            break;
                    }

                    if (selectedPaymentMethod is DigitalWallet digitalWallet)
                    {
                        accBalance = digitalWallet.getDigitalWalletBalance();
                    }
                    else if (selectedPaymentMethod is DebitCard debitCard)
                    {
                        accBalance = debitCard.getAccountBalance();
                    }
                    else if (selectedPaymentMethod is CreditCard creditCard)
                    {
                        accBalance = creditCard.getCreditLimit();
                    }

                }

                if (accBalance < totalFee)
                {
                    displayInsufficientFunds();
                    paymentStatus = "failure";
                }
                else
                {
                    selectedPaymentMethod.DeductBalance(totalFee);
                    paymentStatus = "success";

                    if (status == "Pending")
                    {
                        booking.updateBookingStatus("Confirmed");
                    }
                    else if (status == "Completed")
                    {
                        booking.Status = "All Expenses Paid For";
                    }
                }
            } while (paymentStatus == "failure");

            string receipt = GenerateReceipt(renter);

            promptReceiptDeliveryMethod();

            string method = enterReceiptDeliveryMethod();

            while (method != "EMAIL" && method != "PHONE NUMBER")
            {
                Console.WriteLine("Invalid option! Please select another option!");
                promptReceiptDeliveryMethod();
                method = enterReceiptDeliveryMethod();
            }

            if (method == "EMAIL")
            {
                sendEmailReceipt(renter);
            }
            else if (method == "PHONE NUMBER")
            {
                sendSMSReceipt(renter);
            }
        }

        // Sudarsanam Rithika (S10257149F)
        string getBookingStatus(Booking booking)
        {
            return booking.Status;
        }

        // Sudarsanam Rithika (S10257149F)
        void promptProceedPayment()
        {
            Console.WriteLine("Confirm payment amount? (yes/no) ");
        }

        // Sudarsanam Rithika (S10257149F)
        string proceedPayment()
        {
            string res = Console.ReadLine();
            return res;
        }
        // Sudarsanam Rithika (S10257149F)
        Booking getUnpaidBooking()
        {
            Booking unpaidBooking = null;
            foreach (Booking booking in renter.Bookings)
            {
                if (booking.Status == "Pending" || booking.Status == "Picked Up" || booking.Status == "Completed")
                {
                    unpaidBooking = booking;
                }

                else continue;
            }
            return unpaidBooking;
        }
        // Isabelle Tan S10257093F
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

        // Isabelle Tan S10257093F
        void returnToiCarStation()
        {
            double totalReturnFee = 0;
            Booking booking = getOngoingBooking();
            if (booking != null)
            {
                ReturnMethod returnMethod = booking.getReturnMethod();
                if (returnMethod is SelfReturn selfReturn)
                {
                    DateTime retDateTime = getCurrentDateTime();
                    booking.setDateTimeReturn(retDateTime);
                    DateTime endDate = booking.getEndDate();
                    if (retDateTime > endDate)
                    {
                        double penaltyFee = calculatePenaltyFee(retDateTime, endDate, booking);
                        totalReturnFee += penaltyFee;
                        booking.updatePenaltyFee(penaltyFee);
                        displayPenaltyFee(penaltyFee);
                    }
                    string damage = promptCheckForDamages();
                    double damageFee = updateDamages(damage);
                    totalReturnFee += damageFee;
                    if (totalReturnFee > 0)
                    {
                        booking.updateTotalFees(totalReturnFee);
                        Console.WriteLine();
                        Console.WriteLine("Continue to Payment");
                        Console.WriteLine();
                        MakePayment();
                    }
                    else
                    {
                        displayNoOutstandingFees();
                    }
                    string status = booking.getBookingStatus();
                    if (status != "All Expenses Paid For")
                    {
                        status = "Completed";
                        booking.updateBookingStatus(status);
                    }
                    displayRentalCompleted();
                    return;
                }
                else
                {
                    displayIncorrectReturnMethod();
                    return;
                }
            }
            else
            {
                displayNoOngoingBookings();
                return;
            }

        }

        // Isabelle Tan S10257093F
        Booking getOngoingBooking()
        {
            Booking ongoingBooking = null;
            foreach (Booking booking in renter.Bookings)
            {
                if (booking.Status == "Picked Up")
                {
                    ongoingBooking = booking;
                }

                else continue;
            }
            return ongoingBooking;
        }

        DateTime getCurrentDateTime()
        {
            return DateTime.Now;
        }

        //calculate penalty fee - Isabelle Tan S10257093F
        double calculatePenaltyFee(DateTime retDateTime, DateTime endDate, Booking ongoingBooking)
        {
            double penaltyFee = 0;
            TimeSpan overTime = retDateTime - endDate;
            double totalFee = ongoingBooking.Payment.TotalFee; //get current total cost of booking
            penaltyFee = totalFee * 0.20 * overTime.Hours;
            penaltyFee = Math.Round(penaltyFee, 2);
            return penaltyFee;
        }
        void displayPenaltyFee(double penaltyFee)
        {
            string message = "Penalty Fee for late return: " + penaltyFee;
            Console.WriteLine(message);
        }
        //prompt check for damages - Isabelle Tan S10257093F
        string promptCheckForDamages()
        {
            Console.WriteLine("Please check for damages. If there are damages, enter 'Has Damages'. Else enter 'No Damages'.");
            string damages = Console.ReadLine().ToLower();
            while (damages != "has damages" && damages != "no damages")
            {
                Console.WriteLine("Invalid input. Try again.");
                Console.WriteLine("Please check for damages. If there are damages, enter 'Has Damages'. Else enter 'No Damages'.");
                damages = Console.ReadLine();
            }
            return damages;
        }
        // Isabelle Tan S10257093F
        double updateDamages(string damage)
        {
            double damageFee = 0;
            if (damage == "has damages")
            {
                damageFee = reportAccident();
            }
            return damageFee;

        }
        // Isabelle Tan S10257093F
        double reportAccident()
        {
            Booking booking = getOngoingBooking();
            booking.Payment.AdditionalCharge.DamageFee += 100;
            booking.Payment.TotalFee += 100;
            return 100;
        }
        // Isabelle Tan S10257093F
        void displayNoOutstandingFees()
        {
            Console.WriteLine("No outstanding fees.");
        }
        // Isabelle Tan S10257093F
        void displayRentalCompleted()
        {
            Console.WriteLine("Rental Completed");
        }
        // Isabelle Tan S10257093F
        void displayIncorrectReturnMethod()
        {
            Console.WriteLine("Wrong return method. Returning to main menu.");
        }
        // Isabelle Tan S10257093F
        void displayNoOngoingBookings()
        {
            Console.WriteLine("No ongoing bookings.");
        }

        Booking getPaidBooking()
        {
            Booking ongoingBooking = null;
            foreach (Booking booking in renter.Bookings)
            {
                if (booking.Status == "Confirmed")
                {
                    ongoingBooking = booking;
                }

                else continue;
            }
            return ongoingBooking;
        }
        //pickup car
        void PickUpCar()
        {
            Booking booking = getPaidBooking();

            booking.Status = "Picked Up";
            Console.WriteLine("Pickup confirmed.");
        }
    }
}
else
{
    Console.WriteLine("Too many failed attempts. System will exit.");
}

// display 
void displayRenterMainMenu()
{
    Console.WriteLine();
    Console.WriteLine("1. Book a Car");
    Console.WriteLine("2. View Booking History");
    Console.WriteLine("3. View Payment History");
    Console.WriteLine("4. Return Car");
    Console.WriteLine("5. Pick up car");
    Console.WriteLine("6. Make Payment");
    Console.WriteLine("7. Logout");
    Console.WriteLine("0. Exit");
    Console.Write("Choose an option:");
}

void displayCarOwnerMainMenu()
{
    Console.WriteLine();
    Console.WriteLine("========Menu========");
    Console.WriteLine("1. View Cars Owned");
    Console.WriteLine("2. Register Car");
    Console.WriteLine("3. Logout");
    Console.WriteLine("4. Exit");
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
        CarOwner carOwner = ((CarOwner)user);
        Console.WriteLine($"Licence: {carOwner.License}");
    }
}

// Sudarsanam Rithika (S10257149F)
void displayInsufficientFunds()
{
    Console.WriteLine("Balance insufficient! Please choose another payment method!");
}

// Sudarsanam Rithika (S10257149F)
void displayNoUnpaidBooking()
{
    Console.WriteLine("No ongoing booking or outstanding fees!");
}

// Sudarsanam Rithika (S10257149F)
void promptPaymentMethod()
{
    Console.WriteLine("Proceed with payment");
    Console.WriteLine();

    Console.WriteLine("1. Digital Wallet");
    Console.WriteLine("2. Debit Card");
    Console.WriteLine("3. Credit Card");
    Console.Write("Select Payment Method: ");
}

// Sudarsanam Rithika (S10257149F)
string choosePaymentMethod()
{
    string method = Console.ReadLine();
    return method.ToUpper();
}

// Sudarsanam Rithika (S10257149F)
DigitalWallet validateDigitalWalletInfo()
{
    Console.Write("Enter wallet type: ");
    string walletType = Console.ReadLine().ToUpper();
    var digitalWallet = paymentMethods
        .OfType<DigitalWallet>()
        .FirstOrDefault(dw => dw.Type.ToUpper() == walletType);

    if (digitalWallet == null)
    {
        return null;
    }

    string cardName;
    do
    {
        Console.Write("Enter card name: ");
        cardName = Console.ReadLine();
        if (digitalWallet.Name != cardName)
        {
            Console.WriteLine("Card name does not match user's name. Please try again.");
        }
    } while (digitalWallet.Name != cardName);

    return digitalWallet;
}

// Sudarsanam Rithika (S10257149F)
DebitCard validateDebitCardInfo()
{
    string cardNum;
    do
    {
        Console.Write("Enter card number: ");
        cardNum = Console.ReadLine();
        if (cardNum.Length != 16)
        {
            Console.WriteLine("Invalid card number! It must be 16 digits.");
        }
    } while (cardNum.Length != 16);

    var debitCard = paymentMethods
        .OfType<DebitCard>()
        .FirstOrDefault(dc => dc.CardNum == cardNum);

    if (debitCard == null)
    {
        return null;
    }

    string cardName;
    do
    {
        Console.Write("Enter card name: ");
        cardName = Console.ReadLine();
        if (debitCard.CardName != cardName)
        {
            Console.WriteLine("Card name does not match user's name. Please try again.");
        }
    } while (debitCard.CardName != cardName);

    return debitCard;
}

// Sudarsanam Rithika (S10257149F)
CreditCard validateCreditCardInfo()
{
    string cardNum;
    do
    {
        Console.Write("Enter card number: ");
        cardNum = Console.ReadLine();
        if (cardNum.Length != 16 || !long.TryParse(cardNum, out _))
        {
            Console.WriteLine("Invalid card number! It must be 16 digits.");
        }
    } while (cardNum.Length != 16 || !long.TryParse(cardNum, out _));

    var creditCard = paymentMethods
        .OfType<CreditCard>()
        .FirstOrDefault(cc => cc.CardNum == cardNum);

    if (creditCard == null)
    {
        return null;
    }

    string cardName;
    do
    {
        Console.Write("Enter card name: ");
        cardName = Console.ReadLine();
        if (creditCard.CardName != cardName)
        {
            Console.WriteLine("Card name does not match user's name. Please try again.");
        }
    } while (creditCard.CardName != cardName);

    return creditCard;
}

// Sudarsanam Rithika (S10257149F)
void promptReceiptDeliveryMethod()
{
    Console.WriteLine("How would you like receipt to be sent?");
    Console.WriteLine("1. Email");
    Console.WriteLine("2. Phone number");
}

// Sudarsanam Rithika (S10257149F)
string enterReceiptDeliveryMethod()
{
    Console.Write("Enter delivery method: ");
    string res = Console.ReadLine();
    Console.WriteLine();

    return res.ToUpper();
}

// Sudarsanam Rithika (S10257149F)
string GenerateReceipt(Renter user)
{
    Console.WriteLine("Receipt generated for user: " + user.Email);
    return "Receipt generated"; 
}

// Sudarsanam Rithika (S10257149F)
void sendEmailReceipt(Renter user)
{
    Console.WriteLine("Receipt sent via email to " + user.Email);
    DisplayReceiptConfirmation(true);
}

// Sudarsanam Rithika (S10257149F)
void sendSMSReceipt(Renter user)
{
    Console.WriteLine("Receipt sent via SMS to " + user.ContactNum);
    DisplayReceiptConfirmation(true);
}

// Sudarsanam Rithika (S10257149F)
void DisplayReceiptConfirmation(bool success)
{
    if (success)
    {
        Console.WriteLine("Receipt delivery confirmed.");
    }
    else
    {
        Console.WriteLine("Invalid receipt delivery method.");
    }
}

// Sudarsanam Rithika (S10257149F)
// display current booking details
void displayBooking(Booking currentBooking)
{
    DateTime startTime = currentBooking.StartDate;
    DateTime endTime = currentBooking.EndDate;

    TimeSpan duration = endTime - startTime;
    double totalHours = (double)duration.TotalHours;
    double totalCharge = totalHours * (double)currentBooking.Car.Charge;
    double totalDeliveryFee = currentBooking.Payment.AdditionalCharge.DeliveryFee;

    Console.WriteLine("Booking Details:");
    Console.WriteLine($"Car License Plate: {currentBooking.Car.LicensePlate}");
    Console.WriteLine($"Booking Start Date and Time: {startTime}");
    Console.WriteLine($"Booking End Date and Time: {endTime}");
    Console.WriteLine();

    if (currentBooking.Status == "Pending")
    {
        Console.WriteLine($"Total Booking Charge: {totalCharge:C}");
        Console.WriteLine($"Total Delivery Fee: {totalDeliveryFee:C}");
        Console.WriteLine($"Final Total: {(totalCharge + totalDeliveryFee):C}");
    }
    else
    {
        double additionalcharge = currentBooking.Payment.AdditionalCharge.PenaltyFee + currentBooking.Payment.AdditionalCharge.DamageFee;
        Console.WriteLine("Fees owed: " + additionalcharge);
    }
};

//return from desired location [empty]
void returnFromDesiredLocation() { }
