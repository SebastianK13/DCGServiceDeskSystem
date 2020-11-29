using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DCGServiceDesk.Data.Models
{
    public class Customer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int CustomerId { get; set; }
        public string? ResponsibleEmployee { get; set; }
        [Required(ErrorMessage = "Customer name is required")]
        public string? CustomerName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        [ForeignKey("CustomerLocation")]
        public string? CLocationId { get; set; }
        public virtual CustomerLocation? CustomerLocation { get; set; }
    }
    public class CustomerLocation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string? LocationId { get; set; }
        [Required(ErrorMessage = "City is required")]
        public string? City { get; set; }
        [Required(ErrorMessage = "Country is required")]
        public string? Country { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string? StreetAddress { get; set; }
        [Required(ErrorMessage = "Zip code is required")]
        public string? ZipCode { get; set; }
    }
    public class Invoice
    {
        [Key]
        public string? InvoiceId { get; set; }
        public DateTime Date { get; set; }
        public DateTime ExpiresDate { get; set; }
        public string? ResponsibleEmployee { get; set; }
        [ForeignKey("InvoiceDetails")]
        public int InvoiceDetailsId { get; set; }
        public virtual InvoiceDetails? InvoiceDetails { get; set; }
        [ForeignKey("Payment")]
        public int? PaymentId { get; set; }
        public virtual Payment? Payment { get; set; }
        [ForeignKey("Customer")]
        public int? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }

    }
    public class InvoiceDetails
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int DetailsId { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Currency { get; set; }

    }
    public class Service
    {
        public Service() { }
        public Service(List<string>? services, string? invId)
        {            
            ServicePrice = Convert.ToDecimal(services?[0]);
            Quantity = Convert.ToInt32(services?[1]);
            ServiceName = services?[2];
            InvoiceId = invId;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public int? Quantity { get; set; }
        public decimal? ServicePrice { get; set; }
        [ForeignKey("Invoice")]
        public string? InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
    public class Payment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PaymentId { get; set; }
        public string? MethodName { get; set; }
        public DateTime PaymentDeadline { get; set; }
    }
}
