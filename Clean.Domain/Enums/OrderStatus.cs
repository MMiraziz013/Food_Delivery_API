namespace Clean.Domain.Enums;

public enum OrderStatus
{
    Created,
    Confirmed,
    InProgress,
    ReadyForDelivery,
    OnDelivery,
    Delivered,
    Cancelled
}