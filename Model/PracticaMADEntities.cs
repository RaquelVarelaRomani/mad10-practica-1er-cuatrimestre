using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    public class PracticaMADEntities : DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentReport> CommentReports { get; set; }
        public DbSet<SellerRating> SellerRatings { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public PracticaMADEntities()
            : base("name=PracticaMADEntities")
        {
        }

        public PracticaMADEntities(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public PracticaMADEntities(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Remove pluralizing table name convention
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // 1. UserProfile configuration
            modelBuilder.Entity<UserProfile>()
                .ToTable("UserProfile")
                .HasKey(u => u.usrId);

            modelBuilder.Entity<UserProfile>()
                .Property(u => u.usrId)
                .HasColumnName("usrId");

            // 2. Category configuration
            modelBuilder.Entity<Category>()
                .ToTable("Category")
                .HasKey(c => c.categoryId);

            modelBuilder.Entity<Category>()
                .Property(c => c.categoryId)
                .HasColumnName("categoryId");

            // 3. Product configuration
            modelBuilder.Entity<Product>()
                .ToTable("Product")
                .HasKey(p => p.productId);

            modelBuilder.Entity<Product>()
                .Property(p => p.productId)
                .HasColumnName("productId");

            modelBuilder.Entity<Product>()
                .HasRequired(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.categoryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasRequired(p => p.Seller)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.sellerId)
                .WillCascadeOnDelete(false);

            // 4. Comment configuration
            modelBuilder.Entity<Comment>()
                .ToTable("Comment")
                .HasKey(c => c.commentId);

            modelBuilder.Entity<Comment>()
                .Property(c => c.commentId)
                .HasColumnName("commentId");

            modelBuilder.Entity<Comment>()
                .HasRequired(c => c.Product)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.productId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Comment>()
                .HasRequired(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.userId)
                .WillCascadeOnDelete(false);

            // Many-to-Many Comment <-> Tag via CommentTag table
            modelBuilder.Entity<Comment>()
                .HasMany(c => c.Tags)
                .WithMany(t => t.Comments)
                .Map(m =>
                {
                    m.ToTable("CommentTag");
                    m.MapLeftKey("commentId");
                    m.MapRightKey("tagId");
                });

            // 5. CommentReport configuration
            modelBuilder.Entity<CommentReport>()
                .ToTable("CommentReport")
                .HasKey(cr => cr.reportId);

            modelBuilder.Entity<CommentReport>()
                .Property(cr => cr.reportId)
                .HasColumnName("reportId");

            modelBuilder.Entity<CommentReport>()
                .HasRequired(cr => cr.Comment)
                .WithMany(c => c.CommentReports)
                .HasForeignKey(cr => cr.commentId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<CommentReport>()
                .HasRequired(cr => cr.User)
                .WithMany(u => u.CommentReports)
                .HasForeignKey(cr => cr.userId)
                .WillCascadeOnDelete(false);

            // 6. SellerRating configuration
            modelBuilder.Entity<SellerRating>()
                .ToTable("SellerRating")
                .HasKey(sr => sr.ratingId);

            modelBuilder.Entity<SellerRating>()
                .Property(sr => sr.ratingId)
                .HasColumnName("ratingId");

            modelBuilder.Entity<SellerRating>()
                .HasRequired(sr => sr.Seller)
                .WithMany(u => u.ReceivedRatings)
                .HasForeignKey(sr => sr.sellerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SellerRating>()
                .HasRequired(sr => sr.BuyerUser)
                .WithMany(u => u.GivenRatings)
                .HasForeignKey(sr => sr.buyerUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SellerRating>()
                .HasRequired(sr => sr.Product)
                .WithMany(p => p.SellerRatings)
                .HasForeignKey(sr => sr.productId)
                .WillCascadeOnDelete(false);

            // 7. Tag configuration
            modelBuilder.Entity<Tag>()
                .ToTable("Tag")
                .HasKey(t => t.tagId);

            modelBuilder.Entity<Tag>()
                .Property(t => t.tagId)
                .HasColumnName("tagId");
        }
    }
}

