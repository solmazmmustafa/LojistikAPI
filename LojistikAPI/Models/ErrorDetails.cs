using System.Text.Json;

namespace LojistikAPI.Models
{
    // API'nin patladığı durumlarda dışarıya döneceğimiz standart şablon
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}