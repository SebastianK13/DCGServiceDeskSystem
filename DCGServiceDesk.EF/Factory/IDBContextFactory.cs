using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCGServiceDesk.EF.Factory
{
    public interface IDbContextFactory<TContext> where TContext : DbContext
    {
        TContext CreateDbContext(string connectionString);
    }
}