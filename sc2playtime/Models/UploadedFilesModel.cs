using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace sc2playtime.Models
{
    public class PlayTimeModel
    {
        public List<IFormFile> AccountFolder { get; set; }
    }
}
