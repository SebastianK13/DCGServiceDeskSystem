using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DCGServiceDesk.Data.Models
{
    public class ServiceRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int RequestId { get; set; }
        public string? Topic { get; set; }
        public string? Description { get; set; }
        public int RequestedPerson { get; set; }
        public int ContactPerson { get; set; }
        [ForeignKey("Impact")]
        public int ImpactId { get; set; }
        [ForeignKey("Urgency")]
        public int UrgencyId { get; set; }
        [ForeignKey("Priority")]
        public int PriorityId { get; set; }
        [ForeignKey("Group")]
        public int? GroupId { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Status? Status { get; set; }
        public virtual Impact? Impact { get; set; }//
        public virtual Urgency? Urgency { get; set; }//
        public virtual Priority? Priority { get; set; }
        public virtual AssigmentGroup? Group { get; set; }
        public virtual Categorization? Category { get; set; }
        
    }
    public class TaskRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int TaskId { get; set; }
        public string? Topic { get; set; }
        public string? Description { get; set; }
        public int RequestedPerson { get; set; }
        public int ContactPerson { get; set; }
        [ForeignKey("Impact")]
        public int ImpactId { get; set; }
        [ForeignKey("Urgency")]
        public int UrgencyId { get; set; }
        [ForeignKey("Priority")]
        public int PriorityId { get; set; }
        [ForeignKey("Group")]
        public int? GroupId { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [ForeignKey("AccountForm")]
        public int? AccountFormId { get; set; }
        public virtual Status? Status { get; set; }
        public virtual Impact? Impact { get; set; }//
        public virtual Urgency? Urgency { get; set; }//
        public virtual Priority? Priority { get; set; }
        public virtual AssigmentGroup? Group { get; set; }
        public virtual Categorization? Category { get; set; }
        public virtual NewAccountForm? AccountForm { get; set; }
    }
    public class Incident
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int IncidentId { get; set; }
        public string? Topic { get; set; }
        public string? Description { get; set; }
        public int RequestedPerson { get; set; }
        public int ContactPerson { get; set; }
        [ForeignKey("Impact")]
        public int ImpactId { get; set; }
        [ForeignKey("Urgency")]
        public int UrgencyId { get; set; }
        [ForeignKey("Priority")]
        public int PriorityId { get; set; }
        [ForeignKey("Group")]
        public int? GroupId { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Status? Status { get; set; }
        public virtual Impact? Impact { get; set; }
        public virtual Urgency? Urgency { get; set; }
        public virtual Priority? Priority { get; set; }
        public virtual AssigmentGroup? Group { get; set; }
        public virtual Categorization? Category { get; set; }
    }
    public class NewAccountForm
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int AccountRequestId { get; set; }
        public string? Firstname { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? ZipCode { get; set; }
        public int PositionId { get; set; }
        public int DepartmentId { get; set; }
        public int SuperiorId { get; set; }
        public int TimeZoneId { get; set; }
    }
    public class ApplicationConversation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string? User { get; set; }
        public string? Administrator { get; set; }
        public string? Message { get; set; }
        public DateTime MessageDate { get; set; }
        [ForeignKey("ServiceRequest")]
        public int RequestId { get; set; }
        public virtual ServiceRequest? ServiceRequest { get; set; }
    }
    public class ScheduledWork
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ServiceWorkId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int ResponsibleEmployee { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class Categorization
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
    }
    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Designation { get; set; }
    }
    public class Impact
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        //max 4 lvl
        public int level { get; set; }
        public string? Name { get; set; }
    }
    public class Urgency
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        //max 4 lvl
        public int level { get; set; }
        public string? Name { get; set; }
    }
    public class Priority
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        //max 4 lvl
        public int level { get; private set; }
        public string? Name { get; private set; }
    }
    public class Status
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int StatusId { get; set; }
        [ForeignKey("State")]
        public int StateId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DueTime { get; set; }
        public bool Expired { get; set; }
        public virtual State? State { get; set; }
    }

    public class StatusHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ChangeId { get; set; }
        [ForeignKey("Status")]
        public int? StatusId { get; set; }
        [ForeignKey("State")]
        public int? StateId { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public bool Active { get; set; }
        public virtual State? State { get; set; }
        public virtual Status? Status { get; set; }
    }

    public class State
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int StateId { get; set; }
        public string? StateName { get; set; }
    }

    public class AssigmentGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int GroupId { get; set; }
        public string? GroupName { get; set; }

    }
    public class Member
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int MemberId { get; set; }
        public int Username { get; set; }
        [ForeignKey("Group")]
        public int GroupId { get; set; }
        public virtual AssigmentGroup? Group { get; set; }
    }
}