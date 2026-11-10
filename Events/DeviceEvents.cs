using HomeApplianceRepair.Enums;
using HomeApplianceRepair.Models;

namespace HomeApplianceRepair.Events
{
    /// <summary>Delegate used to notify that a device changed its status.</summary>
    public delegate void DeviceStatusChangedHandler(Device device, RepairStatus oldStatus, RepairStatus newStatus);

    /// <summary>Delegate used to notify that a device delivery is overdue.</summary>
    public delegate void DeliveryOverdueHandler(Device device, int daysLate);
}
