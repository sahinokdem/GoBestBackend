using System;
using System.Collections.Generic;
using GoBest.Models;
using Microsoft.EntityFrameworkCore;

namespace GoBest.Data;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingLeg> BookingLegs { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CompanyMaintainer> CompanyMaintainers { get; set; }

    public virtual DbSet<Itinerary> Itineraries { get; set; }

    public virtual DbSet<ItineraryLeg> ItineraryLegs { get; set; }

    public virtual DbSet<SeatType> SeatTypes { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceSeatInventory> ServiceSeatInventories { get; set; }

    public virtual DbSet<Station> Stations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=gobest;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("booking_pkey");

            entity.ToTable("booking");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookingTime)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("booking_time");
            entity.Property(e => e.ItineraryId).HasColumnName("itinerary_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10, 2)
                .HasColumnName("total_price");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Itinerary).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ItineraryId)
                .HasConstraintName("booking_itinerary_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("booking_user_id_fkey");
        });

        modelBuilder.Entity<BookingLeg>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("booking_leg_pkey");

            entity.ToTable("booking_leg");

            entity.HasIndex(e => new { e.BookingId, e.ServiceId }, "booking_leg_booking_id_service_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.SeatTypeId).HasColumnName("seat_type_id");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingLegs)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("booking_leg_booking_id_fkey");

            entity.HasOne(d => d.SeatType).WithMany(p => p.BookingLegs)
                .HasForeignKey(d => d.SeatTypeId)
                .HasConstraintName("booking_leg_seat_type_id_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.BookingLegs)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("booking_leg_service_id_fkey");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("city_pkey");

            entity.ToTable("city");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("country_code");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("company_pkey");

            entity.ToTable("company");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("country_code");
            entity.Property(e => e.IataCode)
                .HasMaxLength(3)
                .HasColumnName("iata_code");
            entity.Property(e => e.Mode)
                .HasColumnName("mode")
                .HasConversion<int>();
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasColumnName("name");
        });

        modelBuilder.Entity<CompanyMaintainer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("company_maintainer_pkey");

            entity.ToTable("company_maintainer");

            entity.HasIndex(e => new { e.UserId, e.CompanyId }, "company_maintainer_user_id_company_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyMaintainers)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("company_maintainer_company_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.CompanyMaintainers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("company_maintainer_user_id_fkey");
        });

        modelBuilder.Entity<Itinerary>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("itinerary_pkey");

            entity.ToTable("itinerary");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DestCityId).HasColumnName("dest_city_id");
            entity.Property(e => e.OriginCityId).HasColumnName("origin_city_id");
            entity.Property(e => e.SearchTime)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("search_time");
            entity.Property(e => e.TotalDuration).HasColumnName("total_duration");
            entity.Property(e => e.TotalLegs).HasColumnName("total_legs");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10, 2)
                .HasColumnName("total_price");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.DestCity).WithMany(p => p.ItineraryDestCities)
                .HasForeignKey(d => d.DestCityId)
                .HasConstraintName("itinerary_dest_city_id_fkey");

            entity.HasOne(d => d.OriginCity).WithMany(p => p.ItineraryOriginCities)
                .HasForeignKey(d => d.OriginCityId)
                .HasConstraintName("itinerary_origin_city_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Itineraries)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("itinerary_user_id_fkey");
        });

        modelBuilder.Entity<ItineraryLeg>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("itinerary_leg_pkey");

            entity.ToTable("itinerary_leg");

            entity.HasIndex(e => new { e.ItineraryId, e.LegOrder }, "itinerary_leg_itinerary_id_leg_order_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ItineraryId).HasColumnName("itinerary_id");
            entity.Property(e => e.LegOrder).HasColumnName("leg_order");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.SeatTypeId).HasColumnName("seat_type_id");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");

            entity.HasOne(d => d.Itinerary).WithMany(p => p.ItineraryLegs)
                .HasForeignKey(d => d.ItineraryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("itinerary_leg_itinerary_id_fkey");

            entity.HasOne(d => d.SeatType).WithMany(p => p.ItineraryLegs)
                .HasForeignKey(d => d.SeatTypeId)
                .HasConstraintName("itinerary_leg_seat_type_id_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.ItineraryLegs)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("itinerary_leg_service_id_fkey");
        });

        modelBuilder.Entity<SeatType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("seat_type_pkey");

            entity.ToTable("seat_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Mode)
                .HasColumnName("mode")
                .HasConversion<int>();
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .HasColumnName("name");
            entity.Property(e => e.PriceMultiplier)
                .HasPrecision(4, 2)
                .HasColumnName("price_multiplier");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("service_pkey");

            entity.ToTable("service");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArrivalTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("arrival_time");
            entity.Property(e => e.BasePrice)
                .HasPrecision(10, 2)
                .HasColumnName("base_price");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.DepartureTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("departure_time");
            entity.Property(e => e.DestStationId).HasColumnName("dest_station_id");
            entity.Property(e => e.OriginStationId).HasColumnName("origin_station_id");
            entity.Property(e => e.SalesCount)
                .HasDefaultValue(0)
                .HasColumnName("sales_count");
            entity.Property(e => e.ServiceCode)
                .HasMaxLength(16)
                .HasColumnName("service_code");
            entity.Property(e => e.Sold)
                .HasDefaultValue(false)
                .HasColumnName("sold");

            entity.HasOne(d => d.Company).WithMany(p => p.Services)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("service_company_id_fkey");

            entity.HasOne(d => d.DestStation).WithMany(p => p.ServiceDestStations)
                .HasForeignKey(d => d.DestStationId)
                .HasConstraintName("service_dest_station_id_fkey");

            entity.HasOne(d => d.OriginStation).WithMany(p => p.ServiceOriginStations)
                .HasForeignKey(d => d.OriginStationId)
                .HasConstraintName("service_origin_station_id_fkey");
        });

        modelBuilder.Entity<ServiceSeatInventory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("service_seat_inventory_pkey");

            entity.ToTable("service_seat_inventory");

            entity.HasIndex(e => new { e.ServiceId, e.SeatTypeId }, "service_seat_inventory_service_id_seat_type_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Available).HasColumnName("available");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.SeatTypeId).HasColumnName("seat_type_id");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");

            entity.HasOne(d => d.SeatType).WithMany(p => p.ServiceSeatInventories)
                .HasForeignKey(d => d.SeatTypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("service_seat_inventory_seat_type_id_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceSeatInventories)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("service_seat_inventory_service_id_fkey");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("station_pkey");

            entity.ToTable("station");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .HasColumnName("code");
            entity.Property(e => e.Latitude)
                .HasPrecision(9, 6)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasPrecision(9, 6)
                .HasColumnName("longitude");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasColumnName("name");
            entity.Property(e => e.StationType)
                .HasMaxLength(20)
                .HasColumnName("station_type");

            entity.HasOne(d => d.City).WithMany(p => p.Stations)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("station_city_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.ToTable("user");

            entity.HasIndex(e => e.Email, "user_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(128)
                .HasColumnName("full_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasColumnName("role")
                .HasConversion<int>();

        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
