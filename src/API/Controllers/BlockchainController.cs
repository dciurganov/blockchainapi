using Application.Commands;
using Application.Queries;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlockchainController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BlockchainController> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BlockchainController(IMediator mediator, ILogger<BlockchainController> logger, IServiceProvider serviceProvider)
        {
            _mediator = mediator;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Synchronize data for a specific blockchain
        /// </summary>
        [HttpPost("sync/{blockchainType}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SyncBlockchain([FromRoute] BlockchainType blockchainType)
        {
            _logger.LogInformation("Syncing blockchain: {BlockchainType}", blockchainType);

            var command = new SyncBlockchainCommand { BlockchainType = blockchainType };
            var result = await _mediator.Send(command);

            return Ok(new { Message = $"Successfully synced {blockchainType}", Data = result });
        }

        /// <summary>
        /// Synchronize all blockchains in parallel
        /// </summary>
        [HttpPost("sync-all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SyncAllBlockchains()
        {
            _logger.LogInformation("Syncing all blockchains");

            var tasks = Enum.GetValues<BlockchainType>()
                .Select(async type =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var scopedMediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    return await scopedMediator.Send(new SyncBlockchainCommand { BlockchainType = type });
                });

            var results = await Task.WhenAll(tasks);

            return Ok(new { Message = "All blockchains synced", Count = results.Length });
        }

        /// <summary>
        /// Get historical data for a specific blockchain
        /// </summary>
        [HttpGet("{blockchainType}/history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBlockchainHistory(
            [FromRoute] BlockchainType blockchainType,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            var query = new GetBlockchainHistoryQuery
            {
                BlockchainType = blockchainType,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Get all blockchain historical data
        /// </summary>
        [HttpGet("all-history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllHistory(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            var query = new GetAllBlockchainHistoryQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        //[HttpGet(Name = "GetWeatherForecast")]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}
    }
}
