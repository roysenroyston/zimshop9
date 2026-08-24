using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ShopMate.Maping
{
    public class DayEndMap
    {
        public DayEndMap()
        {
            //HasKey(o => o.Id);
            //Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //HasRequired(c => c.User_CustomerUserId).WithMany(o => o.Sale_CustomerUserIds).HasForeignKey(o => o.CustomerUserId).WillCascadeOnDelete(false);
            //HasRequired(c => c.PaymentMode_PaymentModeId).WithMany(o => o.Sale_PaymentModeIds).HasForeignKey(o => o.PaymentModeId).WillCascadeOnDelete(false);
            //HasRequired(c => c.Product_ProductId).WithMany(o => o.Sale_ProductIds).HasForeignKey(o => o.ProductId).WillCascadeOnDelete(false);
            //ToTable("DayEnd");


        }
    }
}