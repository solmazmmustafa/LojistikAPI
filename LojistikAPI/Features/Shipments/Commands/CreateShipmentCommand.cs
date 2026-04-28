using LojistikAPI.Models;
using MediatR;

namespace LojistikAPI.Features.Shipments.Commands
{
    // Bu sınıf postacının taşıyacağı zarftır. Geriye ShipmentResponseDto döneceğini belirtir.
    public class CreateShipmentCommand : IRequest<ShipmentResponseDto>
    {
        public CreateShipmentDto Dto { get; set; }
        public int UserId { get; set; }

        public CreateShipmentCommand(CreateShipmentDto dto, int userId)
        {
            Dto = dto;
            UserId = userId;
        }
    }
}