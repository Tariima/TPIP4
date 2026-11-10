using Application.Interfaces;
using Application.Models;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace Infrastructure.Services;

// Llama a la API de Orders de Mercado Pago (Checkout Pro).
// El HttpClient lo da la HttpClientFactory ya configurado en Program.cs (url base, token y reintentos con Polly)
public class MercadoPagoService : IMercadoPagoService
{
    private readonly HttpClient _httpClient;

    // Mercado Pago usa nombres con guion bajo (total_amount), nosotros PascalCase (TotalAmount).
    // Con esta opción se convierten solos en los dos sentidos
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public MercadoPagoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<OrdenMercadoPagoDTO> CrearOrdenAsync(int idPago, decimal monto, string descripcion)
    {
        // Mercado Pago pide los montos como texto con punto decimal, ej: "5000.00"
        string montoTexto = monto.ToString("F2", CultureInfo.InvariantCulture);

        var body = new
        {
            Type = "online",
            // En Checkout Pro va "manual": el pago lo hace el usuario en el link, no lo procesamos nosotros
            ProcessingMode = "manual",
            TotalAmount = montoTexto,
            ExternalReference = idPago.ToString(),
            Items = new[]
            {
                new { Title = descripcion, UnitPrice = montoTexto, Quantity = 1 }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "v1/orders");
        request.Content = JsonContent.Create(body, options: _jsonOptions);
        // Si Polly reintenta, se manda el mismo request con la misma clave,
        // y Mercado Pago no crea una orden nueva sino que devuelve la que ya creó
        request.Headers.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var orden = await response.Content.ReadFromJsonAsync<OrdenRespuesta>(_jsonOptions);
        return ConvertirOrden(orden!);
    }

    public async Task<OrdenMercadoPagoDTO> GetOrdenAsync(string idOrden)
    {
        var orden = await _httpClient.GetFromJsonAsync<OrdenRespuesta>($"v1/orders/{idOrden}", _jsonOptions);
        return ConvertirOrden(orden!);
    }

    private OrdenMercadoPagoDTO ConvertirOrden(OrdenRespuesta orden)
    {
        var dto = new OrdenMercadoPagoDTO();
        dto.Id = orden.Id;
        dto.CheckoutUrl = orden.CheckoutUrl ?? string.Empty;
        dto.Estado = orden.Status ?? string.Empty;
        dto.DetalleEstado = orden.StatusDetail ?? string.Empty;
        dto.ReferenciaExterna = orden.ExternalReference ?? string.Empty;
        // El medio de pago está dentro del primer pago de la orden, si ya se pagó
        dto.MedioPago = orden.Transactions?.Payments?.FirstOrDefault()?.PaymentMethod?.Id ?? string.Empty;

        return dto;
    }

    // Clases con la forma del JSON que devuelve Mercado Pago. Son privadas porque
    // solo se usan acá: el resto del proyecto trabaja con OrdenMercadoPagoDTO
    private class OrdenRespuesta
    {
        public string Id { get; set; } = string.Empty;
        public string? Status { get; set; }
        public string? StatusDetail { get; set; }
        public string? ExternalReference { get; set; }
        public string? CheckoutUrl { get; set; }
        public TransaccionesRespuesta? Transactions { get; set; }
    }

    private class TransaccionesRespuesta
    {
        public List<PagoRespuesta>? Payments { get; set; }
    }

    private class PagoRespuesta
    {
        public MedioPagoRespuesta? PaymentMethod { get; set; }
    }

    private class MedioPagoRespuesta
    {
        public string? Id { get; set; }
    }
}
