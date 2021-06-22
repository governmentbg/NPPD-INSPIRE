namespace Inspire.Portal.Services.CaptchaService
{
    public interface ICaptchaService
    {
        bool Validate(string encodedResponse);
    }
}