namespace WebApplication1.Repository
{
    public interface IQrService
    {
        string MakeQr(string text);

        // Helper for amounts
        string MakeQrForAmount(decimal amount, string? payload = null);
    }
}
