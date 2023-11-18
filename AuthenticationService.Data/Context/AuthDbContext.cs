using System;
using System.Reflection;
using AuthenticationService.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Data.Context
{
	public class AuthDbContext : IdentityDbContext<User>
	{
		public AuthDbContext(DbContextOptions<AuthDbContext> options): base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}
	}
}

