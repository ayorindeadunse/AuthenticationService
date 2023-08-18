using System;
using AuthenticationService.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuthenticationService.Business.Extensions
{
    public static class AppBuilderExtension
    {
        public static void SetupMigrations(IServiceProvider service)
        {
            var logger = service.GetService<ILogger<ApplicationDbContext>>();
            try
            {
                var context = service.GetService<ApplicationDbContext>();
                context?.Database.Migrate();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ex.Message);
            }
        }
    }
}

