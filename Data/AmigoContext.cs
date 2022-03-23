#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Amigo.Models;

    public class AmigoContext : DbContext
    {
        public AmigoContext (DbContextOptions<AmigoContext> options)
            : base(options)
        {
        }

        public DbSet<Amigo.Models.Product> Product { get; set; }

        public DbSet<Amigo.Models.Admin> Admin { get; set; }
    }
