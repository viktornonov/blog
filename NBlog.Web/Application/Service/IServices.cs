namespace NBlog.Web.Application.Service
{
    public interface IServices
    {
        IEntryService Entry { get; }
        IConfigService Config { get; }
        IMessageService Message { get; }
    }
}