using AngularAuthApi.Models;

namespace AngularAuthApi.utilityService
{
    public interface IEmailService
    {
        void SendEmail(EmailModel emailModel);
    }
}
