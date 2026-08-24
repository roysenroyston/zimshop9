using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ShopMate.Maping
{
    public class TransactionMap : EntityTypeConfiguration<Transaction> 
    {
        public TransactionMap()
        {
             HasKey(o => o.Id);
             Property(o => o.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
             HasOptional(c => c.LedgerAccount_DebitLedgerAccountId).WithMany(o => o.Transaction_DebitLedgerAccountIds).HasForeignKey(o => o.DebitLedgerAccountId);
             HasOptional(c => c.LedgerAccount_CreditLedgerAccountId).WithMany(o => o.Transaction_CreditLedgerAccountIds).HasForeignKey(o => o.CreditLedgerAccountId);
             Property(o => o.Other).HasMaxLength(100);
             Property(o => o.PurchaseOrSale).HasMaxLength(20);
             Property(o => o.Remarks).HasMaxLength(100);
             ToTable("Transaction");
 

        }
    }
}
