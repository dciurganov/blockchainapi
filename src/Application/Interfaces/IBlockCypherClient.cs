using Application.DTOs;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface IBlockCypherClient
    {
        Task<BlockchainApiDto> GetBlockchainDataAsync(
            BlockchainType blockchainType,
            CancellationToken cancellationToken = default);
    }
}
