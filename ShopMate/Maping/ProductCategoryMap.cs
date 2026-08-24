using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ShopMate.Maping
{
    public class ProductCategoryMap : EntityTypeConfiguration<ProductCategory> 
    {
        public ProductCategoryMap()
        {
             HasKey(o => o.Id);
             Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
             Property(o => o.Name).HasMaxLength(100);
             HasOptional(c => c.ProductCategory2).WithMany(o => o.ProductCategory_ParentIds).HasForeignKey(o => o.ParentId);
             ToTable("ProductCategory");
 

        }
    }
}
