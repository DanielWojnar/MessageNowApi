using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using MessageNowApi.Models;

namespace MessageNowApi.Data
{
    public class IdentityContext : IdentityDbContext<MessageNowUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<UserConvConnector> UserConvConnectors { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Message>().HasOne(x => x.User);
            builder.Entity<Message>().HasOne(x => x.Conversation).WithMany(x => x.Messages);
            builder.Entity<UserConvConnector>().HasOne(x => x.Conversation).WithMany(x => x.UserConvConnectors);
            builder.Entity<UserConvConnector>().HasOne(x => x.User);
            base.OnModelCreating(builder);
        }
    }
}
