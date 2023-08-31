using Microsoft.AspNetCore.Mvc;
using sc2playtime.Helpers;
using sc2playtime.scripts;

namespace sc2playtime.Controllers
{
    public class Calculate : Controller
    {
        private readonly IRandomStringGenerator _randomStringGenerator;

        public Calculate(IRandomStringGenerator randomStringGenerator)
        {
            _randomStringGenerator = randomStringGenerator;
        }

        [HttpGet]
        public IActionResult GetReplaysFiles()
        {
            return View();
        }

        [DisableRequestSizeLimit]
        [RequestFormLimits(ValueCountLimit = 10000)]
        [HttpPost]
        public async Task<IActionResult> PostReplaysFiles()
        {
            string folderName = Path.GetRandomFileName();
            string currentDirectory = Directory.GetCurrentDirectory();
            string newFolderPath = Path.Combine(currentDirectory, folderName);

            if (!Directory.Exists(newFolderPath))
            {
                Directory.CreateDirectory(newFolderPath);
            }

            foreach (var formFile in HttpContext.Request.Form.Files)
            {
                if (formFile.Length > 0)
                {
                    string fileName = _randomStringGenerator.GenerateRandomString() +
                                      Path.GetFileName(formFile.FileName);
                    string filePath = Path.Combine(newFolderPath, fileName);

                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await formFile.CopyToAsync(stream);
                }
            }

            return RedirectToAction("CalculatePlayTime", new { replaysPath = newFolderPath });
        }

        public IActionResult CalculatePlayTime(string replaysPath)
        {
            if (Directory.Exists(replaysPath))
            {
                var data = ReplayAnalysis.GameLengthReturning(replaysPath).Result;
                var playtime = new Playtime(data.Item1, data.Item2);
                Directory.Delete(replaysPath, true);
                return View(playtime);
            }

            return RedirectToAction("GetReplaysFiles");
        }
    }

    public class Playtime
    {
        public Playtime(int games, double hours)
        {
            Games = games;
            Hours = hours;
        }

        private int Games { get; set; }

        private double Hours { get; set; }

        public int GetGames()
        {
            return Games;
        }

        public double GetHours()
        {
            var hours = (Hours / 3600) * 0.7138;
            return Math.Round(hours, 1);
        }
    }
}