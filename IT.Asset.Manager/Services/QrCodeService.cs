using System.Drawing;
using System.Drawing.Imaging;
using QRCoder;

namespace IT.Asset.Manager.Services;

public class QrCodeService
{
    public Bitmap Generate(
        string content,
        int pixelsPerModule = 10)
    {
        using var generator = new QRCodeGenerator();

        using QRCodeData data =
            generator.CreateQrCode(
                content,
                QRCodeGenerator.ECCLevel.Q);

        var qrCode = new QRCode(data);

        return qrCode.GetGraphic(
            pixelsPerModule);
    }
}