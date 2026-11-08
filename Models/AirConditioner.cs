namespace HomeApplianceRepair.Models
{
    /// <summary>Air conditioner: cost depends on gas amount and uninstall/install fee.</summary>
    public class AirConditioner : Device
    {
        private const decimal GasPricePerKg = 300m;
        private const decimal InstallationFee = 400m;

        private double _gasKg;
        private bool _needsInstallation;

        public AirConditioner(string serialNumber, string brand, DateTime receivedDate,
                              DateTime expectedDeliveryDate, decimal laborCost,
                              double gasKg, bool needsInstallation)
            : base(serialNumber, brand, receivedDate, expectedDeliveryDate, laborCost)
        {
            if (gasKg < 0) throw new ArgumentException("Gas amount cannot be negative.");

            _gasKg = gasKg;
            _needsInstallation = needsInstallation;
        }

        public double GasKg
        {
            get => _gasKg;
            set
            {
                if (value < 0) throw new ArgumentException("Gas amount cannot be negative.");
                _gasKg = value;
            }
        }

        public bool NeedsInstallation { get => _needsInstallation; set => _needsInstallation = value; }

        public override string DeviceType => "Air Conditioner";

        public override decimal CalculateRepairCost()
            => LaborCost + (decimal)_gasKg * GasPricePerKg + (_needsInstallation ? InstallationFee : 0m);

        protected override string GetDetails()
            => $"Gas: {_gasKg} kg, Uninstall/install: {(_needsInstallation ? "Yes" : "No")}";
    }
}
