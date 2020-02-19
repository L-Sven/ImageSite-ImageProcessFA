using lu.Interfaces;
using lu.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace lu
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        }
    }
}