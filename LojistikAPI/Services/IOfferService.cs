using LojistikAPI.Models;

namespace LojistikAPI.Services
{
    public interface IOfferService
    {
        Task<int> CreateOfferAsync(CreateOfferDto dto);
    }
}