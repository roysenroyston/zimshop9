using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ShopMate.Maping
{
    public class PurchaseMap : EntityTypeConfiguration<Purchase> 
    {
        public PurchaseMap()
        {
             HasKey(o => o.Id);
             Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
             HasRequired(c => c.Vendor_Id).WithMany(o => o.Purchase_VendorUserIds).HasForeignKey(o => o.VendorUserId).WillCascadeOnDelete(false);
             HasRequired(c => c.Product_ProductId).WithMany(o => o.Purchase_ProductIds).HasForeignKey(o => o.ProductId).WillCascadeOnDelete(false);
             ToTable("Purchase");
 

        }
    }
}
