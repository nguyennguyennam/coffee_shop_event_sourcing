using backend.application.Models;

namespace backend.application.interfaces.command
{
    public interface IOrderCommand
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> UpdateOrderAsync(Guid OrderId, string newStatus);
    }
}