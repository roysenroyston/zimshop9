using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopMate.Models
{
    [TrackChanges]
    public class Transaction
    {
        [DisplayName("S.No")] 
        public int Id { get; set; }
        [DisplayName("Debit Ledger Account")] 
        public int? DebitLedgerAccountId { get; set; }
        public virtual LedgerAccount LedgerAccount_DebitLedgerAccountId { get; set; }
        [DisplayName("Debit Amount")] 
        public Nullable<Decimal> DebitAmount { get; set; }
        [DisplayName("Credit Ledger Account")] 
        public int? CreditLedgerAccountId { get; set; }
        public virtual LedgerAccount LedgerAccount_CreditLedgerAccountId { get; set; }
        [DisplayName("Credit Amount")] 
        public Nullable<Decimal> CreditAmount { get; set; }
        [DisplayName("Added By")] 
        public Nullable<int> AddedBy { get; set; }
        [DisplayName("Date Added")] 
        public Nullable<DateTime> DateAdded { get; set; }
        [StringLength(100)] 
        [DisplayName("Other")] 
        public string Other { get; set; }
        [StringLength(20)] 
        [DisplayName("Purchase Or Sale")] 
        public string PurchaseOrSale { get; set; }
        [DisplayName("Purchase Or Sale")] 
        public Nullable<int> PurchaseIdOrSaleId { get; set; }
        [StringLength(100)] 
        [DisplayName("Remarks")] 
        public string Remarks { get; set; }
        [Required]
        [DisplayName("Warehouse")] 
        public int WarehouseId { get; set; }
        public bool IsFormal { get; set; }

    }
}
