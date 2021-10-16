using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Firebase.Auth;
using Firebase.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Images
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ImageUpload
    {
        public class Command : IRequest<Result<Unit>>
        {
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IConfiguration _configuration;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IConfiguration configuration, IMapper mapper, ILogger<Handler> logger)
            {
                _context = context;
                _configuration = configuration;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    // Get any Stream - it can be FileStream, MemoryStream or any other type of Stream
                    string downloadUrl;
                    await using (var stream = File.Open(@"F:\Pictures\Ina (1).png", FileMode.Open))
                    {
                        //authentication
                        var auth = new FirebaseAuthProvider(new FirebaseConfig(_configuration["Firebase:WebApiKey"]));
                        var a = await auth.SignInAnonymouslyAsync();

                        // Construct FirebaseStorage, path to where you want to upload the file and Put it there
                        var task = new FirebaseStorage(_configuration["Firebase:BucketPath"],
                            new FirebaseStorageOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken), ThrowOnCancel = true,
                            })
                            .Child("Ina (1).png")
                            .PutAsync(stream);

                        // Track progress of the upload
                        task.Progress.ProgressChanged += (_, e) => Console.WriteLine($"Progress: {e.Percentage} %");

                        // await the task to wait until upload completes and get the download url
                        downloadUrl = await task;
                    }

                    return Result<Unit>.Success(Unit.Value, downloadUrl);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex) when (ex is DbUpdateException)
                {
                    _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}