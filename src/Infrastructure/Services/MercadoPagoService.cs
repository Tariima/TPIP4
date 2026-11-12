using Application.Interfaces;
using Application.Models;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Infrastructure.Services;

// Cliente de la API de Orders de MP (Checkout Pro).
// El HttpClient viene configurado desde Program.cs (url, token y Polly)
public class MercadoPagoService : IMercadoPagoService
{
    private readonly HttpClient _httpClient;

    // MP usa snake_case (total_amount) y nosotros PascalCase, con esto se convierte solo
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
        // MP pide el monto como string, ej: "5000.00"
        string montoTexto = monto.ToString("F2", CultureInfo.InvariantCulture);

        var body = new
        {
            Type = "online",
            // manual: el pago lo hace el usuario en el checkout de MP
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
        // misma clave en los reintentos de Polly, así MP devuelve la orden que ya creó en vez de duplicarla
        request.Headers.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var orden = await response.Content.ReadFromJsonAsync<OrdenRespuesta>(_jsonOptions);
        return ConvertirOrden(orden!);
    }

    // null si la orden no existe (MP da 404, o 400 si el id es inválido). Otros errores siguen de largo
    public async Task<OrdenMercadoPagoDTO?> GetOrdenAsync(string idOrden)
    {
        var response = await _httpClient.GetAsync($"v1/orders/{idOrden}");

        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var orden = await response.Content.ReadFromJsonAsync<OrdenRespuesta>(_jsonOptions);
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
        // el medio de pago viene en el primer pago de la orden (si ya se pagó)
        dto.MedioPago = orden.Transactions?.Payments?.FirstOrDefault()?.PaymentMethod?.Id ?? string.Empty;

        return dto;
    }

    // Para deserializar el JSON de MP. Privadas porque afuera se usa OrdenMercadoPagoDTO
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
