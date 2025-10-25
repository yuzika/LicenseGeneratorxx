using LicenseGenerator.Helpers;

public class LicenseValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; }
    public string UsbSerial { get; set; }
    public string DbGuidKey { get; set; }
    public string DbMasterKey { get; set; }
    public string ResolvedAppMasterKey { get; set; }
    public string Timestamp { get; set; }

    public static LicenseValidationResult Fail(string errorMessage)
    {
        return new LicenseValidationResult { IsValid = false, ErrorMessage = errorMessage };
    }
}