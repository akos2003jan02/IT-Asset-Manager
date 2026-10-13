namespace IT.Asset.Manager.Models;

public class ImportPreviewItem
{
    public int RowNumber { get; set; }

    public string Manufacturer { get; set; } = "";

    public string Model { get; set; } = "";

    public string SerialNumber { get; set; } = "";

    public string Status { get; set; } = "";

    public string Result { get; set; } = "Ready";

    public string Message { get; set; } = "";

    public bool IsValid => Result == "Ready";
}