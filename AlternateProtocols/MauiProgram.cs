using AlternateProtocols.Services;
using Microsoft.Extensions.Logging;

namespace AlternateProtocols
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IProtocolsService, ProtocolsService>();
            builder.Services.AddSingleton<MainPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            var protocolsService = app.Services.GetService<IProtocolsService>();
            InitProtocols(protocolsService);

            return app;
        }

        private async static void InitProtocols(IProtocolsService? protocolsService)
        {
            if (protocolsService == null)
            {
                return;
            }
            await protocolsService.GetProtocolsFromConfigAsync();
        }
    }
}
