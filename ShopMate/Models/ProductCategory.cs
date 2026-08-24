using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopMate.Models
{
    [TrackChanges]
    public class ProductCategory
    {
        [DisplayName("S.No")] 
        public int Id { get; set; }
        [Required]
        [StringLength(100)] 
        [DisplayName("Name")] 
        public string Name { get; set; }
        [DisplayName("Parent")] 
        public Nullable<int> ParentId { get; set; }
        public virtual ProductCategory ProductCategory2 { get; set; }
        [DisplayName("Warehouse")] 
        public Nullable<int> WarehouseId { get; set; }
        public virtual ICollection<ProductCategory> ProductCategory_ParentIds { get; set; }
        public virtual ICollection<Product> Product_ProductCategoryIds { get; set; }

    }
}
