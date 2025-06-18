namespace SharedKernel;

public class FrontendApplicationOptions
{
  public const string FrontEndOptionsKey = "FrontEnd";
  public string BaseUrl { get; set; } = string.Empty;
  public string EmailVerificationPagePath { get; set; } = string.Empty;
}