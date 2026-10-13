namespace IT.Asset.Manager.Models;

public class AssetWarning
{
    public int AssetId { get; set; }

    public string WarningType { get; set; } = "";

    public string Message { get; set; } = "";

    public string Severity { get; set; } = "Warning";

    public string ActionType { get; set; } = "Edit";
}