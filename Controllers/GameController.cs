using EnumsNET;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private IdentityContext _context;
        public GameController(UnoMatchesStorageService matchesStorageService,
            IdentityContext context)
        {
            _matchesStorageService = matchesStorageService;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPost("move")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Move([FromBody]MoveRequest request)
        {
            UnoMatch match = _matchesStorageService.FindMatch(request.MatchId);
            if (match == null)
            {
                return BadRequest(new MoveErrorResponse 
                { 
                    ErrorCode = MoveErrorCode.MATCH_NOT_FOUND,
                    Description = MoveErrorCode.MATCH_NOT_FOUND.AsString(EnumFormat.Description) 
                });
            }

            if (match.CurrentPlayer != request.Token)
            {
                return BadRequest(new MoveErrorResponse
                {
                    ErrorCode = MoveErrorCode.NOT_YOUR_MOVE,
                    Description = MoveErrorCode.NOT_YOUR_MOVE.AsString(EnumFormat.Description)
                });
            }

            var responseStatus = match.Move(request.Cards, request.Color, _context);
            switch (responseStatus)
            {
                case MoveStatus.FAKE_EMPTY_MOVE:
                    return BadRequest(new MoveErrorResponse
                    {
                        ErrorCode = MoveErrorCode.FAKE_EMPTY_MOVE,
                        Description = MoveErrorCode.FAKE_EMPTY_MOVE.AsString(EnumFormat.Description)
                    });
                case MoveStatus.WRONG_MOVE:
                    return BadRequest(new MoveErrorResponse
                    {
                        ErrorCode = MoveErrorCode.WRONG_MOVE,
                        Description = MoveErrorCode.WRONG_MOVE.AsString(EnumFormat.Description)
                    });
                case MoveStatus.ENDGAME:
                    _matchesStorageService.FinishMatch(match.Id);
                    return Ok();
            }

            await _context.SaveChangesAsync();

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

    public class MoveErrorResponse
    {
        public MoveErrorCode ErrorCode { get; set; }
        public string Description { get; set; }
    }

    public enum MoveErrorCode
    {
        [Description("Игра не найдена")]
        MATCH_NOT_FOUND = 0,
        [Description("Невалидный код")]
        WRONG_MOVE = 1,
        [Description("Не сделан ход, хотя есть возможность")]
        FAKE_EMPTY_MOVE,
        [Description("Ход другого пользователя")]
        NOT_YOUR_MOVE
    }
}
