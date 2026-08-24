using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class FinishedGoods
    {
       
            [DisplayName(" ID")]
            public int Id { get; set; }
            [DisplayName("Total Amount")]
            public Decimal CostPrice { get; set; }
            [Required]
            [DisplayName("Warehouse")]
            public int WarehouseId { get; set; }
            [DisplayName("Finished Date")]
            public Nullable<DateTime> finisheddate { get; set; }
            [DisplayName("Added By")]
            public Nullable<int> AddedBy { get; set; }
            public IEnumerable<finishedItem> finishedItems { get; set; }
        }
    }