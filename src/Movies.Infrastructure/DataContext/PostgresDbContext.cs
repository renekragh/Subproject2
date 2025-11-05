using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;

namespace Movies.Infrastructure.DataContext;

public partial class PostgresDbContext : DbContext
{
//    private readonly string? _connectionString;

/*
    public PostgresDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

*/
    public PostgresDbContext( DbContextOptions<PostgresDbContext> options) : base(options)
    {
    }


    public DbSet<ImdbUser> ImdbUsers { get; set; }
    public DbSet<Localized> Localizeds { get; set; }
    public DbSet<NameKnownForTitle> NameKnownForTitles { get; set; }
    public DbSet<Name> Names { get; set; }
    public DbSet<NamePrimaryProfession> NamePrimaryProfessions { get; set; }
    public DbSet<Principal> Principals { get; set; }
    public DbSet<RatingHistory> RatingHistories { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<SearchHistory> SearchHistories { get; set; }
    public DbSet<TitleGenre> TitleGenres { get; set; }
    public DbSet<Title> Titles { get; set; }
    public DbSet<UserBookmarkName> UserBookmarkNames { get; set; }
    public DbSet<UserBookmarkTitle> UserBookmarkTitles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        optionsBuilder.EnableSensitiveDataLogging();

       // optionsBuilder.UseNpgsql(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<ImdbUser>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("imdb_user_pkey");

            entity.ToTable("imdb_user");

            entity.HasIndex(e => e.Email, "imdb_user_email_key").IsUnique();

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
            entity.Property(e => e.UserName)
                .HasMaxLength(256)
                .HasColumnName("username");
            entity.Property(e => e.Salt)
                .HasMaxLength(256)
                .HasColumnName("salt");
            entity.Property(e => e.Role)
                .HasMaxLength(100)
                .HasColumnName("role");
        });

        modelBuilder.Entity<Localized>(entity =>
        {
            entity.HasKey(e => new { e.Titleid, e.Ordering }).HasName("localized_pkey");

            entity.ToTable("localized");

            entity.Property(e => e.Titleid)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("titleid");
            entity.Property(e => e.Ordering).HasColumnName("ordering");
            entity.Property(e => e.Attributes)
                .HasMaxLength(256)
                .HasColumnName("attributes");
            entity.Property(e => e.Isoriginaltitle).HasColumnName("isoriginaltitle");
            entity.Property(e => e.Language)
                .HasMaxLength(10)
                .HasColumnName("language");
            entity.Property(e => e.Region)
                .HasMaxLength(10)
                .HasColumnName("region");
            entity.Property(e => e.Tconst)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("tconst");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Types)
                .HasMaxLength(256)
                .HasColumnName("types");

            entity.HasOne(d => d.TconstNavigation).WithMany(p => p.Localizeds)
                .HasForeignKey(d => d.Tconst)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("localized_tconst_fkey");
        });

        modelBuilder.Entity<Name>(entity =>
        {
            entity.HasKey(e => e.Nconst).HasName("name_pkey");

            entity.ToTable("name");

            entity.Property(e => e.Nconst)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("nconst");
            entity.Property(e => e.Birthyear)
                .HasMaxLength(4)
                .IsFixedLength()
                .HasColumnName("birthyear");
            entity.Property(e => e.Deathyear)
                .HasMaxLength(4)
                .IsFixedLength()
                .HasColumnName("deathyear");
            entity.Property(e => e.Primaryname)
                .HasMaxLength(256)
                .HasColumnName("primaryname");
        });

        modelBuilder.Entity<NameKnownForTitle>(entity =>
        {
            entity.HasKey(e => new { e.Knownfortitle, e.Nconst }).HasName("name_known_for_titles_pkey");

            entity.ToTable("name_known_for_titles");

            entity.Property(e => e.Knownfortitle)
                .HasMaxLength(256)
                .HasColumnName("knownfortitle");
            entity.Property(e => e.Nconst)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("nconst");

            entity.HasOne(d => d.NconstNavigation).WithMany(p => p.NameKnownForTitles)
                .HasForeignKey(d => d.Nconst)
                .HasConstraintName("name_known_for_titles_nconst_fkey");
        });

        modelBuilder.Entity<NamePrimaryProfession>(entity =>
        {
            entity.HasKey(e => new { e.Primaryprofession, e.Nconst }).HasName("name_primary_profession_pkey");

            entity.ToTable("name_primary_profession");

            entity.Property(e => e.Primaryprofession)
                .HasMaxLength(256)
                .HasColumnName("primaryprofession");
            entity.Property(e => e.Nconst)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("nconst");

            entity.HasOne(d => d.NconstNavigation).WithMany(p => p.NamePrimaryProfessions)
                .HasForeignKey(d => d.Nconst)
                .HasConstraintName("name_primary_profession_nconst_fkey");
        });

        modelBuilder.Entity<Principal>(entity =>
        {
            entity.HasKey(e => new { e.Tconst, e.Nconst, e.Ordering }).HasName("principal_pkey");

            entity.ToTable("principal");

            entity.Property(e => e.Tconst)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("tconst");
            entity.Property(e => e.Nconst)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("nconst");
            entity.Property(e => e.Ordering).HasColumnName("ordering");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasColumnName("category");
            entity.Property(e => e.Characters).HasColumnName("characters");
            entity.Property(e => e.Job).HasColumnName("job");

            entity.HasOne(d => d.NconstNavigation).WithMany(p => p.Principals)
                .HasForeignKey(d => d.Nconst)
                .HasConstraintName("principal_nconst_fkey");

            entity.HasOne(d => d.TconstNavigation).WithMany(p => p.Principals)
                .HasForeignKey(d => d.Tconst)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("principal_tconst_fkey");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Ratingid).HasName("rating_pkey");

            entity.ToTable("rating");

            entity.Property(e => e.Ratingid).HasColumnName("ratingid");
            entity.Property(e => e.Averagerating)
                .HasPrecision(5, 1)
                .HasColumnName("averagerating");
            entity.Property(e => e.Numvotes).HasColumnName("numvotes");
            entity.Property(e => e.Tconst)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("tconst");

            entity.HasOne(d => d.TconstNavigation).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.Tconst)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("rating_tconst_fkey");
        });

        modelBuilder.Entity<RatingHistory>(entity =>
        {
            entity.HasKey(e => new { e.Tconst, e.Userid }).HasName("rating_history_pkey");

            entity.ToTable("rating_history");

            entity.Property(e => e.Tconst)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("tconst");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");

            entity.HasOne(d => d.TconstNavigation).WithMany(p => p.RatingHistories)
                .HasForeignKey(d => d.Tconst)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("rating_history_tconst_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.RatingHistories)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("rating_history_userid_fkey");
        });

        modelBuilder.Entity<SearchHistory>(entity =>
        {
            entity.HasKey(e => e.Searchid).HasName("search_history_pkey");

            entity.ToTable("search_history");

            entity.Property(e => e.Searchid).HasColumnName("searchid");
            entity.Property(e => e.SearchQuery).HasColumnName("search_query");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.SearchHistories)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("search_history_userid_fkey");
        });

        modelBuilder.Entity<Title>(entity =>
        {
            entity.HasKey(e => e.Tconst).HasName("title_pkey");

            entity.ToTable("title");

            entity.Property(e => e.Tconst)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("tconst");
            entity.Property(e => e.Endyear)
                .HasMaxLength(4)
                .IsFixedLength()
                .HasColumnName("endyear");
            entity.Property(e => e.Episode)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("episode");
            entity.Property(e => e.Episodenumber).HasColumnName("episodenumber");
            entity.Property(e => e.Isadult).HasColumnName("isadult");
            entity.Property(e => e.Originaltitle).HasColumnName("originaltitle");
            entity.Property(e => e.Plot).HasColumnName("plot");
            entity.Property(e => e.Poster)
                .HasMaxLength(180)
                .HasColumnName("poster");
            entity.Property(e => e.Primarytitle).HasColumnName("primarytitle");
            entity.Property(e => e.Runtimeinminutes).HasColumnName("runtimeinminutes");
            entity.Property(e => e.Seasonnumber).HasColumnName("seasonnumber");
            entity.Property(e => e.Startyear)
                .HasMaxLength(4)
                .IsFixedLength()
                .HasColumnName("startyear");
            entity.Property(e => e.Titletype)
                .HasMaxLength(20)
                .HasColumnName("titletype");

            entity.HasOne(d => d.EpisodeNavigation).WithMany(p => p.InverseEpisodeNavigation)
                .HasForeignKey(d => d.Episode)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("title_episode_fkey");
        });

        modelBuilder.Entity<TitleGenre>(entity =>
        {
            entity.HasKey(e => new { e.Genre, e.Tconst }).HasName("title_genres_pkey");

            entity.ToTable("title_genres");

            entity.Property(e => e.Genre)
                .HasMaxLength(256)
                .HasColumnName("genre");
            entity.Property(e => e.Tconst)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("tconst");

            entity.HasOne(d => d.TconstNavigation).WithMany(p => p.TitleGenres)
                .HasForeignKey(d => d.Tconst)
                .HasConstraintName("title_genres_tconst_fkey");
        });

        modelBuilder.Entity<UserBookmarkName>(entity =>
        {
            entity.HasKey(e => new { e.Nconst, e.Userid }).HasName("user_bookmark_name_pkey");

            entity.ToTable("user_bookmark_name");

            entity.Property(e => e.Nconst)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("nconst");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Note).HasColumnName("note");

            entity.HasOne(d => d.NconstNavigation).WithMany(p => p.UserBookmarkNames)
                .HasForeignKey(d => d.Nconst)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_bookmark_name_nconst_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserBookmarkNames)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_bookmark_name_userid_fkey");
        });

        modelBuilder.Entity<UserBookmarkTitle>(entity =>
        {
            entity.HasKey(e => new { e.Tconst, e.Userid }).HasName("user_bookmark_title_pkey");

            entity.ToTable("user_bookmark_title");

            entity.Property(e => e.Tconst)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("tconst");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Note).HasColumnName("note");

            entity.HasOne(d => d.TconstNavigation).WithMany(p => p.UserBookmarkTitles)
                .HasForeignKey(d => d.Tconst)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_bookmark_title_tconst_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserBookmarkTitles)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_bookmark_title_userid_fkey");
        });

        modelBuilder.Entity<Wi>(entity =>
        {
            entity.HasKey(e => new { e.Tconst, e.Word, e.Field }).HasName("wi_pkey");

            entity.ToTable("wi");

            entity.Property(e => e.Tconst)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("tconst");
            entity.Property(e => e.Word).HasColumnName("word");
            entity.Property(e => e.Field)
                .HasMaxLength(1)
                .HasColumnName("field");
            entity.Property(e => e.Lexeme).HasColumnName("lexeme");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}