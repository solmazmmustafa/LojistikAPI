using LojistikAPI.Data;
using LojistikAPI.Entities;
using LojistikAPI.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LojistikAPI.Features.Shipments.Commands
{
    // Bu sınıf, zarfı alıp asıl veritabanı işini yapan işçidir.
    public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, ShipmentResponseDto>
    {
        private readonly AppDbContext _context;

        public CreateShipmentCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ShipmentResponseDto> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {
            var shipment = new Shipment
            {
                OriginAddress = request.Dto.OriginAddress!,
                DestinationAddress = request.Dto.DestinationAddress!,
                Weight = (double)request.Dto.Weight,
                Status = "Beklemede",
                CustomerId = request.UserId
            };

            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync(cancellationToken);

            return new ShipmentResponseDto
            {
                Id = shipment.Id,
                OriginAddress = shipment.OriginAddress,
                DestinationAddress = shipment.DestinationAddress,
                Weight = shipment.Weight,
                Status = shipment.Status
            };
        }
    }
}