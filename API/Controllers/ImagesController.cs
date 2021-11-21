using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Domain;
using Domain.Enums;
using Firebase.Storage;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Action for upload images to Firebase.
        /// </summary>
        /// <param name="imageUploadDto">This Dto contains an int ImageType
        /// and a list of IFormFile which is list of images.</param>
        /// <returns>List of download url string.</returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage([FromForm] ImageUploadDto imageUploadDto)
        {
            _logger.LogInformation("Start uploading images to Firebase");
            var ct = HttpContext.RequestAborted;
            var request = HttpContext.Request;

            // Validation of Content-Type
            // 1. First, it must be a form-data request
            // 2. A boundary should be found in the Content-Type
            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return new UnsupportedMediaTypeResult();
            }

            if (imageUploadDto.ImageList!.Count == 0)
            {
                _logger.LogInformation("No image found");
                return HandleResult(Result<List<string>>.Failure("No image found."));
            }

            if (imageUploadDto.ImageList.Any(image => image.Length > 5242880))
            {
                _logger.LogInformation("Image size is too large, must be less than 5MB");
                return HandleResult(Result<List<string>>.Failure("Image size is too large, must be less than 5MB."));
            }

            List<string> downloadUrl = new();

            foreach (var image in imageUploadDto.ImageList)
            {
                string extension = image.ContentType.Split(@"/").Last();
                switch (extension)
                {
                    case "png":
                        extension = ".png";
                        break;
                    default:
                        extension = ".jpg";
                        break;
                }

                string folderName;
                switch (imageUploadDto.ImageType)
                {
                    case (int) ImageType.Bike:
                        folderName = "bike";
                        break;
                    case (int) ImageType.User:
                        folderName = "user";
                        break;
                    case (int) ImageType.Voucher:
                        folderName = "voucher";
                        break;
                    case (int) ImageType.Advertisement:
                        folderName = "advertisement";
                        break;
                    default:
                        _logger.LogInformation("No image type found");
                        return HandleResult(Result<List<string>>.Failure("No image type found."));
                }

                var accessToken = await HttpContext.GetTokenAsync("access_token");
                try
                {
                    FirebaseToken _ = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(accessToken, ct);
                }
                catch (FirebaseAuthException e)
                {
                    _logger.LogInformation("{Error}", e.InnerException?.Message ?? e.Message);
                    return HandleResult(Result<List<string>>.Failure(e.InnerException?.Message ?? e.Message));
                }

                // ReSharper disable StringLiteralTypo
                var fileName = "image_" + CurrentTime.GetCurrentTime().ToString("yyyyMMdd_HHmmss_ffffff") + extension;
                // ReSharper restore StringLiteralTypo
                // Construct FirebaseStorage, path to where you want to upload the file and Put it there
                var task = new FirebaseStorage(_configuration["Firebase:BucketPath"],
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(accessToken), ThrowOnCancel = true
                        }).Child(folderName)
                    .Child(fileName)
                    .PutAsync(image.OpenReadStream(), ct);

                try
                {
                    downloadUrl.Add(await task);
                }
                catch (Exception e)
                {
                    _logger.LogInformation("{Error}", e.InnerException?.Message ?? e.Message);
                    return HandleResult(Result<List<string>>.Failure(e.InnerException?.Message ?? e.Message));
                }
            }

            _logger.LogInformation("End uploading images to Firebase");
            return HandleResult(Result<List<string>>.Success(downloadUrl, "Successfully upload images to Firebase."));
        }
    }

    public class ImageUploadDto
    {
        [Required]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public int? ImageType { get; init; }

        [Required]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public List<IFormFile>? ImageList { get; init; }
    }
}