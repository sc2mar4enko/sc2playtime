using System.Reflection;
using System.Text.RegularExpressions;
using IronPython.Hosting;
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
                    string fileName = _randomStringGenerator.GenerateRandomString() + Path.GetFileName(formFile.FileName);
                    string filePath = Path.Combine(newFolderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            return RedirectToAction("CalculatePlayTime", new { replaysPath = newFolderPath });
        }

        public IActionResult CalculatePlayTime(string replaysPath)
        {
            if (Directory.Exists(replaysPath))
            {
                var data = (0, 0);
                var replays = Directory.GetFiles(replaysPath);
                foreach (var file in replays)
                {
                    if (Path.GetExtension(file) == ".SC2Replay")
                    {
                        data.Item1 += 1;
                        data.Item2 += ReplayAnalysis.GameLengthReturning(file).Result;
                    }
                }
                Directory.Delete(replaysPath, true);

                return View(data);
            }
            return RedirectToAction("GetReplaysFiles");
        }
    }
}