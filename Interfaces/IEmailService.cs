namespace TickSyncAPI.Interfaces
{
    public interface IEmailService
    {
        public Task<string> SendEmail(string email,string subject, string body);
    }
}
