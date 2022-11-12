﻿// <auto-generated />
using System;
using LLS.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LLS.DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20220613223214_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("LLS.Common.Models.Course", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("AddedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("CourseIDD")
                        .HasColumnType("integer");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("NumberOfExp")
                        .HasColumnType("integer");

                    b.Property<int>("NumberOfStudents")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("LLS.Common.Models.Exp_Course", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("ExperimentId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("ExperimentId");

                    b.ToTable("Exp_Courses");
                });

            modelBuilder.Entity("LLS.Common.Models.Experiment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("AddedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("AuthorId")
                        .HasColumnType("text");

                    b.Property<string>("AuthorName")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<Guid?>("LLOGuid")
                        .HasColumnType("uuid");

                    b.Property<string>("LLOPath")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("LLOGuid");

                    b.ToTable("Expirments");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.LLO", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.HasKey("Guid");

                    b.ToTable("LLOs");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.Pages.Blocks.Block", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ConfigGuid")
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<int>("MainType")
                        .HasColumnType("integer");

                    b.Property<Guid?>("PageGuid")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ScoreGuid")
                        .HasColumnType("uuid");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Guid");

                    b.HasIndex("ConfigGuid");

                    b.HasIndex("PageGuid");

                    b.HasIndex("ScoreGuid");

                    b.ToTable("Blocks");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.Pages.Blocks.ChoiceScore", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("ChoiceId")
                        .HasColumnType("integer");

                    b.Property<Guid?>("ScoreGuid")
                        .HasColumnType("uuid");

                    b.Property<int>("score")
                        .HasColumnType("integer");

                    b.HasKey("Guid");

                    b.HasIndex("ScoreGuid");

                    b.ToTable("ChoiceScore");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.Pages.Blocks.Config.Configs", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Grading")
                        .HasColumnType("text");

                    b.Property<string>("Help")
                        .HasColumnType("text");

                    b.Property<int>("NumberOfTrials")
                        .HasColumnType("integer");

                    b.Property<bool>("ShowTime")
                        .HasColumnType("boolean");

                    b.Property<string>("Time")
                        .HasColumnType("text");

                    b.Property<bool>("TimeState")
                        .HasColumnType("boolean");

                    b.Property<bool>("ViewCorrectAnswer")
                        .HasColumnType("boolean");

                    b.HasKey("Guid");

                    b.ToTable("Configs");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.Pages.Blocks.Content.Choice", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("ChoiceId")
                        .HasColumnType("integer");

                    b.Property<Guid?>("ContentGuid")
                        .HasColumnType("uuid");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Guid");

                    b.HasIndex("ContentGuid");

                    b.ToTable("Choices");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.Pages.Blocks.Content.Content", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("HtmlContent")
                        .HasColumnType("text");

                    b.Property<string>("Instructions")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("Guid");

                    b.ToTable("Contents");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.Pages.Blocks.Content.Onspot", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<Guid?>("ContentGuid")
                        .HasColumnType("uuid");

                    b.Property<int>("Left")
                        .HasColumnType("integer");

                    b.Property<int>("Top")
                        .HasColumnType("integer");

                    b.HasKey("Guid");

                    b.HasIndex("ContentGuid");

                    b.ToTable("OnSpots");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.Pages.Blocks.Score", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Answer")
                        .HasColumnType("text");

                    b.Property<int>("score")
                        .HasColumnType("integer");

                    b.HasKey("Guid");

                    b.ToTable("Scores");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.Pages.Page", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("LLOGuid")
                        .HasColumnType("uuid");

                    b.Property<long>("id")
                        .HasColumnType("bigint");

                    b.Property<int>("title")
                        .HasColumnType("integer");

                    b.HasKey("Guid");

                    b.HasIndex("LLOGuid");

                    b.ToTable("Pages");
                });

            modelBuilder.Entity("LLS.Common.Models.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("AddedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("ExpirayDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsRevorked")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("boolean");

                    b.Property<string>("JwtId")
                        .HasColumnType("text");

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("LLS.Common.Models.Student_Course", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("StudentId");

                    b.ToTable("Student_Courses");
                });

            modelBuilder.Entity("LLS.Common.Models.Student_ExpCourse", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Answers")
                        .HasColumnType("text");

                    b.Property<Guid>("Exp_CourseId")
                        .HasColumnType("uuid");

                    b.Property<int>("Grading")
                        .HasColumnType("integer");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("boolean");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Exp_CourseId");

                    b.HasIndex("StudentId");

                    b.ToTable("Student_ExpCourses");
                });

            modelBuilder.Entity("LLS.Common.Models.Teacher_Course", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TeacherId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("TeacherId");

                    b.ToTable("Teacher_Courses");
                });

            modelBuilder.Entity("LLS.Common.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AcademicYear")
                        .HasColumnType("text");

                    b.Property<DateTime>("AddedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Country")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<Guid>("IdentityId")
                        .HasColumnType("uuid");

                    b.Property<string>("Lastname")
                        .HasColumnType("text");

                    b.Property<int>("PhoneNumber")
                        .HasColumnType("integer");

                    b.Property<string>("Role")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("LLS.Common.Models.Exp_Course", b =>
                {
                    b.HasOne("LLS.Common.Models.Course", "Course")
                        .WithMany("Exp_Courses")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LLS.Common.Models.Experiment", "Experiment")
                        .WithMany("Exp_Courses")
                        .HasForeignKey("ExperimentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Experiment");
                });

            modelBuilder.Entity("LLS.Common.Models.Experiment", b =>
                {
                    b.HasOne("LLS.Common.Models.LLO.LLO", "LLO")
                        .WithMany()
                        .HasForeignKey("LLOGuid");

                    b.Navigation("LLO");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.Pages.Blocks.Block", b =>
                {
                    b.HasOne("LLS.Common.Models.LLO.Pages.Blocks.Config.Configs", "Config")
                        .WithMany()
                        .HasForeignKey("ConfigGuid");

                    b.HasOne("LLS.Common.Models.LLO.Pages.Page", null)
                        .WithMany("Blocks")
                        .HasForeignKey("PageGuid");

                    b.HasOne("LLS.Common.Models.LLO.Pages.Blocks.Score", "Score")
                        .WithMany()
                        .HasForeignKey("ScoreGuid");

                    b.Navigation("Config");

                    b.Navigation("Score");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.Pages.Blocks.ChoiceScore", b =>
                {
                    b.HasOne("LLS.Common.Models.LLO.Pages.Blocks.Score", null)
                        .WithMany("choices")
                        .HasForeignKey("ScoreGuid");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.Pages.Blocks.Content.Choice", b =>
                {
                    b.HasOne("LLS.Common.Models.LLO.Pages.Blocks.Content.Content", null)
                        .WithMany("Choices")
                        .HasForeignKey("ContentGuid");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.Pages.Blocks.Content.Onspot", b =>
                {
                    b.HasOne("LLS.Common.Models.LLO.Pages.Blocks.Content.Content", null)
                        .WithMany("Onspots")
                        .HasForeignKey("ContentGuid");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.Pages.Page", b =>
                {
                    b.HasOne("LLS.Common.Models.LLO.LLO", null)
                        .WithMany("Pages")
                        .HasForeignKey("LLOGuid");
                });

            modelBuilder.Entity("LLS.Common.Models.RefreshToken", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LLS.Common.Models.Student_Course", b =>
                {
                    b.HasOne("LLS.Common.Models.Course", "Course")
                        .WithMany("Student_Courses")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LLS.Common.Models.User", "User")
                        .WithMany("Student_Courses")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LLS.Common.Models.Student_ExpCourse", b =>
                {
                    b.HasOne("LLS.Common.Models.Exp_Course", "Exp_Course")
                        .WithMany("Student_ExpCourses")
                        .HasForeignKey("Exp_CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LLS.Common.Models.User", "User")
                        .WithMany("Student_ExpCourses")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exp_Course");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LLS.Common.Models.Teacher_Course", b =>
                {
                    b.HasOne("LLS.Common.Models.Course", "Course")
                        .WithMany("Teacher_Courses")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LLS.Common.Models.User", "User")
                        .WithMany("Teacher_Courses")
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("LLS.Common.Models.Course", b =>
                {
                    b.Navigation("Exp_Courses");

                    b.Navigation("Student_Courses");

                    b.Navigation("Teacher_Courses");
                });

            modelBuilder.Entity("LLS.Common.Models.Exp_Course", b =>
                {
                    b.Navigation("Student_ExpCourses");
                });

            modelBuilder.Entity("LLS.Common.Models.Experiment", b =>
                {
                    b.Navigation("Exp_Courses");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.LLO", b =>
                {
                    b.Navigation("Pages");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.Pages.Blocks.Content.Content", b =>
                {
                    b.Navigation("Choices");

                    b.Navigation("Onspots");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.Pages.Blocks.Score", b =>
                {
                    b.Navigation("choices");
                });

            modelBuilder.Entity("LLS.Common.Models.LLO.Pages.Page", b =>
                {
                    b.Navigation("Blocks");
                });

            modelBuilder.Entity("LLS.Common.Models.User", b =>
                {
                    b.Navigation("Student_Courses");

                    b.Navigation("Student_ExpCourses");

                    b.Navigation("Teacher_Courses");
                });
#pragma warning restore 612, 618
        }
    }
}
