using ShopMate.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ShopMate.Maping
{
    public class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            HasKey(o => o.Id);
            Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(o => o.Name).HasMaxLength(100);
            //   HasRequired(c => c.ProductCategory_ProductCategoryId).WithMany(o => o.Product_ProductCategoryIds).HasForeignKey(o => o.ProductCategoryId).WillCascadeOnDelete(false);
            Property(o => o.BarCode).HasMaxLength(100);
            Property(o => o.ProductImage).HasMaxLength(100);
            //Property(o => o.HSN).HasMaxLength(100);
            ToTable("Product");


        }
    }
}
