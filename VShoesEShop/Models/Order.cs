using System.ComponentModel.DataAnnotations;

namespace VShoesEShop.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        [Required]
        public string LastName { get; set; }
        [StringLength(200)]
        [Required]
        public string Email { get; set; }
        [StringLength(200)]
        [Required]
        public string Address { get; set; }
        [Required]
        [StringLength(200)]
        public string Country { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; }

    }
}
