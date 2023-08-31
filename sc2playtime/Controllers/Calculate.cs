using IronPython.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Scripting.Hosting;
using sc2playtime.Helpers;
using s2protocol;
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
                var engine = Calculating(replaysPath, out var data);

                Directory.Delete(replaysPath, true);
                engine.Runtime.Shutdown();
                return View(data);
            }
            return RedirectToAction("GetReplaysFiles");
        }

        private static ScriptEngine Calculating(string replaysPath, out dynamic data)
        {
            var engine = Python.CreateEngine();
            var searchPaths = engine.GetSearchPaths();
            searchPaths.Add(Path.Combine(Directory.GetCurrentDirectory(), @"scripts\Lib\site-packages"));
            engine.SetSearchPaths(searchPaths);

            var scope = engine.CreateScope();
            scope.SetVariable("replaysPath", replaysPath);

            var source =
                engine.CreateScriptSourceFromFile(Path.Combine(Directory.GetCurrentDirectory(), @"scripts\replays.py"));

            var compilation = source.Compile();
            var result = compilation.Execute(scope);

            data = null!;
            foreach (var function in scope.GetVariableNames())
            {
                if (function == "time")
                    data = scope.GetVariable(function)();
            }

            return engine;
        }
    }
}