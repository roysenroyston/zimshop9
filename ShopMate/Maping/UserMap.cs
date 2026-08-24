using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ShopMate.Maping
{
    public class UserMap : EntityTypeConfiguration<User> 
    {
        public UserMap()
        {
             HasKey(o => o.Id);
             Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
             Property(o => o.UserName).HasMaxLength(100);
             Property(o => o.Password).HasMaxLength(100);
             Property(o => o.FullName).HasMaxLength(111);
             Property(o => o.Mobile).HasMaxLength(15);
             Property(o => o.Address).HasMaxLength(200);
             Property(o => o.About).HasMaxLength(500);
             HasRequired(c => c.Role_RoleId).WithMany(o => o.User_RoleIds).HasForeignKey(o => o.RoleId).WillCascadeOnDelete(false);
             ToTable("User");
 

        }
    }
}
