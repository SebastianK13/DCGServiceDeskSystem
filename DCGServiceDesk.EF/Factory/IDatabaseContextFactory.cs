using DCGServiceDesk.EF.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCGServiceDesk.EF.Factory
{
    public interface IDatabaseContextFactory
    {
        AppIdentityDbContext CreateIdentityDbContext();
    }
}
