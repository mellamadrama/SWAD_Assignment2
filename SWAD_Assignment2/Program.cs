// See https://aka.ms/new-console-template for more information

using SWAD_Assignment2;
using System.Collections.Specialized;
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
            var car = new Car
            {
                CarOwnerId = carOwnerId,
                LicensePlate = carPlateNo
            };

            var company = new Insurance_Company
            {
                BranchNo = branchNo,
                CompanyName = companyName,
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
            insurance = insuranceList.FirstOrDefault(i => i.Car.LicensePlate == licensePlate && i.Car.CarOwnerId == ownerId);
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
                    RegisterCar(cars, insuranceList, carMakes, carOwner);
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
        static void ViewCars(List<Car> cars, Car_Owner carOwner)
        {
            var ownerCars = cars.Where(c => c.CarOwnerId == carOwner.Id).ToList();
            Console.WriteLine();
            Console.WriteLine("====Cars Owned====");
            foreach (var car in ownerCars)
            {
                Console.WriteLine($"{"License Plate:",-14} {car.LicensePlate,-9} {"Make:",-5} {car.CarMake,-15} {"Model:",-6} {car.Model,-9} {"Year:",-5} {car.Year,-6} {"Mileage:",-8} {car.Mileage} {"Availability:",-15} {car.Availability} {"Insurance Status:",-5} {car.InsuranceStatus}");
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

        static void RegisterCar(List<Car> cars, List<Insurance> insuranceList, List<string> carMakes, Car_Owner carOwner)
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
                            return;
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
                if (int.TryParse(yearInput, out year) && year > 1980 && year <= DateTime.Now.Year)
                {
                    break;
                }
                Console.WriteLine($"Invalid input. Car Year must be between 1980 and {DateTime.Now.Year}.");
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
            while (true)
            {
                Console.Write("Enter Hourly Charge ($): ");
                string Charge = Console.ReadLine();

                if (float.TryParse(Charge, NumberStyles.Float, CultureInfo.InvariantCulture, out charge) && charge >= 0)
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
            string insuranceStatus = insuranceList.Any(i => i.Car.LicensePlate == carPlateNo) ? "Y" : "X";

            Console.WriteLine();
            Console.WriteLine("========Car details summary========");
            Console.WriteLine($"Car Plate Number: {carPlateNo}");
            Console.WriteLine($"Car Make: {carMake}");
            Console.WriteLine($"Car Model: {carModel}");
            Console.WriteLine($"Year: {year}");
            Console.WriteLine($"Car Mileage: {carMileage}");
            Console.WriteLine($"Availability: {availability}");
            Console.WriteLine($"Hourly Charge: ${charge:F2}");
            if (insuranceStatus == "Y")
            {
                Console.WriteLine($"Insurance Status: {insuranceStatus}");
                var insurance = insuranceList.FirstOrDefault(i => i.Car.LicensePlate == carPlateNo);
                if (insurance != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("====Insurance Company Details====");
                    Console.WriteLine($"Insurance Company: {insurance.Company.CompanyName}");
                    Console.WriteLine($"Branch Number: {insurance.Company.BranchNo}");
                    Console.WriteLine($"Expiry Date: {insurance.ExpiryDate.ToShortDateString()}");
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("====Insurance Company Details====");
                    Console.WriteLine("No insurance details found for this car plate number.");
                }
            }
            else
            {
                Console.WriteLine($"Insurance Status: {insuranceStatus}");
                Console.WriteLine();
                Console.WriteLine("====Insurance Company Details====");
                Console.WriteLine("No insurance details found for this car plate number.");
            }

            Console.WriteLine();
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
                        CarOwnerId = carOwner.Id,
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

        (DateTime, DateTime, TimeSpan, double, double, double) getBookingDetails(string startDateTime, string endDateTime, Car selectedCar, double deliveryFee) {
            DateTime startTime = DateTime.ParseExact(startDateTime, "yyyy-MM-dd hh:mm tt", null);
            DateTime endTime = DateTime.ParseExact(endDateTime, "yyyy-MM-dd hh:mm tt", null);
            TimeSpan duration = endTime - startTime;
            double totalHours = (double)duration.TotalHours;
            double totalCharge = totalHours * (double)selectedCar.Charge;
            double totalDeliveryFee = deliveryFee;

            return (startTime, endTime, duration, totalHours, totalCharge, totalDeliveryFee);
        }

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

        void displaySelectedCar(Car selectedCar)
        {
            Console.WriteLine();
            Console.WriteLine("Car Details:");
            Console.WriteLine($"{"License Plate:",-14} {selectedCar.LicensePlate,-9} {"Make:",-5} {selectedCar.CarMake,-15} {"Model:",-6} {selectedCar.Model,-9} {"Year:",-5} {selectedCar.Year,-6} {"Mileage:",-8} {selectedCar.Mileage,-14} {"Availability:",-13} {selectedCar.Availability} {"Charge per hour:",-16} ${selectedCar.Charge}");
        }

        void displayBookingConfirm()
        {
            Console.WriteLine();
            Console.WriteLine("Booking Confirmed!");

            Console.WriteLine();
            Console.WriteLine("Continue to Payment");
        }

        void displayRedoBooking()
        {
            Console.WriteLine();
            Console.WriteLine("Redoing the booking...");
        }

        void displayExitBooking()
        {
            Console.WriteLine();
            Console.WriteLine("Booking Cancelled. Exiting.");
        }

        void displayRedoBookingInvalid()
        {
            Console.WriteLine();
            Console.WriteLine("Invalid choice. Please enter 'C', 'R', or 'E'.");
        }

        void displayMonthInvalid()
        {
            Console.WriteLine("Invalid month. Please enter a number between 1 and 12.");
            Console.WriteLine();
        }

        void displayInvalidFilterChoice()
        {
            Console.WriteLine("Invalid choice. Please enter 'Y' for yes or 'N' for no.");
        }

        void displayInvalidStartDateTime()
        {
            Console.WriteLine("Invalid start date and time or it is unavailable, or it is the last available date. Please try again.");
            Console.WriteLine();
        }

        void displayInvalidEndDateTime()
        {
            Console.WriteLine("Invalid end date and time or it is unavailable, or it is the first available date, or end date and time is before start date and time. Please try again.");
            Console.WriteLine();
        }

        void displayBookingTimeValid()
        {
            Console.WriteLine();
            Console.WriteLine("Booking date is valid.");
        }

        void displayBookingTimeInvalid()
        {
            Console.WriteLine();
            Console.WriteLine("Invalid booking range or it intersects with an unavailable date.");
            Console.WriteLine();
        }

        void MakeBooking(Car selectedCar)
        {
            bool redoBooking = true;
            while (redoBooking)
            {
                var availableDates = selectedCar.AvailableDates.Except(selectedCar.UnavailableDates).ToList();
                var originalAvailableDates = new List<string>(availableDates);
                var originalUnavailableDates = new List<string>(selectedCar.UnavailableDates);

                displaySelectedCar(selectedCar);

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
                        while (true)
                        {
                            Console.WriteLine("Enter the month number 1-12 (January-December) you want to filter:");
                            if (int.TryParse(Console.ReadLine(), out int month) && month >= 1 && month <= 12)
                            {
                                var filteredDates = availableDates.Where(date => DateTime.TryParse(date, out DateTime dt) && dt.Month == month).ToList();

                                Console.WriteLine($"Available dates for month {month}:");
                                foreach (var date in filteredDates)
                                {
                                    Console.WriteLine(date);
                                }
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
                    while (true)
                    {
                        Console.WriteLine("Please enter the start date and time slot for your booking (yyyy-MM-dd hh:mm tt): ");
                        startDateTime = Console.ReadLine();

                        if (!selectedCar.UnavailableDates.Contains(startDateTime) && availableDates.IndexOf(startDateTime) != availableDates.Count - 1 && availableDates.Contains(startDateTime))
                        {
                            break;
                        }
                        else
                        {
                            displayInvalidStartDateTime();
                        }
                    }

                    while (true)
                    {
                        Console.WriteLine("Please enter the end date and time slot for your booking (yyyy-MM-dd hh:mm tt): ");
                        endDateTime = Console.ReadLine();

                        DateTime startDate = DateTime.ParseExact(startDateTime, "yyyy-MM-dd hh:mm tt", null);
                        DateTime endDate = DateTime.ParseExact(endDateTime, "yyyy-MM-dd hh:mm tt", null);

                        if (!selectedCar.UnavailableDates.Contains(endDateTime) && availableDates.IndexOf(endDateTime) != 0 && availableDates.Contains(endDateTime) && endDate > startDate)
                        {
                            break;
                        }
                        else
                        {
                            displayInvalidEndDateTime();
                        }
                    }

                    if (IsBookingTimeValid(startDateTime, endDateTime, selectedCar))
                    {
                        displayBookingTimeValid();
                        break;
                    }
                    else
                    {
                        displayBookingTimeInvalid();
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

                string pickupOrDelivery = "";
                PickUpMethod pickUpMethod = null;
                double deliveryFee = 0;

                while (pickupOrDelivery != "P" && pickupOrDelivery != "D")
                { 
                    Console.WriteLine();
                    Console.WriteLine("Do you want to pick up the car yourself or have it delivered? Enter 'P' for pickup or 'D' for delivery: ");
                    pickupOrDelivery = Console.ReadLine().Trim().ToUpper();

                    if (pickupOrDelivery == "P")
                    {
                        var locations = ReadLocationsFromCsv("iCar_Locations.csv");

                        Console.WriteLine();
                        Console.WriteLine("List of Pickup Locations:");
                        for (int i = 0; i < locations.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {locations[i]}");
                        }

                        while (true)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Enter the number of the location where you want to pick up the car: ");
                            if (int.TryParse(Console.ReadLine(), out int locationIndex) && locationIndex >= 1 && locationIndex <= locations.Count)
                            {
                                pickUpMethod = initialisePickUpMethod(startDateTime, pickUpMethod, locationIndex);
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid location number. Please try again.");
                                Console.WriteLine();
                            }
                        }

                    }
                    else if (pickupOrDelivery == "D")
                    {

                        while (true)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Enter the delivery location in this format: Postal Code, Address, Country: ");
                            string deliveryLocation = Console.ReadLine();

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
                                    Console.WriteLine("Invalid postal code. It must be exactly 6 digits long and contain only numbers. Please try again.");
                                    Console.WriteLine();
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid location. Ensure the country is 'Singapore' and the format is correct. Please try again.");
                                Console.WriteLine();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter 'P' for pickup or 'D' for delivery.");
                        Console.WriteLine();
                    }
                }
                Console.WriteLine();

                string returnMethodChoice = "";
                ReturnMethod returnMethod = null;

                while (returnMethodChoice != "S" && returnMethodChoice != "D")
                {
                    Console.WriteLine("Do you want to return the car yourself or have it picked up? Enter 'S' for self-return or 'D' for delivery return: ");
                    returnMethodChoice = Console.ReadLine().Trim().ToUpper();

                    if (returnMethodChoice == "S")
                    {

                        Console.WriteLine("Please select a return location:");

                        var locations = ReadLocationsFromCsv("iCar_Locations.csv");

                        for (int i = 0; i < locations.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {locations[i]}");
                        }

                        while (true)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Enter the number of the location where you want to return the car: ");
                            if (int.TryParse(Console.ReadLine(), out int locationIndex) && locationIndex >= 1 && locationIndex <= locations.Count)
                            {
                                returnMethod = initialiseReturnMethod(endDateTime, returnMethod, locationIndex);
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid location number. Please try again.");
                                Console.WriteLine();
                            }
                        }
                    }
                    else if (returnMethodChoice == "D")
                    {

                        while (true)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Enter the return delivery location in this format: Postal Code, Address, Country: ");
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
                                    Console.WriteLine("Invalid postal code. It must be exactly 6 digits long and contain only numbers. Please try again.");
                                    Console.WriteLine();
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid location. Ensure the country is 'Singapore' and the format is correct. Please try again.");
                                Console.WriteLine();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter 'S' for self-return or 'D' for delivery return.");
                        Console.WriteLine();
                    }
                }

                (DateTime startTime, DateTime endTime, TimeSpan duration, double totalHours, double totalCharge, double totalDeliveryFee) = getBookingDetails(startDateTime, endDateTime, selectedCar, deliveryFee);

                displayBookingDetails(selectedCar, pickUpMethod, returnMethod, startTime, endTime, duration, totalHours, totalCharge, totalDeliveryFee);

                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine("Would you like to: [C]onfirm the booking, [R]edo the booking, or [E]xit and cancel the booking?");
                    string choice = Console.ReadLine().ToUpper();

                    if (choice == "C")
                    {
                        redoBooking = false;

                        AdditionalCharge additionalCharge = new AdditionalCharge(0, 0, totalDeliveryFee);
                        Booking booking = new Booking
                        {
                            BookingId = Guid.NewGuid().ToString(),
                            StartDate = startTime,
                            EndDate = endTime,
                            Status = "Pending",
                            PickUpMethod = pickUpMethod,
                            ReturnMethod = returnMethod,
                            Payment = new Payment(DateTime.Now, totalCharge, additionalCharge),
                            Car = selectedCar
                        };

                        renter.Bookings.Add(booking);

                        displayBookingConfirm();
                        MakePayment();
                        break;
                    }
                    else if (choice == "R")
                    {
                        displayRedoBooking();

                        redoBooking = true;

                        selectedCar.AvailableDates = new List<string>(originalAvailableDates);
                        selectedCar.UnavailableDates = new List<string>(originalUnavailableDates);
                        break;
                    }
                    else if (choice == "E")
                    {
                        displayExitBooking();

                        redoBooking = false;

                        selectedCar.AvailableDates = new List<string>(originalAvailableDates);
                        selectedCar.UnavailableDates = new List<string>(originalUnavailableDates);
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

        // make payment 
        void MakePayment()
        {
            Booking booking = getUnpaidBooking();
            if (booking != null)
            {
                string status = getBookingStatus(booking);

                displayBooking(booking);
                string name = user.FullName;

                promptProceedPayment();

                string res = proceedPayment(); 

                if (res != "yes")
                {
                    Console.WriteLine("Payment Cancelled.");
                    return;
                }

                Console.WriteLine("Proceed with payment");
                Console.WriteLine();

                promptPaymentMethod();

                string method = choosePaymentMethod();

                (PaymentMethod selectedPaymentMethod, double accBalance) = validatePaymentMethod();

                if (status == "Pending")
                {
                    while (accBalance < booking.Payment.TotalFee + booking.Payment.AdditionalCharge.DeliveryFee)
                    {
                        Console.WriteLine("Balance insufficient! Please choose another payment method!");
                        (selectedPaymentMethod, accBalance) = validatePaymentMethod();
                    }

                    selectedPaymentMethod.DeductBalance(booking.Payment.TotalFee);

                    status = "Confirmed";
                    booking.updateBookingStatus(status);
                }
                else if (status == "Picked Up" || booking.Status == "Completed")
                {
                    while (accBalance < booking.Payment.AdditionalCharge.PenaltyFee + booking.Payment.AdditionalCharge.DamageFee)
                    {
                        Console.WriteLine("Balance insufficient! Please choose another payment method!");
                        (selectedPaymentMethod, accBalance) = validatePaymentMethod();
                    }
                    double additionalCharge = booking.Payment.AdditionalCharge.PenaltyFee + booking.Payment.AdditionalCharge.DamageFee;

                    selectedPaymentMethod.DeductBalance(additionalCharge);
                    booking.Status = "All Expenses Paid For";
                }

                sendReceipt((Renter)user);
            }
            else
            {
                Console.WriteLine("No ongoing bookings that require payment");
                
            }
            
        }

        string getBookingStatus(Booking booking)
        {
            return booking.Status;
        }

        void promptProceedPayment()
        {
            Console.WriteLine("Confirm payment amount? (yes/no) ");
        }

        string proceedPayment()
        {
            string res = Console.ReadLine();
            return res;
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

        //calculate penalty fee
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
        //prompt check for damages
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
        double updateDamages(string damage)
        {
            double damageFee = 0;
            if (damage == "has damages")
            {
                damageFee = reportAccident();
            }
            return damageFee;

        }

        double reportAccident()
        {
            Booking booking = getOngoingBooking();
            booking.Payment.AdditionalCharge.DamageFee += 100;
            booking.Payment.TotalFee += 100;
            return 100;
        }
        void displayNoOutstandingFees()
        {
            Console.WriteLine("No outstanding fees.");
        }
        void displayRentalCompleted()
        {
            Console.WriteLine("Rental Completed");
        }
        void displayIncorrectReturnMethod()
        {
            Console.WriteLine("Wrong return method. Returning to main menu.");
        }
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
        Car_Owner carOwner = ((Car_Owner)user);
        Console.WriteLine($"Licence: {carOwner.License}");
    }
}

void promptPaymentMethod()
{
    Console.WriteLine("Proceed with payment");
    Console.WriteLine();

    Console.WriteLine("1. Digital Wallet");
    Console.WriteLine("2. Debit Card");
    Console.WriteLine("3. Credit Card");
    Console.Write("Select Payment Method: ");
}

string choosePaymentMethod()
{
    string method = Console.ReadLine();
    return method;
}

// validate payment method exists
(PaymentMethod, double) validatePaymentMethod()
{
    PaymentMethod selectedPaymentMethod = null;

    while (selectedPaymentMethod == null)
    {
        Console.WriteLine("Proceed with payment");
        Console.WriteLine();

        promptPaymentMethod();

        string method = choosePaymentMethod();

        while (method != "1" && method != "2" && method != "3")
        {
            Console.WriteLine("Invalid input! Please try again.");

            Console.WriteLine();

            Console.WriteLine("1. Digital Wallet");
            Console.WriteLine("2. Debit Card");
            Console.WriteLine("3. Credit Card");
            Console.Write("Select Payment Method: ");

            method = Console.ReadLine();

            if (method != "Digital Wallet" && method != "Debit Card" && method != "Credit Card")
            {
                Console.WriteLine("Invalid input! Please try again.");
            }
        }

        if (method == "Digital Wallet")
        {
            Console.WriteLine();
            Console.Write("Enter wallet type: ");
            string walletType = Console.ReadLine().ToUpper();
            selectedPaymentMethod = paymentMethods
                .OfType<DigitalWallet>()
                .FirstOrDefault(dw => dw.Type.ToUpper() == walletType);

            if (selectedPaymentMethod == null)
            {
                Console.WriteLine("No matching Digital Wallet found. Please try again.");
                continue;
            }

            var digitalWallet = (DigitalWallet)selectedPaymentMethod;
            string cardName;
            do
            {
                Console.WriteLine();
                Console.Write("Enter card name: ");
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
        else if (method == "Debit Card")
        {
            string cardNum;
            do
            {
                Console.WriteLine();
                Console.Write("Enter card number: ");
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
        else if (method == "Credit Card")
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
