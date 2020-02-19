using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using lu.Models;
using lu.Interfaces;

namespace lu
{
    public class ImageProcess
    {
        private IAuthenticationService _authService;
        private IImageService _imageService;

        public ImageProcess(IImageService imageService, IAuthenticationService authService)
        {
            _imageService = imageService;
            _authService = authService;
        }

        [FunctionName("lu_image_service")]
        public async Task<IActionResult> Upload(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Upload")] HttpRequest req,
            ILogger log)
        {
            // var value = Environment.GetEnvironmentVariable("BlobStorageAccessKey");
            // var conString = Environment.GetEnvironmentVariable("BlobStorageConnectionString");
            // var imageContainer = Environment.GetEnvironmentVariable("ImageContainer");
            // var thumbnailContainer = Environment.GetEnvironmentVariable("ThumbnailContainer");

            // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            // if (requestBody == null || requestBody.Length < 1)
            // {
            //     return (ActionResult)new OkObjectResult($"value, {value}, and constring, {conString}");
            // }

            // byte[] data = JsonConvert.DeserializeObject<byte[]>(requestBody);

            // await ImageHandler.UploadImage(conString, imageContainer, data, false);
            // await ImageHandler.UploadImage(conString, thumbnailContainer, data, false);

            // return (ActionResult)new OkObjectResult($"value, {value}, and constring, {conString}");

            var data = await ReadRequestBodyAsync<ImageResult>(req);

            if (data == null
                || data.id == 0
                || data.token.Length < 1
                || data.file == null)
            {
                return new BadRequestObjectResult("Neccessary data was not provided");
            }

            var isValidated = await _authService.ValidateToken(data.token);

            if (!isValidated)
            {
                return new BadRequestObjectResult("Token could not be validated. Please try again");
            }

            await _imageService.UploadAsync(data.id, data.file, false);
            await _imageService.UploadAsync(data.id, data.file, true);

            return (ActionResult)new OkObjectResult($"UserID: {data.id}");
        }

        private async Task<T> ReadRequestBodyAsync<T>(HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>(requestBody);
        }
    }
}
