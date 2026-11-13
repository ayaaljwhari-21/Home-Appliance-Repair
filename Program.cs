using HomeApplianceRepair.Enums;
using HomeApplianceRepair.Models;
using HomeApplianceRepair.Services;

var center = new RepairCenter();
LoadSampleData(center);

bool running = true;
while (running)
{
    Console.WriteLine("\n===== Home Appliance Repair Center =====");
    Console.WriteLine("1) Show all devices");
    Console.WriteLine("2) Add a new device");
    Console.WriteLine("3) Change device status");
    Console.WriteLine("4) Show devices by status");
    Console.WriteLine("5) Total cost and statistics");
    Console.WriteLine("6) Check overdue deliveries");
    Console.WriteLine("0) Exit");
    Console.Write("Your choice: ");

    try
    {
        switch (Console.ReadLine())
        {
            case "1": center.ShowAllDevices(); break;
            case "2": AddDeviceMenu(center); break;
            case "3": ChangeStatusMenu(center); break;
            case "4": ShowByStatusMenu(center); break;
            case "5": ShowStatistics(center); break;
            case "6": center.CheckAllOverdue(DateTime.Today); break;
            case "0": running = false; break;
            default: Console.WriteLine("Invalid choice."); break;
        }
    }
    catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException
                               || ex is KeyNotFoundException || ex is FormatException)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Error: {ex.Message}");
        Console.ResetColor();
    }
}

// ===================== Helper functions =====================

static void LoadSampleData(RepairCenter center)
{
    DateTime today = DateTime.Today;

    // The refrigerator is already late (expected delivery was 2 days ago)
    center.AddDevice(new Refrigerator("SN-100001", "Samsung", today.AddDays(-12), today.AddDays(-2), 300, true, false));
    center.AddDevice(new WashingMachine("SN-100002", "LG", today.AddDays(-3), today.AddDays(4), 250, false, 400));
    center.AddDevice(new AirConditioner("SN-100003", "Carrier", today.AddDays(-1), today.AddDays(5), 350, 1.5, true));
}

static void AddDeviceMenu(RepairCenter center)
{
    Console.Write("Type (1 Refrigerator, 2 Washing Machine, 3 Air Conditioner): ");
    string? type = Console.ReadLine();

    Console.Write("Serial number (e.g. SN-123456): ");
    string serial = Console.ReadLine() ?? "";
    Console.Write("Brand: ");
    string brand = Console.ReadLine() ?? "";
    DateTime received = ReadDate("Received date (yyyy-MM-dd): ");
    DateTime expected = ReadDate("Expected delivery date (yyyy-MM-dd): ");
    decimal labor = ReadDecimal("Labor cost: ");

    Device device;
    switch (type)
    {
        case "1":
            device = new Refrigerator(serial, brand, received, expected, labor,
                ReadYesNo("Replace compressor? (y/n): "), ReadYesNo("Replace thermostat? (y/n): "));
            break;
        case "2":
            device = new WashingMachine(serial, brand, received, expected, labor,
                ReadYesNo("Replace motor? (y/n): "), ReadDecimal("Parts cost: "));
            break;
        case "3":
            device = new AirConditioner(serial, brand, received, expected, labor,
                (double)ReadDecimal("Gas amount (kg): "), ReadYesNo("Needs uninstall/install? (y/n): "));
            break;
        default:
            Console.WriteLine("Invalid type.");
            return;
    }

    center.AddDevice(device);
    Console.WriteLine("Device added successfully.");
}

static void ChangeStatusMenu(RepairCenter center)
{
    Console.Write("Serial number: ");
    string serial = Console.ReadLine() ?? "";
    Console.Write("New status (1 In progress, 2 Repaired, 3 Delivered): ");

    RepairStatus status;
    switch (Console.ReadLine())
    {
        case "1": status = RepairStatus.InProgress; break;
        case "2": status = RepairStatus.Repaired; break;
        case "3": status = RepairStatus.Delivered; break;
        default: throw new ArgumentException("Invalid status.");
    }

    center.ChangeStatus(serial, status);
}

static void ShowByStatusMenu(RepairCenter center)
{
    Console.Write("Status (0 Received, 1 In progress, 2 Repaired, 3 Delivered): ");
    if (!Enum.TryParse(Console.ReadLine(), out RepairStatus status) || !Enum.IsDefined(typeof(RepairStatus), status))
        throw new ArgumentException("Invalid status.");

    center.ShowDevicesByStatus(status);
}

static void ShowStatistics(RepairCenter center)
{
    Console.WriteLine($"Total devices: {Device.TotalDevices}");
    Console.WriteLine($"Under maintenance: {Device.UnderMaintenanceCount}");
    Console.WriteLine($"Repaired: {Device.RepairedCount}");
    Console.WriteLine($"Total cost: {center.GetTotalCost():N0}");
    Console.WriteLine($"Cost of repaired devices: {center.GetTotalCostByStatus(RepairStatus.Repaired):N0}");
}

static DateTime ReadDate(string prompt)
{
    Console.Write(prompt);
    if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
        throw new FormatException("Invalid date format.");
    return date;
}

static decimal ReadDecimal(string prompt)
{
    Console.Write(prompt);
    if (!decimal.TryParse(Console.ReadLine(), out decimal value))
        throw new FormatException("Please enter a valid number.");
    return value;
}

static bool ReadYesNo(string prompt)
{
    Console.Write(prompt);
    return Console.ReadLine()?.Trim().ToLower() == "y";
}
