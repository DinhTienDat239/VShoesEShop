using System.ComponentModel.DataAnnotations;

namespace VShoesEShop.Models
{
    public class Size
    {
        [Key]
        public int SizeId { get; set; }
        [StringLength(10)]
        public int? SizeNumber { get; set; }
    }
}
