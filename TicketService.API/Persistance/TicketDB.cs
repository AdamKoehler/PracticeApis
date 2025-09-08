//using Microsoft.EntityFrameworkCore;
//using TicketService.API.Shared.Domain.Models;
//namespace TicketService.API.Persistance;

//public class TicketDB : DbContext
//{
//    public TicketDB(DbContextOptions<TicketDB> options) : base(options)
//    {
//    }

//    public DbSet<Ticket> tickets => Set<Ticket>();

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//    {
//        optionsBuilder.UseSqlServer("<CONNECTION STRING>");
//    }
//}
