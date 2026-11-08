using HomeApplianceRepair.Enums;

namespace HomeApplianceRepair.Interfaces
{
    /// <summary>Core operations shared by every device type.</summary>
    public interface IDevice
    {
        string SerialNumber { get; }
        string Brand { get; }
        RepairStatus Status { get; }

        decimal CalculateRepairCost();
        void ChangeStatus(RepairStatus newStatus);
        string GetInfo();
    }
}
