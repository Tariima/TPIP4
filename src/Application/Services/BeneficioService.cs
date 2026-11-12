using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class BeneficioService : IBeneficioService
{
    private readonly IBeneficioRepository _beneficioRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IGimnasioRepository _gimnasioRepository;
    private readonly ISuscripcionRepository _suscripcionRepository;

    public BeneficioService(IBeneficioRepository beneficioRepository, IUsuarioRepository usuarioRepository, IGimnasioRepository gimnasioRepository, ISuscripcionRepository suscripcionRepository)
    {
        _beneficioRepository = beneficioRepository;
        _usuarioRepository = usuarioRepository;
        _gimnasioRepository = gimnasioRepository;
        _suscripcionRepository = suscripcionRepository;
    }

    public async Task<List<BeneficioDTO>> GetAllDelGimnasio(int idAdmin)
    {
        int gimnasioId = await GetGimnasioDelAdmin(idAdmin);
        var beneficioList = await _beneficioRepository.GetActivosByGimnasioAsync(gimnasioId);

        // los que vencen antes, arriba
        return BeneficioDTO.CreateList(beneficioList.OrderBy(b => b.VigenteHasta).ToList());
    }

    public async Task<BeneficioDTO> GetById(int idAdmin, int idBeneficio)
    {
        int gimnasioId = await GetGimnasioDelAdmin(idAdmin);
        var beneficio = await GetBeneficioDelGimnasio(gimnasioId, idBeneficio);

        return BeneficioDTO.Create(beneficio);
    }

    public async Task<BeneficioDTO> Create(int idAdmin, CreationBeneficioDTO creationBeneficioDto)
    {
        int gimnasioId = await GetGimnasioDelAdmin(idAdmin);

        // no tiene sentido cargar uno que ya terminó
        if (creationBeneficioDto.VigenteHasta!.Value.Date < DateTime.Today)
        {
            throw new ValidationException("La fecha de fin de vigencia no puede ser anterior a hoy");
        }

        var nuevoBeneficio = new Beneficio();

        CargarDatos(nuevoBeneficio, creationBeneficioDto);
        nuevoBeneficio.Activo = true;
        nuevoBeneficio.GimnasioId = gimnasioId;

        var beneficioCreado = await _beneficioRepository.CreateAsync(nuevoBeneficio);
        return BeneficioDTO.Create(beneficioCreado);
    }

    public async Task Update(int idAdmin, int idBeneficio, CreationBeneficioDTO creationBeneficioDto)
    {
        int gimnasioId = await GetGimnasioDelAdmin(idAdmin);
        var beneficio = await GetBeneficioDelGimnasio(gimnasioId, idBeneficio);

        CargarDatos(beneficio, creationBeneficioDto);

        await _beneficioRepository.UpdateAsync(beneficio);
    }

    // Baja lógica, queda como historial del gimnasio
    public async Task Delete(int idAdmin, int idBeneficio)
    {
        int gimnasioId = await GetGimnasioDelAdmin(idAdmin);
        var beneficio = await GetBeneficioDelGimnasio(gimnasioId, idBeneficio);

        beneficio.Activo = false;

        await _beneficioRepository.UpdateAsync(beneficio);
    }

    // Solo Pro
    public async Task<List<GimnasioConBeneficiosDTO>> GetVigentes(int idUsuario)
    {
        if (!await _suscripcionRepository.TieneSuscripcionActivaAsync(idUsuario))
        {
            throw new ForbiddenException("Los beneficios en gimnasios son solo para usuarios Pro");
        }

        var beneficioList = await _beneficioRepository.GetVigentesAsync(DateTime.Today);

        // un item por gimnasio con sus beneficios
        return beneficioList
            .GroupBy(b => b.Gimnasio)
            .OrderBy(g => g.Key.Nombre)
            .Select(g => new GimnasioConBeneficiosDTO
            {
                GimnasioId = g.Key.Id,
                Nombre = g.Key.Nombre,
                Direccion = g.Key.Direccion,
                Telefono = g.Key.Telefono,
                Beneficios = BeneficioDTO.CreateList(g.OrderBy(b => b.VigenteHasta).ToList())
            })
            .ToList();
    }

    // Validaciones comunes de Create y Update
    private void CargarDatos(Beneficio beneficio, CreationBeneficioDTO creationBeneficioDto)
    {
        // .Value es seguro, el [Required] ya validó que vengan
        float porcentajeDescuento = creationBeneficioDto.PorcentajeDescuento!.Value;
        DateTime vigenteDesde = creationBeneficioDto.VigenteDesde!.Value.Date;
        DateTime vigenteHasta = creationBeneficioDto.VigenteHasta!.Value.Date;

        // 0 vale para cosas tipo "primera clase gratis"
        if (porcentajeDescuento < 0 || porcentajeDescuento > 100)
        {
            throw new ValidationException("El porcentaje de descuento tiene que estar entre 0 y 100");
        }

        if (vigenteDesde > vigenteHasta)
        {
            throw new ValidationException("La fecha de inicio de vigencia no puede ser posterior a la de fin");
        }

        beneficio.Titulo = creationBeneficioDto.Titulo;
        beneficio.Descripcion = creationBeneficioDto.Descripcion;
        beneficio.Tipo = creationBeneficioDto.Tipo;
        beneficio.PorcentajeDescuento = porcentajeDescuento;
        beneficio.VigenteDesde = vigenteDesde;
        beneficio.VigenteHasta = vigenteHasta;
    }

    // El gimnasio sale de la cuenta del admin, nunca del request (igual que en horarios)
    private async Task<int> GetGimnasioDelAdmin(int idAdmin)
    {
        var admin = await _usuarioRepository.GetByIdAsync(idAdmin) as AdminGimnasio;

        if (admin is null)
        {
            throw new NotFoundException("No se encontró la cuenta del admin");
        }

        var gimnasio = await _gimnasioRepository.GetByIdAsync(admin.GimnasioId);

        if (gimnasio is null || !gimnasio.Activo)
        {
            throw new ConflictException("El gimnasio está dado de baja");
        }

        return gimnasio.Id;
    }

    // 404 también si es de otro gimnasio, para no dar pistas de que existe
    private async Task<Beneficio> GetBeneficioDelGimnasio(int gimnasioId, int idBeneficio)
    {
        var beneficio = await _beneficioRepository.GetByIdAsync(idBeneficio);

        if (beneficio is null || !beneficio.Activo || beneficio.GimnasioId != gimnasioId)
        {
            throw new NotFoundException($"No se encontró el beneficio con id {idBeneficio}");
        }

        return beneficio;
    }
}
