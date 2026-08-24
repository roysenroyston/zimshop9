using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ShopMate.Maping
{
    public class ProductStockMap : EntityTypeConfiguration<ProductStock> 
    {
        public ProductStockMap()
        {
             HasKey(o => o.Id);
             Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
             HasRequired(c => c.Product_ProductId).WithMany(o => o.ProductStock_ProductIds).HasForeignKey(o => o.ProductId).WillCascadeOnDelete(false);
             Property(o => o.Description).HasMaxLength(200);
             HasRequired(c => c.InventoryType_InventoryTypeId).WithMany(o => o.ProductStock_InventoryTypeIds).HasForeignKey(o => o.InventoryTypeId).WillCascadeOnDelete(false);
             ToTable("ProductStock");
 

        }
    }
}
