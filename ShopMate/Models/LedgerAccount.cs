using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopMate.Models
{
    [TrackChanges]
    public class LedgerAccount
    {
        [DisplayName("S.No")] 
        public int Id { get; set; }
        [Required]
        [StringLength(100)] 
        [DisplayName("Name")] 
        public string Name { get; set; }
        [DisplayName("Parent")] 
        public Nullable<int> ParentId { get; set; }
        public virtual LedgerAccount LedgerAccount2 { get; set; }
        [DisplayName("Date Added")] 
        public Nullable<DateTime> DateAdded { get; set; }
        [DisplayName("Added By")] 
        public Nullable<int> AddedBy { get; set; }
        [DisplayName("Warehouse")] 
        public Nullable<int> WarehouseId { get; set; }
        public virtual ICollection<LedgerAccount> LedgerAccount_ParentIds { get; set; }
        public virtual ICollection<Transaction> Transaction_DebitLedgerAccountIds { get; set; }
        public virtual ICollection<Transaction> Transaction_CreditLedgerAccountIds { get; set; }

    }
}
