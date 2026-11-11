using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

public interface ISolicitudCuentaGimnasioRepository : IRepositoryBase<SolicitudCuentaGimnasio>
{
    Task<List<SolicitudCuentaGimnasio>> GetConGimnasioAsync(EstadoSolicitud? estado);
    Task<SolicitudCuentaGimnasio?> GetByIdConGimnasioAsync(int id);
}
