using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DCGServiceDesk.Data.Models
{
    public class TimeZonesModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ZonId { get; set; }
        public string? TimeZoneID { get; set; }
        public string? TimeZoneName { get; set; }
        public int TimeDifference { get; set; }

    }
}