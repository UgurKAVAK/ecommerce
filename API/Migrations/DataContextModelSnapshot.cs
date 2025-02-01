﻿// <auto-generated />
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("API.Entity.Card", b =>
                {
                    b.Property<int>("CardId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CardId"));

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CardId");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("API.Entity.CardItem", b =>
                {
                    b.Property<int>("CardItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CardItemId"));

                    b.Property<int>("CardId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("CardItemId");

                    b.HasIndex("CardId");

                    b.HasIndex("ProductId");

                    b.ToTable("CardItem");
                });

            modelBuilder.Entity("API.Entity.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Siyah",
                            ImageUrl = "1.jpg",
                            IsActive = true,
                            Name = "Iphone 12",
                            Price = 12000m,
                            Stock = 10
                        },
                        new
                        {
                            Id = 2,
                            Description = "Siyah",
                            ImageUrl = "2.jpg",
                            IsActive = true,
                            Name = "Iphone 13",
                            Price = 13000m,
                            Stock = 10
                        },
                        new
                        {
                            Id = 3,
                            Description = "Siyah",
                            ImageUrl = "3.jpg",
                            IsActive = true,
                            Name = "Iphone 14",
                            Price = 14000m,
                            Stock = 10
                        },
                        new
                        {
                            Id = 4,
                            Description = "Siyah",
                            ImageUrl = "4.jpg",
                            IsActive = true,
                            Name = "Iphone 15",
                            Price = 15000m,
                            Stock = 10
                        },
                        new
                        {
                            Id = 5,
                            Description = "Siyah",
                            ImageUrl = "5.jpg",
                            IsActive = true,
                            Name = "Iphone 16",
                            Price = 16000m,
                            Stock = 10
                        });
                });

            modelBuilder.Entity("API.Entity.CardItem", b =>
                {
                    b.HasOne("API.Entity.Card", "Card")
                        .WithMany("CardItems")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Entity.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("API.Entity.Card", b =>
                {
                    b.Navigation("CardItems");
                });
#pragma warning restore 612, 618
        }
    }
}
