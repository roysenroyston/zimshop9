using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ShopMate.Maping
{
    public class RoleMap : EntityTypeConfiguration<Role> 
    {
        public RoleMap()
        {
             HasKey(o => o.Id);
             Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
             Property(o => o.RoleName).HasMaxLength(50);
             ToTable("Role");
 

        }
    }
}
