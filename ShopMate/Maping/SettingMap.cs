using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ShopMate.Maping
{
    public class SettingMap : EntityTypeConfiguration<Setting> 
    {
        public SettingMap()
        {
             HasKey(o => o.Id);
             Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
             Property(o => o.sKey).HasMaxLength(200);
             Property(o => o.sValue).HasMaxLength(3000);
             Property(o => o.sGroup).HasMaxLength(500);
             ToTable("Setting");
 

        }
    }
}
