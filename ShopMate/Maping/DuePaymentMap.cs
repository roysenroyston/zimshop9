using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ShopMate.Maping
{
    public class DuePaymentMap : EntityTypeConfiguration<DuePayment> 
    {
        public DuePaymentMap()
        {
             HasKey(o => o.Id);
             Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
             HasRequired(c => c.User_UserId).WithMany(o => o.DuePayment_UserIds).HasForeignKey(o => o.UserId).WillCascadeOnDelete(false);
             Property(o => o.Remarks).HasMaxLength(200);
             ToTable("DuePayment");
 

        }
    }
}
