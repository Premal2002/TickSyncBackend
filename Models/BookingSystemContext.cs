using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TickSyncAPI.Models;

public partial class BookingSystemContext : DbContext
{
    public BookingSystemContext()
    {
    }

    public BookingSystemContext(DbContextOptions<BookingSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Bus> Buses { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<SeatLock> SeatLocks { get; set; }

    public virtual DbSet<Show> Shows { get; set; }

    public virtual DbSet<Train> Trains { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Venue> Venues { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=DESKTOP-0DTLUPK\\SQLEXPRESS;Database=BookingSystem;Trusted_Connection=True;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951AED5F729C2F");

            entity.Property(e => e.BookingType).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Show).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ShowId)
                .HasConstraintName("FK__Bookings__ShowId__6A30C649");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__UserId__693CA210");
        });

        modelBuilder.Entity<Bus>(entity =>
        {
            entity.HasKey(e => e.BusId).HasName("PK__Buses__6A0F60B5AEF646A8");

            entity.Property(e => e.BusName).HasMaxLength(200);
            entity.Property(e => e.Destination).HasMaxLength(100);
            entity.Property(e => e.Source).HasMaxLength(100);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Events__7944C810D960A442");

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.Venue).WithMany(p => p.Events)
                .HasForeignKey(d => d.VenueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Events__VenueId__4316F928");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__Movies__4BD2941A443B7544");

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Genre).HasMaxLength(200);
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A38B33EB410");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TransactionId).HasMaxLength(100);

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Bookin__6EF57B66");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__UserId__6FE99F9F");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("PK__Seats__311713F388559582");

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RowNumber).HasMaxLength(10);
            entity.Property(e => e.SeatNumber).HasMaxLength(10);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Available");

            entity.HasOne(d => d.Venue).WithMany(p => p.Seats)
                .HasForeignKey(d => d.VenueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seats__VenueId__4E88ABD4");
        });

        modelBuilder.Entity<SeatLock>(entity =>
        {
            entity.HasKey(e => e.LockId).HasName("PK__Seat_Loc__E7C1E232C907643A");

            entity.ToTable("Seat_Locks");

            entity.Property(e => e.BookingType).HasMaxLength(50);
            entity.Property(e => e.ExpiryTime).HasColumnType("datetime");
            entity.Property(e => e.LockedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Seat).WithMany(p => p.SeatLocks)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seat_Lock__SeatI__74AE54BC");

            entity.HasOne(d => d.Show).WithMany(p => p.SeatLocks)
                .HasForeignKey(d => d.ShowId)
                .HasConstraintName("FK__Seat_Lock__ShowI__76969D2E");

            entity.HasOne(d => d.User).WithMany(p => p.SeatLocks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seat_Lock__UserI__75A278F5");
        });

        modelBuilder.Entity<Show>(entity =>
        {
            entity.HasKey(e => e.ShowId).HasName("PK__Shows__6DE3E0B267836ACF");

            entity.HasOne(d => d.Movie).WithMany(p => p.Shows)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Shows__MovieId__45F365D3");

            entity.HasOne(d => d.Venue).WithMany(p => p.Shows)
                .HasForeignKey(d => d.VenueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Shows__VenueId__46E78A0C");
        });

        modelBuilder.Entity<Train>(entity =>
        {
            entity.HasKey(e => e.TrainId).HasName("PK__Trains__8ED2723AB88BF367");

            entity.Property(e => e.Destination).HasMaxLength(100);
            entity.Property(e => e.Source).HasMaxLength(100);
            entity.Property(e => e.TrainName).HasMaxLength(200);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C7401C88F");

            entity.HasIndex(e => e.Phone, "UQ__Users__5C7E359EE4048357").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534DF418A06").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(15);
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.VenueId).HasName("PK__Venues__3C57E5F29BCA606C");

            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
