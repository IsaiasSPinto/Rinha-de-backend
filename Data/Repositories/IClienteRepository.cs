using Rinha_de_backend.Dtos;

namespace Rinha_de_backend.Data.Repositories;

public interface IClienteRepository
{
    public Task<ClienteDto> GetCliente(int clienteId);

}
