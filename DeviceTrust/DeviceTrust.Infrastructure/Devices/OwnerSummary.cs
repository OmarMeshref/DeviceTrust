namespace DeviceTrust.Infrastructure.Devices;

public class OwnerSummary
{
    public int TotalDevices { get; set; }
    public int PendingTransfersOut { get; set; }
    public int PendingTransfersIn { get; set; }
    public int TotalRepairsAcrossDevices { get; set; }
}