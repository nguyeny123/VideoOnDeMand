using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VideoOnDemand.Data.Data;
using VideoOnDemand.Data.Data.Entities;

[assembly: HostingStartup(typeof(VideoOnDemand.UI.Areas.Identity.IdentityHostingStartup))]
namespace VideoOnDemand.UI.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}