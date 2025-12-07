using Application.DTOs;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries
{
    public class GetBlockchainHistoryQuery : IRequest<PagedResult<BlockchainDataDto>>
    {
        public BlockchainType BlockchainType { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class GetBlockchainHistoryQueryHandler
        : IRequestHandler<GetBlockchainHistoryQuery, PagedResult<BlockchainDataDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetBlockchainHistoryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<BlockchainDataDto>> Handle(
            GetBlockchainHistoryQuery request,
            CancellationToken cancellationToken)
        {
            var repository = _unitOfWork.Repository<Domain.Entities.BlockchainData>();

            var query = repository.GetQueryable()
                .Where(x => x.BlockchainType == request.BlockchainType.ToString())
                .OrderByDescending(x => x.CreatedAt);

            var totalCount = query.Count();

            var items = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new BlockchainDataDto
                {
                    Id = x.Id,
                    BlockchainType = x.BlockchainType,
                    Name = x.Name,
                    Height = x.Height,
                    Hash = x.Hash,
                    Time = x.Time,
                    CreatedAt = x.CreatedAt,
                    PeerCount = x.PeerCount,
                    UnconfirmedCount = x.UnconfirmedCount
                })
                .ToList();

            return new PagedResult<BlockchainDataDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
