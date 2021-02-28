using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnoServer.Services;

namespace UnoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private UnoMatchesStorageService _matchesStorageService;
        public MatchController(UnoMatchesStorageService matchesStorageService)
        {
            _matchesStorageService = matchesStorageService ?? throw new ArgumentNullException(nameof(matchesStorageService));
        }

        /// <summary>
        /// Get token for authentication
        /// </summary>
        /// <returns></returns>
        [HttpGet("token")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MatchTokenResponse))]
        public async Task<IActionResult> MatchToken()
        {
            Guid guid = Guid.NewGuid();

            _matchesStorageService.Enqueue(guid);

            return Ok(new MatchTokenResponse
            {
                Token = guid
            });
        }

        /// <summary>
        /// Get match id for current user
        /// </summary>
        /// <param name="request">token - user id got by /token request</param>
        /// <returns>Status = 0 if not matched, Status = 1 and matchId if matched</returns>
        [HttpPost("start")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(StartMatchResponse))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> StartMatch([FromBody]StartMatchRequest request)
        {
            var matchId = _matchesStorageService.TryStartMatch(request.Token);

            if (!matchId.HasValue && !_matchesStorageService.IsEnqued(request.Token))
            {
                return NotFound("User GUID not found");
            }

            StartMatchResponse response = new StartMatchResponse {
                Status = StartMatchResponseStatus.Queued,
                MatchId = Guid.Empty
            };

            if (matchId.HasValue)
            {
                response.Status = StartMatchResponseStatus.Matched;
                response.MatchId = matchId.Value;
            }

            return Ok(response);
        }
    }

    public class MatchTokenResponse
    {
        public Guid Token { get; set; }
    }

    public class StartMatchRequest
    {
        public Guid Token { get; set; }
    }

    public class StartMatchResponse
    {
        public Guid MatchId { get; set; }
        public StartMatchResponseStatus Status { get; set; }
    }

    public enum StartMatchResponseStatus
    {
        Queued = 0,
        Matched = 1
    }
}
