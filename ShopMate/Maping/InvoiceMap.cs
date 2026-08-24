using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ShopMate.Maping
{
    public class InvoiceMap : EntityTypeConfiguration<Invoice> 
    {
        public InvoiceMap()
        {
             HasKey(o => o.Id);
             Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
             Property(o => o.IsPurchaseOrSale).HasMaxLength(20);
             ToTable("Invoice");
 

        }
    }
}
