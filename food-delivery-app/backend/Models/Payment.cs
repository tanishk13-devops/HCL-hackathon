using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryAPI.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        [Required, StringLength(30)]
        public string PaymentMethod { get; set; } = "CashOnDelivery";

        [Required, StringLength(30)]
        public string PaymentStatus { get; set; } = "Pending";

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Order? Order { get; set; }
    }
}
