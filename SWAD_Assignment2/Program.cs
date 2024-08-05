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
static List<Car> LoadCarsFromCSV(string carCsvFilePath, List<string> dates, List<Insurance> insuranceList)
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
        string status = values[7].Trim();

        Insurance insurance = null;
        if (status == "Y")
        {
             insurance = insuranceList.FirstOrDefault(i => i.CarPlateNo == licensePlate && i.CarOwnerId == ownerId);
        }

        List<Booking> bookings = new List<Booking>(); //empty list of bookings 

        var car = new Car(ownerId, licensePlate, carMake, model, year, mileage, availability, status, insurance, bookings);

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
                paymentMethods.Add(new DigitalWallet(bank, balance));
                break;
            case "CreditCard":
                paymentMethods.Add(new CreditCard(cardNum, name, balance, bank);
                break;
        }
    }

    return paymentMethods;
}


string usercsvFilePath = "Users_Data.csv";
string carCsvFilePath = "Car_List.csv";
string icCsvFilePath = "Insurance_Company_List.csv";
string insuranceCsvFilePath = "Insurance_list.csv";
string datesCsvFilePath = "DateTimeSlots.csv";
string paymenthMethodFilePath = "PaymentMethods.csv";

var users = LoadUsersFromCSV(usercsvFilePath);
var dates = LoadDateListFromCSV(datesCsvFilePath);
var companyDictionary = LoadCompanyDictionary(icCsvFilePath);
var insuranceList = LoadInsuranceFromCSV(insuranceCsvFilePath, companyDictionary);
var paymentMethods = LoadPaymentMethodsFromCSV(paymenthMethodFilePath);
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
                    RegisterCar(cars, insuranceList);
                    break;
                case "2":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

        static bool IsValidFileType(string fileName)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
            string fileExtension = System.IO.Path.GetExtension(fileName);
            return allowedExtensions.Contains(fileExtension);
        }

        static void RegisterCar(List<Car> cars, List<Insurance> insuranceList)
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

            List<string> photoFiles = new List<string>();

            Console.WriteLine();
            Console.WriteLine("Please upload images of the car!");
            
            string photoFile;
            while (true)
            {
                Console.WriteLine("Upload Photo (jpg/png/jpeg/pdf) or type 'cancel' to finish: ");
                photoFile = Console.ReadLine().Trim().ToLower();
                
                if (photoFile == "cancel")
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
            Console.WriteLine("Uploaded photos:");
            foreach (var file in photoFiles)
            {
                Console.WriteLine(file);
            } 
            
            // Check if car plate number exists in the insuranceList
            string insuranceStatus = insuranceList.Any(i => i.CarPlateNo == carPlateNo) ? "Y" : "X";

            Console.WriteLine();
            Console.WriteLine("Car details summary:");
            Console.WriteLine($"Car Make: {carMake}");
            Console.WriteLine($"Car Model: {carModel}");
            Console.WriteLine($"Car Mileage: {carMileage}");
            Console.WriteLine($"Year: {year}");
            Console.WriteLine($"Car Plate Number: {carPlateNo}");
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

            Console.WriteLine("Uploaded photos:");
            foreach (var file in photoFiles)
            {
                Console.WriteLine(file);
            }

            while (true)
            {
                Console.WriteLine("Are you sure you want to register this car? (yes/no)");
                string response = Console.ReadLine().Trim().ToLower();
                
                if (response == "yes")
                {
                    var newCar = new Car
                    {
                        CarMake = carMake,
                        Model = carModel,
                        Mileage = carMileage,
                        Year = year,
                        LicensePlate = carPlateNo,
                        InsuranceStatus = insuranceStatus
                    };
                    cars.Add(newCar);

                    Console.WriteLine("Car Successfully Registered!")
                    break;
                }
                else if (response == "no")
                {
                    Console.WriteLine("Car registration canceled.");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please type 'yes' or 'no'.");
                }
            }           

            // Check if car plate number exists in the insuranceList
            string insuranceStatus = insuranceList.Any(i => i.CarPlateNo == carPlateNo) ? "Y" : "X";
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
            Console.WriteLine("4. Return Car");
            Console.WriteLine("5. Exit");
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
                    returnCar();
                    break;
                case "5":
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
        string PenaltyFee = "Penalty Fee for late return: " + penaltyFee;
        display(PenaltyFee);
    }
    promptCheckForDamages();
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
    Console.WriteLine("Please check for damages. If there are damages, enter 'Has Damages'. Else enter 'No Damages'.");
    string damages = Console.ReadLine();
    while (damages != "Has Damages" && damages != "No Damages")
    {
        Console.WriteLine("Invalid input. Try again.");
        Console.WriteLine("Please check for damages. If there are damages, enter 'Has Damages'. Else enter 'No Damages'.");
        damages = Console.ReadLine();
    }
    updateDamages(damages);
}

void updateDamages(string damages)
{
    if (damages == "Has Damages")
    {
        reportAccident();
    }
}

void reportAccident() { }

void MakePayment() {
    Booking booking = getOngoingBooking((Renter)user);

    displayBooking(booking);
    string name = user.FullName;

    Console.WriteLine("Confirm payment amount? (yes/no) ");
    string res = Console.ReadLine();

    if (res != "yes")
    {
        Console.WriteLine("Payment Cancelled.");
        return;
    }

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
            }
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
                if (cardName != name)
                {
                    Console.WriteLine("Card name does not match user's name!");
                }
            } while (cardName != name);
            debitCard.CardName = cardName;

            Console.WriteLine("Enter bank: ");
            debitCard.Bank = Console.ReadLine();
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
                if (cardName != name)
                {
                    Console.WriteLine("Card name does not match user's name!");
                }
            } while (cardName != name);
            creditCard.CardName = cardName;

            Console.WriteLine("Enter bank: ");
            creditCard.Bank = Console.ReadLine();
        }
    }

    string status = "Confirmed";
    booking.updateBookingStatus(status);
}

void displayBooking(Booking currentBooking) {
    Console.WriteLine("BookingID: " + currentBooking.BookingId);
    Console.WriteLine("Start Date: " + currentBooking.StartDate.ToString());
    Console.WriteLine("End Date:  " + currentBooking.EndDate.ToString());
    Console.WriteLine("Pick Up Method: " + currentBooking.PickUpMethod);
    Console.WriteLine("Return Method: " + currentBooking.ReturnMethod);
    if (currentBooking.Status == "pending")
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