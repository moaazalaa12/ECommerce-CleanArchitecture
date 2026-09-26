using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Payments
{
    public class RefundRequestApiDto
    {
        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}
