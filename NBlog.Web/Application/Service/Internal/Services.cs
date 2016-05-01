namespace NBlog.Web.Application.Service.Internal
{
    public class Services : IServices
    {
        public Services(
            IEntryService entryService,
            IConfigService configService,
            IMessageService messageService)
        {
            Entry = entryService;
            Config = configService;
            Message = messageService;
        }
        
        public IEntryService Entry { get; private set; }
        public IConfigService Config { get; private set; }
        public IMessageService Message { get; private set; }
    }
}