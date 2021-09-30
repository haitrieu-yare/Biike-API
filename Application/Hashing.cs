using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application
{
	public class Hashing
	{
		private readonly DataContext _context;
		private readonly ILogger<Hashing> _logger;
		public Hashing(DataContext context, ILogger<Hashing> logger)
		{
			_logger = logger;
			_context = context;
		}

		public async Task CreatePasswordForUsers()
		{
			try
			{
				var usersFromDB = await _context.User.ToListAsync();
				if (usersFromDB.Count == 0) return;

				foreach (var user in usersFromDB)
				{
					string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
					user.PasswordHash = passwordHash;
				}

				var result = await _context.SaveChangesAsync() > 0;
				if (!result)
				{
					throw new Exception("Failed to create hashed password for users");
				}
			}
			catch (System.Exception ex)
			{
				_logger.LogError(ex.Message);
			}
		}

		public string HashPassword(string password)
		{
			return BCrypt.Net.BCrypt.HashPassword(password);
		}
	}
}