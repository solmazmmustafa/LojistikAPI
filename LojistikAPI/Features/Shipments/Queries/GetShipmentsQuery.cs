using MediatR;

namespace LojistikAPI.Features.Shipments.Queries
{
    // Listeleme isteği zarfımız
    public class GetShipmentsQuery : IRequest<object>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}