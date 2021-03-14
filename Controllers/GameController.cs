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
        private UnoMatchesStorageService _matchesStorageService;
        public GameController(UnoMatchesStorageService matchesStorageService)
        {
            _matchesStorageService = matchesStorageService;
        }

        [HttpGet("board")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BoardResponse))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Board([FromQuery]BoardRequest request)
        {
            UnoMatch match = _matchesStorageService.FindMatch(request.MatchId);
            if (match == null)
            {
                return NotFound("Match not found");
            }

            BoardResponse response = new BoardResponse
            {
                MatchId = match.Id,
                //TODO: check winning status
                Status = GameStatus.InProcess,
                MyMove = match.CurrentPlayer.Equals(request.Token),
                Hand = match.GetHand(request.Token),
                CurrentCard = match.GetCurrentCard()
            };

            return Ok(response);
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
        public bool MyMove { get; set; }
        public List<UnoCard> Hand { get; set; }
        public UnoCard CurrentCard { get; set; }
        public UnoCardColor CurrentColor { get; set; }
        public BoardResponse()
        {
            Hand = new List<UnoCard>();
        }
    }

    public class MoveRequest
    {
        public Guid Token { get; set; }
        public Guid MatchId { get; set; }
        public List<UnoCard> Cards { get; set; }
        public UnoCardColor Color { get; set; }
        public MoveRequest()
        {
            Cards = new List<UnoCard>();
        }
    }

    public enum GameStatus
    {
        InProcess = 0,
        Win = 1,
        Lose
    }
}
