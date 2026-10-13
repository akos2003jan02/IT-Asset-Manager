namespace IT.Asset.Manager.Models;

public class AssetHistory
{
    public int Id { get; set; }

    public int AssetId { get; set; }

    public string Manufacturer { get; set; } = "";

    public string Model { get; set; } = "";

    public string SerialNumber { get; set; } = "";

    public string Action { get; set; } = "";

    public string Details { get; set; } = "";

    public DateTime Timestamp { get; set; } = DateTime.Now;
}