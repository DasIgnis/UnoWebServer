using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnoServer.Models;
using UnoServer.Services;

namespace UnoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private IMatchesStorageService _matchesStorageService;
        public GameController(UnoMatchesStorageService matchesStorageService)
        {
            _matchesStorageService = matchesStorageService;
        }

        [HttpGet("board")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BoardResponse))]
        public async Task<IActionResult> Board([FromQuery]BoardRequest request)
        {
            return Ok();
        }

        [HttpPost("move")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Move([FromBody]MoveRequest request)
        {
            return Ok();
        }
    }

    public class BoardRequest
    {
        public Guid Token { get; set; }
        public Guid MatchId { get; set; }
    }

    public class BoardResponse
    {
        public Guid MatchId { get; set; }
        public GameStatus Status { get; set; }
        public Guid CurrentPlayerId { get; set; }
        public List<UnoCard> Hand { get; set; }
        public UnoCard CurrentCard { get; set; }
    }

    public class MoveRequest
    {
        public Guid Token { get; set; }
        public Guid MatchId { get; set; }
        public UnoCard Card { get; set; }
    }

    public enum GameStatus
    {
        InProcess = 0,
        Win = 1,
        Lose
    }
}
