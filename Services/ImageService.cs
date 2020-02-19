using System;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;
using lu.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace lu.Services
{
    public class ImageService : IImageService
    {
        public async Task UploadAsync(int id, IFormFile file, bool isThumbnail)
        {
            int fileNameStartLocation = file.FileName.LastIndexOf("\\") + 1;
            string fileName = file.FileName.Substring(fileNameStartLocation);

            if (isThumbnail)
            {
                fileName = $"thumbnail-{fileName}";
            }

            byte[] fileBytes;

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);

                fileBytes = ms.ToArray();
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable(Constants.BLOB_STORAGE_CONNECTIONSTRING));

            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(id.ToString());

            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            MagickImage image = new MagickImage(fileBytes);
            image.AutoOrient();

            if (isThumbnail)
            {
                var thumbnailBytes = CreateThumbNail(image);
                await blockBlob.UploadFromByteArrayAsync(thumbnailBytes, 0, thumbnailBytes.Length);
            }
            else
            {
                byte[] bytes = image.ToByteArray();
                await blockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
            }
        }

        private static byte[] CreateThumbNail(MagickImage image)
        {
            image.Resize(125, 125);
            return image.ToByteArray();
        }
    }
}