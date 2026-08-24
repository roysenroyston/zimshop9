using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShopMate.Models
{
    [TrackChanges]
    public class Currency
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("Currency")]
        public string Name { get; set; }
        public string CurrencySymbol { get; set; }
        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }
    }
}