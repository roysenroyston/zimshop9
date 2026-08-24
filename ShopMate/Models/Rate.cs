using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    [TrackChanges]
    public class Rate
    {
        public int Id { get; set; }

        [DefaultValue(0.00)]
        [DisplayName("Currency Rate")]
        public double CurrencyRate { get; set; }

        [DisplayName("Modified Date")]
        public Nullable<DateTime> DateModified { get; set; } 

        public Currency Currency { get; set; }

        public int CurrencyId { get; set; }
        public int WarehouseId { get; set; }
        //public virtual ICollection<PaymentMode> PaymentMode { get; set; }
    }
}