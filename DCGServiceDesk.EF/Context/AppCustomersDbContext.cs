using DCGServiceDesk.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DCGServiceDesk.EF.Context
{
    public class AppCustomersDbContext:DbContext
    {
        public AppCustomersDbContext(DbContextOptions<AppCustomersDbContext>
            options) : base(options) { }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerLocation> CustomersLocation { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetails> InvoicesDetails { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }
}
