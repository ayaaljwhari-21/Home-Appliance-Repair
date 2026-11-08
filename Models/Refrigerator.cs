namespace HomeApplianceRepair.Models
{
    /// <summary>Refrigerator: cost depends on compressor and thermostat replacement.</summary>
    public class Refrigerator : Device
    {
        private const decimal CompressorPrice = 1500m;
        private const decimal ThermostatPrice = 200m;

        private bool _compressorReplaced;
        private bool _thermostatReplaced;

        public Refrigerator(string serialNumber, string brand, DateTime receivedDate,
                            DateTime expectedDeliveryDate, decimal laborCost,
                            bool compressorReplaced, bool thermostatReplaced)
            : base(serialNumber, brand, receivedDate, expectedDeliveryDate, laborCost)
        {
            _compressorReplaced = compressorReplaced;
            _thermostatReplaced = thermostatReplaced;
        }

        public bool CompressorReplaced { get => _compressorReplaced; set => _compressorReplaced = value; }
        public bool ThermostatReplaced { get => _thermostatReplaced; set => _thermostatReplaced = value; }

        public override string DeviceType => "Refrigerator";

        public override decimal CalculateRepairCost()
        {
            decimal cost = LaborCost;
            if (_compressorReplaced) cost += CompressorPrice;
            if (_thermostatReplaced) cost += ThermostatPrice;
            return cost;
        }

        protected override string GetDetails()
            => $"New compressor: {(_compressorReplaced ? "Yes" : "No")}, New thermostat: {(_thermostatReplaced ? "Yes" : "No")}";
    }
}
