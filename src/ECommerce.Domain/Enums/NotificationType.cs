namespace ECommerce.Domain.Enums;

public enum NotificationType
{
    OrderPlaced = 0,
    OrderShipped = 1,
    OrderDelivered = 2,
    OrderCancelled = 3,
    PaymentSuccess = 4,
    PaymentFailed = 5,
    NewReview = 6,
    CouponApplied = 7
}