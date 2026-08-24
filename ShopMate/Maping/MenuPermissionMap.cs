using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ShopMate.Maping
{
    public class MenuPermissionMap : EntityTypeConfiguration<MenuPermission> 
    {
        public MenuPermissionMap()
        {
             HasKey(o => o.Id);
             Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
             HasOptional(c => c.Menu_MenuId).WithMany(o => o.MenuPermission_MenuIds).HasForeignKey(o => o.MenuId);
             HasRequired(c => c.Role_RoleId).WithMany(o => o.MenuPermission_RoleIds).HasForeignKey(o => o.RoleId).WillCascadeOnDelete(true);
             HasOptional(c => c.User_UserId).WithMany(o => o.MenuPermission_UserIds).HasForeignKey(o => o.UserId);
             ToTable("MenuPermission");
 

        }
    }
}
