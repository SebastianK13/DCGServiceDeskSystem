
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.EF.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCGServiceDesk.EF.Factory
{
    public class DCGServiceDeskContextFactory : IDatabaseContextFactory
    {
        private readonly IConfiguration _configuration;

        public DCGServiceDeskContextFactory(IConfiguration configuration) =>
            _configuration = configuration;

        public AppIdentityDbContext CreateIdentityDbContext()
        {
            AppIdentityDbContext dbContext = new AppIdentityDbContext();
            DbContextOptionsBuilder<AppIdentityDbContext> options = new DbContextOptionsBuilder<AppIdentityDbContext>();
            options.UseSqlServer(_configuration["Data:DCTEIdentity:ConnectionString"]);

            return new AppIdentityDbContext(options.Options);
        }

        public AppServiceDeskDbContext CreateServiceDeskDbContext()
        {
            AppServiceDeskDbContext dbContext = new AppServiceDeskDbContext();
            DbContextOptionsBuilder<AppServiceDeskDbContext> options = new DbContextOptionsBuilder<AppServiceDeskDbContext>();
            options.UseSqlServer(_configuration["Data:DCTEServiceDesk:ConnectionString"]);

            return new AppServiceDeskDbContext(options.Options);
        }
    }
}