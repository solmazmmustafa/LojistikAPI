using Microsoft.AspNetCore.SignalR;

namespace LojistikAPI.Hubs
{
    // Bu telsiz merkezi, kullanıcılara (Müşteri veya Firma) özel canlı bildirimler göndermek içindir
    public class NotificationHub : Hub
    {
        // Kullanıcı sisteme (web sayfasına) giriş yaptığında kendi ID'sine özel bir odaya katılır.
        // Böylece başkasının bildirimi ona, onunki başkasına gitmez.
        public async Task JoinUserGroup(int userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }
    }
}