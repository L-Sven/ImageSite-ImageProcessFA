using Microsoft.AspNetCore.Http;

namespace lu.Models
{
    public class ImageResult
    {
        public int id { get; set; }
        public string token { get; set; }
        public IFormFile file { get; set; }
    }
}