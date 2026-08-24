using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ShopMate.Maping
{
    public class InvoiceItemsMap : EntityTypeConfiguration<InvoiceItems>
    {
        public InvoiceItemsMap()
        {
            HasKey(o => o.Id);
            Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            HasRequired(c => c.Product_ProductId).WithMany(o => o.InvoiceItems_ProductIds).HasForeignKey(o => o.ProductId).WillCascadeOnDelete(false);
            //HasRequired(c => c.Invoice_InvoiceId).WithMany(o => o.InvoiceItems_InvoiceIds).HasForeignKey(o => o.InvoiceId).WillCascadeOnDelete(true);
            ToTable("InvoiceItems");

        }
    }
}
