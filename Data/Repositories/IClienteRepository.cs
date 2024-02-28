using Rinha_de_backend.Dtos;

namespace Rinha_de_backend.Data.Repositories;

public interface IClienteRepository
{
    public Task<Result<ClienteDto>> GetCliente(int clienteId);

}
