using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Auth.Hash;
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
                var usersFromDb = await _context.User.ToListAsync();
                var usersToFireBase = new List<ImportUserRecordArgs>();

                if (usersFromDb.Count > 0)
                    foreach (var user in usersFromDb)
                        usersToFireBase.Add(
                            new ImportUserRecordArgs
                            {
                                Uid = user.UserId.ToString(),
                                Email = user.Email,
                                PhoneNumber = user.PhoneNumber,
                                DisplayName = user.FullName,
                                PhotoUrl = user.Avatar,
                                Disabled = false,
                                CustomClaims = new Dictionary<string, object>
                                {
                                    {"role", user.Role}
                                },
                                PasswordHash = Encoding.ASCII.GetBytes(user.PasswordHash)
                            }
                        );

                var options = new UserImportOptions
                {
                    Hash = new Bcrypt()
                };

                UserImportResult result = await FirebaseAuth.DefaultInstance
                    .ImportUsersAsync(usersToFireBase, options);

                foreach (ErrorInfo indexedError in result.Errors)
                    _logger.LogError($"Failed to import user: {indexedError.Reason}");
            }
            catch (FirebaseAuthException e)
            {
                _logger.LogError($"Error importing users: {e.Message}");
            }
        }
    }
}