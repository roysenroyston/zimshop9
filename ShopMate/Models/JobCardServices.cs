using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    [TrackChanges]
    public class JobCardServices
    {
        [DisplayName(" ID")]
        public int Id { get; set; }

        [DisplayName("Machine Used")]
        public int? machineused { get; set; }
        public virtual Machine Machine_MachineId { get; set; }

        [DisplayName("Artisan Name")]
        public int? artisan { get; set; }
        public virtual User User_UserId { get; set; }

        [DisplayName("Hours")]
        public string hours { get; set; }

        [DisplayName("Rate")]
        public decimal rate { get; set; }


        [DisplayName("Invoice")]
        public Nullable<int> InvoiceId { get; set; }
        public virtual Invoice Invoice_InvoiceId { get; set; }

        [DisplayName("JobCard")]
        public Nullable<int> JobCardId { get; set; }
        public virtual JobCard JobCard_JobCardId { get; set; }

    }
}