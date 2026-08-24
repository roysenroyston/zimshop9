using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ShopMate.Maping
{
    public class WarehouseMap : EntityTypeConfiguration<Warehouse> 
    {
        public WarehouseMap()
        {
             HasKey(o => o.Id);
             Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
             Property(o => o.Name).HasMaxLength(100);
             Property(o => o.Address).HasMaxLength(300);
             Property(o => o.Mobile).HasMaxLength(15);
             Property(o => o.Email).HasMaxLength(50);
             ToTable("Warehouse");
 

        }
    }
}
