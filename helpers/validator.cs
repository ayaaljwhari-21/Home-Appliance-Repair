using System.Text.RegularExpressions;

namespace HomeApplianceRepair.Helpers
{
    /// <summary>Static class that validates user input.</summary>
    public static class Validator
    {
        // Accepted serial format: two capital letters, a dash, six digits (e.g. SN-123456)
        private static readonly Regex SerialPattern = new Regex(@"^[A-Z]{2}-\d{6}$");

        public static bool IsValidSerialNumber(string? serial)
            => !string.IsNullOrWhiteSpace(serial) && SerialPattern.IsMatch(serial);

        public static bool IsValidName(string? text)
            => !string.IsNullOrWhiteSpace(text) && text.Trim().Length >= 2;

        /// <summary>The received date must not be in the future.</summary>
        public static bool IsValidReceivedDate(DateTime date)
            => date.Date <= DateTime.Today;

        /// <summary>The expected delivery date must not be before the received date.</summary>
        public static bool IsValidDeliveryDate(DateTime received, DateTime expected)
            => expected.Date >= received.Date;

        public static bool IsNonNegative(decimal value) => value >= 0;
    }
}
