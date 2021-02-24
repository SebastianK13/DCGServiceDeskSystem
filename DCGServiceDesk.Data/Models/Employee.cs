using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DCGServiceDesk.Data.Models
{
    public class Employee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }
        public string? Firstname { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? ProfilePicPath { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("Superior")]
        public int? SuperiorId { get; set; }
        public virtual Employee? Superior { get; set; }
        [ForeignKey("Location")]
        public string LocationId { get; set; }
        public virtual Location? Location { get; set; }
        [ForeignKey("Department")]
        public int? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        [ForeignKey("Zone")]
        public int? ZoneId { get; set; }
        public virtual TimeZonesModel? Zone { get; set; }
        [ForeignKey("Earnings")]
        public string? EarningsId { get; set; }
        public virtual Earnings? Earnings { get; set; }
        [ForeignKey("Positions")]
        public int? PositionId { get; set; }
        public virtual Position? Positions { get; set; }
    }
    public class Position
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PositionId { get; set; }
        public string? PositionName { get; set; }
    }
    public class Earnings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string SalaryId { get; set; }
        public decimal? Salary { get; set; }
        public decimal? Premium { get; set; }
        public string? Currency { get; set; }
    }
    public class Department
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        //public string? ChiefDepartment { get; set; }
        [ForeignKey("Employee")]
        public int? ChiefId { get; set; }
        public virtual Employee? Employee { get; set; }
    }
    public class Location
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string? LocationId { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? StreetAddress { get; set; }
        public string? ZipCode { get; set; }
        public string? Room { get; set; }
    }
    public class Presence
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string PresenceId { get; set; }
        public bool WorkBegin { get; set; }
        public DateTime WorkBeginTime { get; set; }
        public bool WorkEnd { get; set; }
        public DateTime WorkEndTime { get; set; }
        public double WorkTime { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
    public class Break
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int BreakId { get; set; }
        public bool Break_s { get; set; }
        public DateTime BreakStart { get; set; }
        public DateTime BreakEnd { get; set; }
        public double BreakTime { get; set; }
        [ForeignKey("Presence")]
        public string PresenceId { get; set; }
        public virtual Presence Presence { get; set; }
    }

    public class TaskSchedule
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string TaskId { get; set; }
        public string Task { get; set; }
        public string TaskDesc { get; set; }
        public DateTime TaskBegin { get; set; }
        public DateTime TaskDeadline { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
    }
    public class LoginByID
    {
        public string? Login { get; set; }
        public string? Id { get; set; }
    }
    public class CommunicationInfo
    {
        public string? ContactPerson { get; set; }
        public string? RequestedPerson { get; set; }
    }
}
