namespace Application.Options;

public class FrontendApplicationOptions
{
    public const string FrontEndOptionsKey = "FrontEnd";

    public string BaseUrl { get; set; } = string.Empty;
    public string PasswordReset { get; set; } = string.Empty;
    public string EmailVerification { get; set; } = string.Empty;
    public string AppName { get; set; } = "Meetini"; 
    public string SupportLink { get; set; } = "https://meetini.com/support"; 
    public string SecurityLink { get; set; } = "https://meetini.com/report";
}