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
    public class GameLogsModel : PageModel
    {
        private UnoMatchesStorageService _matchesStorageService;

        public GameLogsModel(UnoMatchesStorageService matchesStorageService)
        {
            _matchesStorageService = matchesStorageService ?? throw new ArgumentNullException(nameof(matchesStorageService));
        }

        public List<UnoMatchDetails> GetMatches()
        {
            return _matchesStorageService.GetPassedMatches();
        }

        public void OnGet()
        {
        }
    }
}
