using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Domain;
using Firebase.Storage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace API.Controllers
{
    /// <summary>
    /// Controller for upload image
    /// </summary>
    [Authorize]
    public class ImagesController : BaseApiController
    {
        private readonly ILogger<ImagesController> _logger;
        private readonly IConfiguration _configuration;

        public ImagesController(ILogger<ImagesController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // Keer, Biker, Admin
        /// <summary>
        /// Action for upload large file
        /// </summary>
        /// <remarks>
        /// Request to this action will not trigger any model binding or model validation,
        /// because this is a no-argument action
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadImage()
        {
            _logger.LogInformation("Start uploading images to Firebase");
            var ct = HttpContext.RequestAborted;
            var request = HttpContext.Request;

            // validation of Content-Type
            // 1. first, it must be a form-data request
            // 2. a boundary should be found in the Content-Type
            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return new UnsupportedMediaTypeResult();
            }

            var reader = new MultipartReader(mediaTypeHeader.Boundary.Value, request.Body);
            var section = await reader.ReadNextSectionAsync(ct);
            List<string> downloadUrl = new();
            List<string> pathList = new();

            // Get the all file from request and save it
            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out var contentDisposition);

                if (contentDisposition != null && hasContentDispositionHeader &&
                    contentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
                    var fileName = "image_" + CurrentTime.GetCurrentTime().ToString("yyyyMMdd_HHmmss_ffffff") + ".jpg";
                    var saveToPath = Path.Combine(Path.GetTempPath(), fileName);
                    pathList.Add(saveToPath);
                    await using var targetStream = System.IO.File.Create(saveToPath);
                    await section.Body.CopyToAsync(targetStream, ct);
                }

                section = await reader.ReadNextSectionAsync(ct);
            }

            foreach (var path in pathList)
            {
                await using (var stream = System.IO.File.Open(path, FileMode.Open))
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");

                    // Construct FirebaseStorage, path to where you want to upload the file and Put it there
                    var task = new FirebaseStorage(_configuration["Firebase:BucketPath"],
                            new FirebaseStorageOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(accessToken), ThrowOnCancel = true,
                            }).Child(path.Split(@"\").Last())
                        .PutAsync(stream);

                    // await the task to wait until upload completes and get the download url
                    downloadUrl.Add(await task);
                }

                // Delete file after upload
                System.IO.File.Delete(path);
            }

            _logger.LogInformation("End uploading images to Firebase");
            return HandleResult(Result<List<string>>.Success(downloadUrl, "Successfully upload images to Firebase."));
        }
    }
}