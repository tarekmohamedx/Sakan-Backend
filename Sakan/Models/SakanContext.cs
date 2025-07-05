using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Sakan.Models;

public partial class SakanContext : DbContext
{
    public SakanContext()
    {
    }

    public SakanContext(DbContextOptions<SakanContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Amenity> Amenities { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Bed> Beds { get; set; }

    public virtual DbSet<BedPhoto> BedPhotos { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingRequest> BookingRequests { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Listing> Listings { get; set; }

    public virtual DbSet<ListingPhoto> ListingPhotos { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomPhoto> RoomPhotos { get; set; }

    public virtual DbSet<SupportTicket> SupportTickets { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<TicketReply> TicketReplies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=sakanak.database.windows.net;Database=sakan;User Id=sakanakteam45;Password=sakan@2468;Encrypt=True;TrustServerCertificate=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Amenitie__3214EC07E2E91D51");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Bed>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Beds__3214EC071B8CAC29");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.Label).HasMaxLength(50);
            entity.Property(e => e.OccupiedByUserId).HasMaxLength(450);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Type).HasMaxLength(20);

            entity.HasOne(d => d.OccupiedByUser).WithMany(p => p.Beds)
                .HasForeignKey(d => d.OccupiedByUserId)
                .HasConstraintName("FK__Beds__OccupiedBy__0A9D95DB");

            entity.HasOne(d => d.Room).WithMany(p => p.Beds)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__Beds__RoomId__08B54D69");
        });

        modelBuilder.Entity<BedPhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BedPhoto__3214EC0775C4C75B");

            entity.Property(e => e.PhotoUrl).HasMaxLength(255);

            entity.HasOne(d => d.Bed).WithMany(p => p.BedPhotos)
                .HasForeignKey(d => d.BedId)
                .HasConstraintName("FK__BedPhotos__BedId__2180FB33");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bookings__3214EC075C6F19C3");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GuestId).HasMaxLength(450);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PaymentStatus).HasMaxLength(20);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Bed).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.BedId)
                .HasConstraintName("FK__Bookings__BedId__10566F31");

            entity.HasOne(d => d.Guest).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.GuestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__GuestI__0D7A0286");

            entity.HasOne(d => d.Listing).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ListingId)
                .HasConstraintName("FK__Bookings__Listin__0E6E26BF");

            entity.HasOne(d => d.Room).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__Bookings__RoomId__0F624AF8");
        });

        modelBuilder.Entity<BookingRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookingR__3214EC078CA25B7B");

            entity.Property(e => e.GuestApproved).HasDefaultValue(false);
            entity.Property(e => e.GuestId).HasMaxLength(450);
            entity.Property(e => e.HostApproved).HasDefaultValue(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Bed).WithMany(p => p.BookingRequests)
                .HasForeignKey(d => d.BedId)
                .HasConstraintName("FK__BookingRe__BedId__17036CC0");

            entity.HasOne(d => d.Guest).WithMany(p => p.BookingRequests)
                .HasForeignKey(d => d.GuestId)
                .HasConstraintName("FK__BookingRe__Guest__14270015");

            entity.HasOne(d => d.Listing).WithMany(p => p.BookingRequests)
                .HasForeignKey(d => d.ListingId)
                .HasConstraintName("FK__BookingRe__Listi__151B244E");

            entity.HasOne(d => d.Room).WithMany(p => p.BookingRequests)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__BookingRe__RoomI__160F4887");
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("PK__Chats__A9FBE7C65D74D977");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Listing).WithMany(p => p.Chats)
                .HasForeignKey(d => d.ListingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Chats__ListingId__540C7B00");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ListingId }).HasName("PK__Favorite__0C7B27A18B893413");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Listing).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.ListingId)
                .HasConstraintName("FK_Favorites_Listings");

            entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Favorites_AspNetUsers");
        });

        modelBuilder.Entity<Listing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Listings__3214EC0789E080BA");

            entity.Property(e => e.AverageRating).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.District).HasMaxLength(100);
            entity.Property(e => e.Governorate).HasMaxLength(100);
            entity.Property(e => e.HostId).HasMaxLength(450);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsApproved).HasDefaultValue(false);
            entity.Property(e => e.IsBookableAsWhole).HasDefaultValue(true);
            entity.Property(e => e.MinBedPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PricePerMonth).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Title).HasMaxLength(150);

            entity.HasOne(d => d.Host).WithMany(p => p.Listings)
                .HasForeignKey(d => d.HostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Listings__HostId__7F2BE32F");

            entity.HasMany(d => d.Amenities).WithMany(p => p.Listings)
                .UsingEntity<Dictionary<string, object>>(
                    "ListingAmenity",
                    r => r.HasOne<Amenity>().WithMany()
                        .HasForeignKey("AmenitiesId")
                        .HasConstraintName("FK_ListingAmenities_Amenities"),
                    l => l.HasOne<Listing>().WithMany()
                        .HasForeignKey("ListingsId")
                        .HasConstraintName("FK_ListingAmenities_Listings"),
                    j =>
                    {
                        j.HasKey("ListingsId", "AmenitiesId").HasName("PK__ListingA__177C11808CFF318D");
                        j.ToTable("ListingAmenities");
                    });
        });

        modelBuilder.Entity<ListingPhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ListingP__3214EC075FDABF87");

            entity.Property(e => e.PhotoUrl).HasMaxLength(255);

            entity.HasOne(d => d.Listing).WithMany(p => p.ListingPhotos)
                .HasForeignKey(d => d.ListingId)
                .HasConstraintName("FK__ListingPh__Listi__1BC821DD");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C037C52C1C551");

            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.ReceiverId)
                .HasMaxLength(450)
                .HasColumnName("ReceiverID");
            entity.Property(e => e.SenderId)
                .HasMaxLength(450)
                .HasColumnName("SenderID");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Chat).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatId)
                .HasConstraintName("FK_Messages_Chat");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("FK__Messages__Receiv__395884C4");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK__Messages__Sender__3864608B");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC079AAC318F");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Notifications_AspNetUsers");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A58F9AA6902");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValue("usd");
            entity.Property(e => e.Method)
                .HasMaxLength(20)
                .HasDefaultValue("Stripe");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.StripePaymentIntentId)
                .HasMaxLength(100)
                .HasColumnName("StripePaymentIntentID");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Payments__Bookin__2A164134");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79AE8B85C912");

            entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ReviewedUserId)
                .HasMaxLength(450)
                .HasColumnName("ReviewedUserID");
            entity.Property(e => e.ReviewerId)
                .HasMaxLength(450)
                .HasColumnName("ReviewerID");

            entity.HasOne(d => d.Booking).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Reviews_Bookings");

            entity.HasOne(d => d.Listing).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ListingId)
                .HasConstraintName("FK_Review_Listing");

            entity.HasOne(d => d.ReviewedUser).WithMany(p => p.ReviewReviewedUsers)
                .HasForeignKey(d => d.ReviewedUserId)
                .HasConstraintName("FK__Reviews__Reviewe__3E1D39E1");

            entity.HasOne(d => d.Reviewer).WithMany(p => p.ReviewReviewers)
                .HasForeignKey(d => d.ReviewerId)
                .HasConstraintName("FK__Reviews__Reviewe__3D2915A8");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rooms__3214EC07FEFC7CA8");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsBookableAsWhole).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PricePerNight).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Type).HasMaxLength(20);

            entity.HasOne(d => d.Listing).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.ListingId)
                .HasConstraintName("FK__Rooms__ListingId__04E4BC85");
        });

        modelBuilder.Entity<RoomPhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RoomPhot__3214EC07C35A5E4D");

            entity.Property(e => e.PhotoUrl).HasMaxLength(255);

            entity.HasOne(d => d.Room).WithMany(p => p.RoomPhotos)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__RoomPhoto__RoomI__1EA48E88");
        });

        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SupportT__3214EC07700DED93");

            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.GuestAccessToken).HasMaxLength(100);
            entity.Property(e => e.GuestEmail).HasMaxLength(200);
            entity.Property(e => e.GuestName).HasMaxLength(200);
            entity.Property(e => e.Priority)
                .HasMaxLength(50)
                .HasDefaultValue("Normal");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Open");
            entity.Property(e => e.Subject).HasMaxLength(250);
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.Booking).WithMany(p => p.SupportTickets)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_SupportTickets_Bookings");

            entity.HasOne(d => d.User).WithMany(p => p.SupportTickets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_SupportTickets_AspNetUsers");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Test__3213E83F68BC99D2");

            entity.ToTable("Test");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TicketReply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TicketRe__3214EC075DD0A917");

            entity.Property(e => e.AuthorId).HasMaxLength(450);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Author).WithMany(p => p.TicketReplies)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK_TicketReplies_AspNetUsers");

            entity.HasOne(d => d.SupportTicket).WithMany(p => p.TicketReplies)
                .HasForeignKey(d => d.SupportTicketId)
                .HasConstraintName("FK_TicketReplies_SupportTickets");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
