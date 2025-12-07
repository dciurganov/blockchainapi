using Application.DTOs;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface IBlockchainService
    {
        Task<BlockchainDataDto> SyncBlockchainDataAsync(
            BlockchainType blockchainType,
            CancellationToken cancellationToken = default);
    }
}
