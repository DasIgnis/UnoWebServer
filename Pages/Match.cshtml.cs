using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnoServer.Models;
using UnoServer.Services;

namespace UnoServer.Pages
{
    public class MatchModel : PageModel
    {
        public Guid Id { get; set; }
        public UnoMatch CurrentMatch { get; set; }

        private UnoMatchesStorageService _matchesStorageService;

        public MatchModel(UnoMatchesStorageService matchesStorageService)
        {
            _matchesStorageService = matchesStorageService ?? throw new ArgumentNullException(nameof(matchesStorageService));
        }

        public void OnGet(Guid id)
        {
            Id = id;
            CurrentMatch = _matchesStorageService.FindMatch(Id);
        }
    }
}
