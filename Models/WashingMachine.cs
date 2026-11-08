using HomeApplianceRepair.Helpers;

namespace HomeApplianceRepair.Models
{
    /// <summary>Washing machine: cost depends on motor replacement and parts price.</summary>
    public class WashingMachine : Device
    {
        private const decimal MotorPrice = 800m;

        private bool _motorReplaced;
        private decimal _partsCost;

        public WashingMachine(string serialNumber, string brand, DateTime receivedDate,
                              DateTime expectedDeliveryDate, decimal laborCost,
                              bool motorReplaced, decimal partsCost)
            : base(serialNumber, brand, receivedDate, expectedDeliveryDate, laborCost)
        {
            if (!Validator.IsNonNegative(partsCost))
                throw new ArgumentException("Parts cost cannot be negative.");

            _motorReplaced = motorReplaced;
            _partsCost = partsCost;
        }

        public bool MotorReplaced { get => _motorReplaced; set => _motorReplaced = value; }

        public decimal PartsCost
        {
            get => _partsCost;
            set
            {
                if (!Validator.IsNonNegative(value))
                    throw new ArgumentException("Parts cost cannot be negative.");
                _partsCost = value;
            }
        }

        public override string DeviceType => "Washing Machine";

        public override decimal CalculateRepairCost()
            => LaborCost + _partsCost + (_motorReplaced ? MotorPrice : 0m);

        protected override string GetDetails()
            => $"New motor: {(_motorReplaced ? "Yes" : "No")}, Parts: {_partsCost:N0}";
    }
}
