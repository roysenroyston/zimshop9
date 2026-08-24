using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ShopMate.Maping
{
    public class InvoiceFormatMap : EntityTypeConfiguration<InvoiceFormat> 
    {
        public InvoiceFormatMap()
        {
             HasKey(o => o.Id);
             Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
             Property(o => o.CompanyName).HasMaxLength(100);
             Property(o => o.Logo).HasMaxLength(100);
             Property(o => o.AddressInfo).HasMaxLength(500);
             Property(o => o.OtherInfo).HasMaxLength(500);
             Property(o => o.FooterInfo).HasMaxLength(500);
             ToTable("InvoiceFormat");
 

        }
    }
}
