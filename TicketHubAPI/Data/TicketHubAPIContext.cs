using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketHubAPI.Models;

namespace TicketHubAPI.Data
{
    public class TicketHubAPIContext : DbContext
    {
        public TicketHubAPIContext (DbContextOptions<TicketHubAPIContext> options)
            : base(options)
        {
        }

        public DbSet<TicketHubAPI.Models.Contact> Contact { get; set; } = default!;
    }
}
