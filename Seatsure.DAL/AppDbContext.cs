
using Microsoft.EntityFrameworkCore;
using Seatsure.Domain;

namespace Seatsure.DAL
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {


        }

        public DbSet<User> User { get; set; }

        public DbSet<Event> Event { get; set; }

        public DbSet<TicketType> TicketType { get; set; }

        public DbSet<Reservation> Reservation { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().
                HasKey(u => u.id);

            modelBuilder.Entity<User>().
                HasIndex(u => u.email);


            modelBuilder.Entity<Event>().
                HasKey(e => e.id);
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organizer);
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Tickets)
                .WithOne(t => t.Event);


            modelBuilder.Entity<Reservation>().
                HasKey(r => r.id);

            modelBuilder.Entity<Reservation>().
                HasOne(r => r.ticket);
            modelBuilder.Entity<Reservation>().
                HasOne(r => r.user);

            modelBuilder.Entity<TicketType>().
                HasOne(t => t.Event);



        }

    }
}