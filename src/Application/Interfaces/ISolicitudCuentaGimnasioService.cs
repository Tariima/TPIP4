using Application.Models;
using Application.Models.Request;

namespace Application.Interfaces;

public interface ISolicitudCuentaGimnasioService
{
    Task<SolicitudCuentaGimnasioDTO> Create(CreationSolicitudCuentaGimnasioDTO creationSolicitudDto);
    Task<List<SolicitudCuentaGimnasioDTO>> GetAll(string? estado);
    Task<SolicitudCuentaGimnasioDTO> GetById(int idSolicitud);
    Task Aprobar(int idSuperAdmin, int idSolicitud);
    Task Rechazar(int idSuperAdmin, int idSolicitud);
}
