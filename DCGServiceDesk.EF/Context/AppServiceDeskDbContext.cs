using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DCGServiceDesk.Data.Models;

namespace DCGServiceDesk.EF.Context
{
    public class AppServiceDeskDbContext : DbContext
    {
        public AppServiceDeskDbContext() { }
        public AppServiceDeskDbContext(DbContextOptions<AppServiceDeskDbContext>
            options) : base(options) { }
        public DbSet<ServiceRequest> Applications { get; set; }
        public DbSet<ScheduledWork> Works { get; set; }
        public DbSet<ApplicationConversation> Conversations { get; set; }
        public DbSet<Impact> Impacts { get; set; }
        public DbSet<Categorization> Categorizations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Urgency> Urgencies { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<AssigmentGroup> AssigmentGroup { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<StatusHistory> StatusHistory { get; set; }
        public DbSet<TaskRequest> Tasks { get; set; }
        public DbSet<NewAccountForm> AccountForms { get; set; }
        public DbSet<Incident> Incidents { get; set; }
    }
}
