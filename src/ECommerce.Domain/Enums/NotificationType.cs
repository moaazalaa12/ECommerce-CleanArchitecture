namespace ECommerce.Domain.Enums;

public enum NotificationType
{
    OrderPlaced = 0,
    OrderProcessing = 1,
    OrderShipped = 2,
    OrderDelivered = 3,
    OrderCancelled = 4,
    PaymentSuccess = 5,
    PaymentFailed = 6,
    PaymentRefunded = 7,
    NewReview = 8,
    CouponApplied = 9
}