using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Ambassador.Service.Sales.Lib.Models
{
    public class LogHistory
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity), Key()]
        public int Id { get; set; }
        [MaxLength(255)]
        public string Division { get; set; }
        [MaxLength(1000)]
        public string Activity { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(255)]
        public string CreatedBy { get; set; }
        [MaxLength(255)]
        public string Remark { get; set; }
    }
}
