using ImageMagick;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

public class ImageHandler
{
    public static async Task UploadImage(string storageConnectionString, string containerName, byte[] fileBytes, bool isThumbnail)
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

        var blobClient = storageAccount.CreateCloudBlobClient();
        var container = blobClient.GetContainerReference(containerName);

        await container.CreateIfNotExistsAsync();
        await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

        CloudBlockBlob blockBlob = container.GetBlockBlobReference("test"); //replace with userId

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