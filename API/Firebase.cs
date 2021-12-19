using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Auth.Hash;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace API
{
    public class Firebase
    {
        private readonly DataContext _context;
        private readonly ILogger<Firebase> _logger;

        public Firebase(DataContext context, ILogger<Firebase> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ImportUserFromDatabaseToFirebase()
        {
            try
            {
                List<User> usersFromDb = await _context.User.ToListAsync();
                var usersToFireBase = new List<ImportUserRecordArgs>();

                if (usersFromDb.Count > 0)
                    usersToFireBase.AddRange(usersFromDb.Select(user => new ImportUserRecordArgs
                    {
                        Uid = user.UserId.ToString(),
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        DisplayName = user.FullName,
                        PhotoUrl = user.Avatar,
                        Disabled = false,
                        CustomClaims = new Dictionary<string, object> {{"role", user.RoleId}},
                        PasswordHash = Encoding.ASCII.GetBytes(user.PasswordHash)
                    }));

                var options = new UserImportOptions {Hash = new Bcrypt()};

                UserImportResult result = await FirebaseAuth.DefaultInstance.ImportUsersAsync(usersToFireBase, options);

                foreach (ErrorInfo indexedError in result.Errors)
                    _logger.LogError("Failed to import user: {indexedError.Reason}", indexedError.Reason);
            }
            catch (FirebaseAuthException e)
            {
                _logger.LogError("Error importing users: {e.Message}", e.Message);
            }
            catch (TokenResponseException e)
            {
                _logger.LogError("Firebase service is down or configured wrong. {Error}", e.Message);
            }       
        }       
    }
}