using Microsoft.Extensions.Logging;

namespace AppProyectoFinal
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            //Configuración de las fuentes del proyecto
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Afterglow-Regular.otf", "AfterglowRegular");
                    fonts.AddFont("TommySoft_Black.otf", "TommySoftBlack");
                    fonts.AddFont("TommySoft_Bold.otf", "TommySoftBold");
                    fonts.AddFont("TommySoft_ExtraBold.otf", "TommySoftExtraBold");
                    fonts.AddFont("TommySoft_Light.otf", "TommySoftLight");
                    fonts.AddFont("TommySoft_Medium.otf", "TommySoftMedium");
                    fonts.AddFont("TommySoft_Regular.otf", "TommySoftRegular");
                    fonts.AddFont("TommySoft_Thin.otf", "TommySoftThin");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}