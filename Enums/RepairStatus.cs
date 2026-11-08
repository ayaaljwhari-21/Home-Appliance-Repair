namespace HomeApplianceRepair.Enums
{
    /// <summary>Stages of a device inside the repair center, in natural order.</summary>
    public enum RepairStatus
    {
        Received = 0,
        InProgress = 1,
        Repaired = 2,
        Delivered = 3
    }
}
