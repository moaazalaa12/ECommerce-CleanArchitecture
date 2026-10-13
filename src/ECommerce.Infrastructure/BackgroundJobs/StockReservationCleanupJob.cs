using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.CatalogInterfaces;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.BackgroundJobs;

public class StockReservationCleanupJob(IStockReservationRepository _reservationRepo, IProductRepository _productRepo, IUnitOfWork _unitOfWork, ILogger<StockReservationCleanupJob> _logger)
{
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("StockReservationCleanupJob started at {Time}", DateTime.UtcNow);

        var expiredReservations = await _reservationRepo.FindAsync(
            sr => !sr.IsReleased && sr.ExpiryDate < DateTime.UtcNow);

        if (!expiredReservations.Any())
        {
            _logger.LogInformation("No expired reservations found.");
            return;
        }

        foreach (var reservation in expiredReservations)
        {
            reservation.IsReleased = true;
            _reservationRepo.Update(reservation);

            var product = await _productRepo.GetByIdAsync(reservation.ProductId);
            if (product != null)
            {
                product.StockQuantity += reservation.Quantity;
                _productRepo.Update(product);

                _logger.LogInformation(
                    "Returned {Qty} units of Product {ProductId} to stock.",
                    reservation.Quantity, reservation.ProductId);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation(
            "StockReservationCleanupJob finished. Released {Count} reservations.",
            expiredReservations.Count());
    }
}