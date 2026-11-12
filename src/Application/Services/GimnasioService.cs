using Application.Interfaces;
using Application.Models;
using Application.Models.Request;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class GimnasioService : IGimnasioService
{
    private readonly IGimnasioRepository _gimnasioRepository;

    public GimnasioService(IGimnasioRepository gimnasioRepository)
    {
        _gimnasioRepository = gimnasioRepository;
    }

    public async Task<List<GimnasioDTO>> GetActivos()
    {
        var gimnasioList = await _gimnasioRepository.GetActivosAsync();
        return GimnasioDTO.CreateList(gimnasioList);
    }

    // incluye los dados de baja (es para el superadmin)
    public async Task<List<GimnasioDTO>> GetAll()
    {
        var gimnasioList = await _gimnasioRepository.GetAllAsync();
        return GimnasioDTO.CreateList(gimnasioList);
    }

    public async Task<GimnasioDTO> GetById(int idGimnasio)
    {
        var gimnasio = await GetGimnasioActivo(idGimnasio);
        return GimnasioDTO.Create(gimnasio);
    }

    public async Task<GimnasioDTO> Create(int idSuperAdmin, CreationGimnasioDTO creationGimnasioDto)
    {
        var nuevoGimnasio = new Gimnasio();

        nuevoGimnasio.Nombre = creationGimnasioDto.Nombre;
        nuevoGimnasio.Direccion = creationGimnasioDto.Direccion;
        nuevoGimnasio.Telefono = creationGimnasioDto.Telefono;
        nuevoGimnasio.Email = creationGimnasioDto.Email;
        nuevoGimnasio.FechaAlta = DateTime.Now;
        nuevoGimnasio.Activo = true;
        nuevoGimnasio.SuperAdminId = idSuperAdmin;

        var gimnasioCreado = await _gimnasioRepository.CreateAsync(nuevoGimnasio);
        return GimnasioDTO.Create(gimnasioCreado);
    }

    public async Task Update(int idGimnasio, CreationGimnasioDTO creationGimnasioDto)
    {
        var gimnasio = await GetGimnasioActivo(idGimnasio);

        gimnasio.Nombre = creationGimnasioDto.Nombre;
        gimnasio.Direccion = creationGimnasioDto.Direccion;
        gimnasio.Telefono = creationGimnasioDto.Telefono;
        gimnasio.Email = creationGimnasioDto.Email;

        await _gimnasioRepository.UpdateAsync(gimnasio);
    }

    // Baja lógica para no perder reservas y solicitudes. También desactiva sus horarios y beneficios
    public async Task Delete(int idGimnasio)
    {
        var gimnasio = await _gimnasioRepository.GetByIdConHorariosYBeneficiosAsync(idGimnasio);

        if (gimnasio is null || !gimnasio.Activo)
        {
            throw new NotFoundException($"No se encontró el gimnasio con id {idGimnasio}");
        }

        gimnasio.Activo = false;

        foreach (var horario in gimnasio.Horarios)
        {
            horario.Activo = false;
        }

        foreach (var beneficio in gimnasio.Beneficios)
        {
            beneficio.Activo = false;
        }

        await _gimnasioRepository.UpdateAsync(gimnasio);
    }

    // 404 si no existe o está dado de baja
    private async Task<Gimnasio> GetGimnasioActivo(int idGimnasio)
    {
        var gimnasio = await _gimnasioRepository.GetByIdAsync(idGimnasio);

        if (gimnasio is null || !gimnasio.Activo)
        {
            throw new NotFoundException($"No se encontró el gimnasio con id {idGimnasio}");
        }

        return gimnasio;
    }
}
