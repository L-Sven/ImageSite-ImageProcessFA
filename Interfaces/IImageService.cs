using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace lu.Interfaces
{
    public interface IImageService
    {
        Task UploadAsync(int id, IFormFile file, bool isThumbnail);
    }
}