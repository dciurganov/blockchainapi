using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries
{
    public class GetAllBlockchainHistoryQuery : IRequest<PagedResult<BlockchainDataDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class GetAllBlockchainHistoryQueryHandler
        : IRequestHandler<GetAllBlockchainHistoryQuery, PagedResult<BlockchainDataDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllBlockchainHistoryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<BlockchainDataDto>> Handle(
            GetAllBlockchainHistoryQuery request,
            CancellationToken cancellationToken)
        {
            var repository = _unitOfWork.Repository<Domain.Entities.BlockchainData>();

            var query = repository.GetQueryable()
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
