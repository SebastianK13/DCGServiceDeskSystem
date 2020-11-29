
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.EF.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCGServiceDesk.EF.Factory
{
    public class DCGServiceDeskContextFactory<TContext> : IDbContextFactory<TContext>
        where TContext : DbContext
    {
        private readonly Action<DbContextOptionsBuilder> _dbContextConfig;

        public DCGServiceDeskContextFactory(Action<DbContextOptionsBuilder> dbContextConfig) =>
            _dbContextConfig = dbContextConfig;

        public TContext CreateDbContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseSqlServer(connectionString);
            return (TContext)Activator.CreateInstance(typeof(TContext), optionsBuilder.Options);
        }

    }
}