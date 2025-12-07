using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands
{
    public class SyncBlockchainCommand : IRequest<BlockchainDataDto>
    {
        public BlockchainType BlockchainType { get; set; }
    }

    public class SyncBlockchainCommandHandler : IRequestHandler<SyncBlockchainCommand, BlockchainDataDto>
    {
        private readonly IBlockchainService _blockchainService;
        private readonly ILogger<SyncBlockchainCommandHandler> _logger;

        public SyncBlockchainCommandHandler(
            IBlockchainService blockchainService,
            ILogger<SyncBlockchainCommandHandler> logger)
        {
            _blockchainService = blockchainService;
            _logger = logger;
        }

        public async Task<BlockchainDataDto> Handle(SyncBlockchainCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executing sync command for {BlockchainType}", request.BlockchainType);
            return await _blockchainService.SyncBlockchainDataAsync(request.BlockchainType, cancellationToken);
        }
    }
}
