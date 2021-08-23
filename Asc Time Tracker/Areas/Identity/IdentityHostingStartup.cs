using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Asc_Time_Tracker.Areas.Identity.IdentityHostingStartup))]

namespace Asc_Time_Tracker.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}