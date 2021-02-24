using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DCGServiceDesk.Data.Models;

namespace DCGServiceDesk.EF.Context
{
    public class AppEmployeesDbContext : DbContext
    {
        public AppEmployeesDbContext() { }
        public AppEmployeesDbContext(DbContextOptions<AppEmployeesDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Earnings> Earnings { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Presence> Presences { get; set; }
        public DbSet<TaskSchedule> TaskSchedules { get; set; }
        public DbSet<Break> Breaks { get; set; }
        public DbSet<TimeZonesModel> Zone { get; set; }
        public DbSet<Position> Positions { get; set; }
    }
}
