namespace Inspire.Services
{
    using System.Globalization;
    using System.Threading;

    using AutoMapper;

    using Inspire.Core.Infrastructure.RequestData;

    public abstract class BaseService
    {
        protected readonly IMapper Mapper;
        protected readonly IRequestData RequestData;

        protected BaseService(IMapper mapper, IRequestData requestData)
        {
            Mapper = mapper;
            RequestData = requestData;
        }

        public CultureInfo Culture => Thread.CurrentThread.CurrentUICulture;

        public string CurrentCulture => Culture.TwoLetterISOLanguageName.ToUpper();
    }
}