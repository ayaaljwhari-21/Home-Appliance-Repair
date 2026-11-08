using HomeApplianceRepair.Enums;
using HomeApplianceRepair.Events;
using HomeApplianceRepair.Helpers;
using HomeApplianceRepair.Interfaces;

namespace HomeApplianceRepair.Models
{
    /// <summary>Abstract base class for all home appliances.</summary>
    public abstract class Device : IDevice
    {
        // ===== Private fields =====
        private readonly string _serialNumber;      // read-only after creation
        private readonly DateTime _receivedDate;    // read-only after creation
        private string _brand;
        private DateTime _expectedDeliveryDate;
        private decimal _laborCost;
        private RepairStatus _status;

        // ===== Static counters per status =====
        private static readonly Dictionary<RepairStatus, int> StatusCounts = new Dictionary<RepairStatus, int>
        {
            { RepairStatus.Received, 0 }, { RepairStatus.InProgress, 0 },
            { RepairStatus.Repaired, 0 }, { RepairStatus.Delivered, 0 }
        };

        public static int TotalDevices => StatusCounts.Values.Sum();
        public static int UnderMaintenanceCount
            => StatusCounts[RepairStatus.Received] + StatusCounts[RepairStatus.InProgress];
        public static int RepairedCount
            => StatusCounts[RepairStatus.Repaired] + StatusCounts[RepairStatus.Delivered];

        // ===== Events =====
        public event DeviceStatusChangedHandler? StatusChanged;
        public event DeliveryOverdueHandler? DeliveryOverdue;

        // ===== Constructor =====
        protected Device(string serialNumber, string brand, DateTime receivedDate,
                         DateTime expectedDeliveryDate, decimal laborCost)
        {
            if (!Validator.IsValidSerialNumber(serialNumber))
                throw new ArgumentException("Invalid serial number (expected format: SN-123456).");
            if (!Validator.IsValidName(brand))
                throw new ArgumentException("Invalid brand name.");
            if (!Validator.IsValidReceivedDate(receivedDate))
                throw new ArgumentException("Received date cannot be in the future.");
            if (!Validator.IsValidDeliveryDate(receivedDate, expectedDeliveryDate))
                throw new ArgumentException("Delivery date cannot be before the received date.");
            if (!Validator.IsNonNegative(laborCost))
                throw new ArgumentException("Labor cost cannot be negative.");

            _serialNumber = serialNumber;
            _brand = brand.Trim();
            _receivedDate = receivedDate;
            _expectedDeliveryDate = expectedDeliveryDate;
            _laborCost = laborCost;
            _status = RepairStatus.Received;

            StatusCounts[RepairStatus.Received]++;
        }

        // ===== Properties =====
        public string SerialNumber => _serialNumber;
        public DateTime ReceivedDate => _receivedDate;
        public RepairStatus Status => _status;

        public string Brand
        {
            get => _brand;
            set
            {
                if (!Validator.IsValidName(value))
                    throw new ArgumentException("Invalid brand name.");
                _brand = value.Trim();
            }
        }

        public DateTime ExpectedDeliveryDate
        {
            get => _expectedDeliveryDate;
            set
            {
                if (!Validator.IsValidDeliveryDate(_receivedDate, value))
                    throw new ArgumentException("Delivery date cannot be before the received date.");
                _expectedDeliveryDate = value;
            }
        }

        public decimal LaborCost
        {
            get => _laborCost;
            set
            {
                if (!Validator.IsNonNegative(value))
                    throw new ArgumentException("Labor cost cannot be negative.");
                _laborCost = value;
            }
        }

        /// <summary>Device type name (Refrigerator, Washing Machine...) set by each subclass.</summary>
        public abstract string DeviceType { get; }

        // ===== Abstract method: each device calculates its own cost =====
        public abstract decimal CalculateRepairCost();

        /// <summary>Type-specific details (optional override).</summary>
        protected virtual string GetDetails() => string.Empty;

        // ===== Status change that raises an event =====
        public void ChangeStatus(RepairStatus newStatus)
        {
            if (newStatus == _status) return;
            if (newStatus < _status)
                throw new InvalidOperationException("Cannot move a device back to a previous status.");

            RepairStatus old = _status;
            _status = newStatus;

            StatusCounts[old]--;
            StatusCounts[newStatus]++;

            StatusChanged?.Invoke(this, old, newStatus);
        }

        /// <summary>Checks for a late delivery and raises the event if needed.</summary>
        public void CheckOverdue(DateTime today)
        {
            bool stillInShop = _status == RepairStatus.Received || _status == RepairStatus.InProgress;
            int daysLate = (today.Date - _expectedDeliveryDate.Date).Days;

            if (stillInShop && daysLate > 0)
                DeliveryOverdue?.Invoke(this, daysLate);
        }

        public string GetInfo()
        {
            string details = GetDetails();
            return $"[{DeviceType}] {_serialNumber} | {_brand} | Status: {_status} | " +
                   $"Received: {_receivedDate:yyyy-MM-dd} | Expected: {_expectedDeliveryDate:yyyy-MM-dd} | " +
                   $"Cost: {CalculateRepairCost():N0}" +
                   (details.Length > 0 ? $" | {details}" : "");
        }
    }
}
