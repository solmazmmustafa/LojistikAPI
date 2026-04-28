using LojistikAPI.Data;
using LojistikAPI.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LojistikAPI.Features.Shipments.Queries
{
    public class GetShipmentsQueryHandler : IRequestHandler<GetShipmentsQuery, object>
    {
        private readonly AppDbContext _context;

        public GetShipmentsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> Handle(GetShipmentsQuery request, CancellationToken cancellationToken)
        {
            int pageSize = Math.Clamp(request.PageSize, 1, 50);
            var totalCount = await _context.Shipments.CountAsync(cancellationToken);

            var shipments = await _context.Shipments
                .AsNoTracking()
                .OrderByDescending(s => s.Id)
                .Skip((request.Page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new ShipmentResponseDto
                {
                    Id = s.Id,
                    OriginAddress = s.OriginAddress,
                    DestinationAddress = s.DestinationAddress,
                    Weight = s.Weight,
                    Status = s.Status
                })
                .ToListAsync(cancellationToken);

            return new
            {
                data = shipments,
                totalCount,
                page = request.Page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
    }
}