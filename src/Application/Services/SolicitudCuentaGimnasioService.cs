using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

public class SolicitudCuentaGimnasioService : ISolicitudCuentaGimnasioService
{
    private readonly ISolicitudCuentaGimnasioRepository _solicitudCuentaGimnasioRepository;

    public SolicitudCuentaGimnasioService(ISolicitudCuentaGimnasioRepository solicitudCuentaGimnasioRepository)
    {
        _solicitudCuentaGimnasioRepository = solicitudCuentaGimnasioRepository;
    }
}
