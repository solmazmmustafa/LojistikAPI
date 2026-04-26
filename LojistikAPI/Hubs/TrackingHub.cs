using Microsoft.AspNetCore.SignalR;

namespace LojistikAPI.Hubs
{
    // Hub, SignalR'ın telsiz merkezidir.
    public class TrackingHub : Hub
    {
        // Bir müşteri kendi kargosunun harita sayfasına girdiğinde bu gruba (odaya) katılacak.
        // Böylece sadece kendi kargosunun konum güncellemelerini duyacak.
        public async Task JoinShipmentGroup(string shipmentId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Shipment_{shipmentId}");
        }
    }
}