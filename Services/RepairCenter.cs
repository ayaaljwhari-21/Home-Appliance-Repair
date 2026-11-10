using HomeApplianceRepair.Enums;
using HomeApplianceRepair.Models;

namespace HomeApplianceRepair.Services
{
    /// <summary>Main class: manages the device list and processes it polymorphically.</summary>
    public class RepairCenter
    {
        private readonly List<Device> _devices = new List<Device>();

        public IReadOnlyList<Device> Devices => _devices;

        public void AddDevice(Device device)
        {
            if (FindBySerial(device.SerialNumber) != null)
                throw new InvalidOperationException($"Device {device.SerialNumber} is already registered.");

            // Subscribe to the device events
            device.StatusChanged += OnStatusChanged;
            device.DeliveryOverdue += OnDeliveryOverdue;

            _devices.Add(device);
        }

        public Device? FindBySerial(string serial)
            => _devices.FirstOrDefault(d => d.SerialNumber == serial);

        public void ChangeStatus(string serial, RepairStatus newStatus)
        {
            Device device = FindBySerial(serial)
                ?? throw new KeyNotFoundException("Device not found.");
            device.ChangeStatus(newStatus);
        }

        // ===== Polymorphism: each device runs its own CalculateRepairCost =====
        public decimal GetTotalCost()
            => _devices.Sum(d => d.CalculateRepairCost());

        public decimal GetTotalCostByStatus(RepairStatus status)
            => _devices.Where(d => d.Status == status).Sum(d => d.CalculateRepairCost());

        public void ShowAllDevices()
        {
            if (_devices.Count == 0) { Console.WriteLine("No devices registered."); return; }
            foreach (Device d in _devices)
                Console.WriteLine(d.GetInfo());
        }

        public void ShowDevicesByStatus(RepairStatus status)
        {
            var list = _devices.Where(d => d.Status == status).ToList();
            if (list.Count == 0) { Console.WriteLine("No devices with this status."); return; }
            foreach (Device d in list)
                Console.WriteLine(d.GetInfo());
        }

        public void CheckAllOverdue(DateTime today)
        {
            foreach (Device d in _devices)
                d.CheckOverdue(today);
        }

        // ===== Event handlers =====
        private void OnStatusChanged(Device device, RepairStatus oldStatus, RepairStatus newStatus)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[NOTICE] {device.DeviceType} {device.SerialNumber}: {oldStatus} -> {newStatus}");

            if (newStatus == RepairStatus.Repaired)
                Console.WriteLine($"[NOTICE] Repair of {device.SerialNumber} is complete. Cost: {device.CalculateRepairCost():N0}. Please inform the customer.");

            Console.ResetColor();
        }

        private void OnDeliveryOverdue(Device device, int daysLate)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ALERT] Delivery of {device.DeviceType} {device.SerialNumber} is {daysLate} day(s) late!");
            Console.ResetColor();
        }
    }
}
