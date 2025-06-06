using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace AdnTestingSystem.Core.Utils;

public static class CoreHelper
{
    public static DateTimeOffset SystemTimeNow => TimeHelper.ConvertToUtcPlus7(DateTimeOffset.Now);

    public static async Task<IFormFile?> InstallImage(string imageUrl)
    {
        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            return null;
        }

        using HttpClient client = new HttpClient();

        try
        {
            // Download the image data as a byte array
            byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);

            using (var memoryStream = new MemoryStream(imageBytes))
            {
                IFormFile file = new FormFile(memoryStream, 0, memoryStream.Length, null, "image.png")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png"
                };
                return file;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }

        return null;
    }
}