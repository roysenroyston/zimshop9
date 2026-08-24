using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    [TrackChanges]
    public class Payment
    {
        public int Id { get; set; }
        [DisplayName("Added By")]
        public int AddedBy { get; set; }
        [DisplayName("Invoice Id")]
        public int? InvoiceId { get; set; }       
        [DisplayName("Bank")]
        public int? BankId { get; set; }
        public Bank Bank_BankId { get; set; }
        [Required]
        [DisplayName("Customer")]
        public int? CustomerId { get; set; }
        public User User_UserId { get; set; }
        [DisplayName("Account Number")]
        public string BankAccount { get; set; }
        [DisplayName("Currency")]
        public int? CurrencyId { get; set; }
        public Currency Currency_CurrencyId { get; set; }
        [DisplayName("Payment Mode")]
        public int PaymentModeId { get; set; }
        [StringLength(100)]
        [DisplayName("Payment Reference")]
        public string PaymentReference { get; set; }
        [Required]
        public decimal? Amount { get; set; }
        public decimal CurrencyAmount { get; set; }
        public DateTime PaymentDate { get; set; }


    }
}