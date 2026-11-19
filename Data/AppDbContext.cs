using HomeOfMemories.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeOfMemories.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // список таблиц
    public DbSet<User> Users => Set<User>();
    public DbSet<Memory> Memories => Set<Memory>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<MemoryTag> MemoryTags => Set<MemoryTag>();
    public DbSet<Reaction> Reactions => Set<Reaction>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<Translation> Translations => Set<Translation>();
    public DbSet<UiTranslation> UiTranslations => Set<UiTranslation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // уникальность логина и почты
        modelBuilder.Entity<User>().HasIndex(u => u.Login).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        // составные ключи
        modelBuilder.Entity<MemoryTag>().HasKey(mt => new { mt.MemoryId, mt.TagId });
        modelBuilder.Entity<Favorite>().HasKey(f => new { f.UserId, f.MemoryId });

        // одна реакция на пару пользователь-воспоминание
        modelBuilder.Entity<Reaction>()
            .HasIndex(r => new { r.UserId, r.MemoryId }).IsUnique();

        // настройка каскадных удалений для реакций
        modelBuilder.Entity<Reaction>()
            .HasOne(r => r.Memory).WithMany(m => m.Reactions)
            .HasForeignKey(r => r.MemoryId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Reaction>()
            .HasOne(r => r.User).WithMany(u => u.Reactions)
            .HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.NoAction);

        // - для комментариев
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Memory).WithMany(m => m.Comments)
            .HasForeignKey(c => c.MemoryId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User).WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.NoAction);

        // - для избранного
        modelBuilder.Entity<Favorite>()
           .HasOne(f => f.Memory).WithMany()
           .HasForeignKey(f => f.MemoryId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Favorite>()
            .HasOne(f => f.User).WithMany()
            .HasForeignKey(f => f.UserId).OnDelete(DeleteBehavior.NoAction);

        // уникальный индекс для кеша переводов
        modelBuilder.Entity<Translation>()
            .HasIndex(t => new { t.SourceHash, t.SourceLang, t.TargetLang })
            .IsUnique();

        // настройка уникальности для готовых переводов
        modelBuilder.Entity<UiTranslation>()
            .HasIndex(t => new { t.Key, t.Language })
            .IsUnique();

        // заполнение категорий
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Игрушки", Slug = "toys" },
            new Category { Id = 2, Name = "Мебель", Slug = "furniture" },
            new Category { Id = 3, Name = "Рукоделие", Slug = "handicrafts" },
            new Category { Id = 4, Name = "Одежда", Slug = "clothes" },
            new Category { Id = 5, Name = "Аксессуары", Slug = "accessories" },
            new Category { Id = 6, Name = "Печать", Slug = "print" },
            new Category { Id = 7, Name = "Техника", Slug = "devices" },
            new Category { Id = 8, Name = "Природа", Slug = "nature" }
        );

        // начальное заполнение словаря интерфейса
        modelBuilder.Entity<UiTranslation>().HasData(
            new UiTranslation { Id = 1, Key = "main", Language = "en", Text = "Home of Memories" },
            new UiTranslation { Id = 2, Key = "footer.main", Language = "en", Text = "@ Home of Memories 2026" },
            new UiTranslation { Id = 3, Key = "slogan", Language = "en", Text = "Stories create History" },
            new UiTranslation { Id = 4, Key = "nav.find", Language = "en", Text = "Find memory" },
            new UiTranslation { Id = 5, Key = "nav.add", Language = "en", Text = "Add memory" },
            new UiTranslation { Id = 6, Key = "nav.random", Language = "en", Text = "Random memory" },
            new UiTranslation { Id = 7, Key = "nav.about", Language = "en", Text = "About us" },
            new UiTranslation { Id = 8, Key = "nav.about", Language = "ru", Text = "О нас" },
            new UiTranslation { Id = 9, Key = "main", Language = "ru", Text = "Дом Воспоминаний" },
            new UiTranslation { Id = 10, Key = "footer.main", Language = "ru", Text = "@ Дом Воспоминаний 2026" },
            new UiTranslation { Id = 11, Key = "slogan", Language = "ru", Text = "Воспоминания хранят Память" },
            new UiTranslation { Id = 12, Key = "nav.find", Language = "ru", Text = "Найти воспоминание" },
            new UiTranslation { Id = 13, Key = "nav.add", Language = "ru", Text = "Добавить воспоминание" },
            new UiTranslation { Id = 14, Key = "nav.random", Language = "ru", Text = "Случайное воспоминание" }
        );
    }
}