﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ElectroJournal.DataBase
{
    public partial class zhirovContext : DbContext
    {
        public zhirovContext()
        {
        }

        public zhirovContext(DbContextOptions<zhirovContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Basistraining> Basistrainings { get; set; }
        public virtual DbSet<Cabinet> Cabinets { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Date> Dates { get; set; }
        public virtual DbSet<Discipline> Disciplines { get; set; }
        public virtual DbSet<Disciplinehour> Disciplinehours { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Housing> Housings { get; set; }
        public virtual DbSet<Journal> Journals { get; set; }
        public virtual DbSet<Journaltheme> Journalthemes { get; set; }
        public virtual DbSet<Periodclass> Periodclasses { get; set; }
        public virtual DbSet<Requestcabinet> Requestcabinets { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<TeachersHasGroup> TeachersHasGroups { get; set; }
        public virtual DbSet<Typeclass> Typeclasses { get; set; }
        public virtual DbSet<Typelearning> Typelearnings { get; set; }
        public virtual DbSet<Typescore> Typescores { get; set; }
        public virtual DbSet<Update> Updates { get; set; }
        public virtual DbSet<Weekday> Weekdays { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.

                string server = ElectroJournal.Properties.Settings.Default.Server;
                string username = ElectroJournal.Properties.Settings.Default.UserName;
                string password = ElectroJournal.Properties.Settings.Default.Password;
                string database = ElectroJournal.Properties.Settings.Default.DataBase;
                // Connection String.
                String connString = "Server=" + server + ";Database=" + database + ";User Id=" + username + ";password=" + password;

                optionsBuilder.UseMySql(connString, Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.26-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");

            modelBuilder.Entity<Basistraining>(entity =>
            {
                entity.HasKey(e => e.Idbasistraining)
                    .HasName("PRIMARY");

                entity.ToTable("basistraining");

                entity.HasIndex(e => e.Idbasistraining, "idbasistraining_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idbasistraining).HasColumnName("idbasistraining");

                entity.Property(e => e.BasistrainingName)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("basistraining_name")
                    .HasComment("тип группы - бюджет или коммерция");
            });

            modelBuilder.Entity<Cabinet>(entity =>
            {
                entity.HasKey(e => e.Idcabinet)
                    .HasName("PRIMARY");

                entity.ToTable("cabinet");

                entity.HasIndex(e => e.HousingIdhousing, "fk_cabinet_housing_idx");

                entity.HasIndex(e => e.Idcabinet, "idcabinet_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idcabinet).HasColumnName("idcabinet");

                entity.Property(e => e.CabinetFeatures)
                    .HasMaxLength(45)
                    .HasColumnName("cabinet_features")
                    .HasComment("особенности кабинета");

                entity.Property(e => e.CabinetFloor)
                    .HasMaxLength(3)
                    .HasColumnName("cabinet_floor")
                    .HasComment("этаж кабинета");

                entity.Property(e => e.CabinetName)
                    .HasMaxLength(45)
                    .HasColumnName("cabinet_name")
                    .HasComment("название кабинета");

                entity.Property(e => e.CabinetNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .HasColumnName("cabinet_number")
                    .HasComment("номер кабинета");

                entity.Property(e => e.CabinetNumberSeats)
                    .HasColumnName("cabinet_number_seats")
                    .HasComment("количество мест");

                entity.Property(e => e.HousingIdhousing).HasColumnName("housing_idhousing");

                entity.HasOne(d => d.HousingIdhousingNavigation)
                    .WithMany(p => p.Cabinets)
                    .HasForeignKey(d => d.HousingIdhousing)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_cabinet_housing");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.Idcourse)
                    .HasName("PRIMARY");

                entity.ToTable("course");

                entity.HasIndex(e => e.Idcourse, "idcourse_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idcourse).HasColumnName("idcourse");

                entity.Property(e => e.CourseName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("course_name");
            });

            modelBuilder.Entity<Date>(entity =>
            {
                entity.HasKey(e => e.IdDate)
                    .HasName("PRIMARY");

                entity.ToTable("dates");

                entity.HasIndex(e => e.Fulldate, "td_dbdate_idx")
                    .IsUnique();

                entity.HasIndex(e => new { e.Year, e.Month, e.Day }, "td_ymd_idx")
                    .IsUnique();

                entity.Property(e => e.IdDate)
                    .ValueGeneratedNever()
                    .HasColumnName("idDate");

                entity.Property(e => e.Day).HasColumnName("day");

                entity.Property(e => e.DayOfWeek).HasColumnName("dayOfWeek");

                entity.Property(e => e.Fulldate)
                    .HasColumnType("date")
                    .HasColumnName("fulldate");

                entity.Property(e => e.Month).HasColumnName("month");

                entity.Property(e => e.Quarter).HasColumnName("quarter");

                entity.Property(e => e.Week).HasColumnName("week");

                entity.Property(e => e.Weekend).HasColumnName("weekend");

                entity.Property(e => e.Year).HasColumnName("year");
            });

            modelBuilder.Entity<Discipline>(entity =>
            {
                entity.HasKey(e => e.Iddisciplines)
                    .HasName("PRIMARY");

                entity.ToTable("disciplines");

                entity.HasIndex(e => e.Iddisciplines, "iddisciplines_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Iddisciplines).HasColumnName("iddisciplines");

                entity.Property(e => e.DisciplinesIndex)
                    .HasColumnType("text")
                    .HasColumnName("disciplines_index");

                entity.Property(e => e.DisciplinesName)
                    .HasColumnType("text")
                    .HasColumnName("disciplines_name");

                entity.Property(e => e.DisciplinesNameAbbreviated)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("disciplines_name_abbreviated");
            });

            modelBuilder.Entity<Disciplinehour>(entity =>
            {
                entity.HasKey(e => e.Iddisciplinehours)
                    .HasName("PRIMARY");

                entity.ToTable("disciplinehours");

                entity.HasIndex(e => e.CourseIdcourse, "fk_disciplinehours_course1_idx");

                entity.HasIndex(e => e.DisciplinesIddisciplines, "fk_disciplinehours_disciplines1_idx");

                entity.HasIndex(e => e.GroupsIdgroups, "fk_disciplinehours_groups1_idx");

                entity.HasIndex(e => e.Iddisciplinehours, "iddisciplinehours_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Iddisciplinehours).HasColumnName("iddisciplinehours");

                entity.Property(e => e.CourseIdcourse).HasColumnName("course_idcourse");

                entity.Property(e => e.DisciplinehoursMadatory)
                    .HasColumnName("disciplinehours_madatory")
                    .HasComment("обязательные часы");

                entity.Property(e => e.DisciplinehoursSeparate)
                    .HasColumnName("disciplinehours_separate")
                    .HasComment("самостоятельные часы");

                entity.Property(e => e.DisciplinesIddisciplines).HasColumnName("disciplines_iddisciplines");

                entity.Property(e => e.GroupsIdgroups).HasColumnName("groups_idgroups");

                entity.HasOne(d => d.CourseIdcourseNavigation)
                    .WithMany(p => p.Disciplinehours)
                    .HasForeignKey(d => d.CourseIdcourse)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_disciplinehours_course1");

                entity.HasOne(d => d.DisciplinesIddisciplinesNavigation)
                    .WithMany(p => p.Disciplinehours)
                    .HasForeignKey(d => d.DisciplinesIddisciplines)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_disciplinehours_disciplines1");

                entity.HasOne(d => d.GroupsIdgroupsNavigation)
                    .WithMany(p => p.Disciplinehours)
                    .HasForeignKey(d => d.GroupsIdgroups)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_disciplinehours_groups1");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.Idgroups)
                    .HasName("PRIMARY");

                entity.ToTable("groups");

                entity.HasIndex(e => e.CourseIdcourse, "fk_groups_course1_idx");

                entity.HasIndex(e => e.TeachersIdteachers, "fk_groups_teachers1_idx");

                entity.HasIndex(e => e.TypelearningIdtypelearning, "fk_groups_typelearning1_idx");

                entity.HasIndex(e => e.Idgroups, "idgroups_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idgroups).HasColumnName("idgroups");

                entity.Property(e => e.CourseIdcourse).HasColumnName("course_idcourse");

                entity.Property(e => e.GroupsName)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("groups_name");

                entity.Property(e => e.GroupsNameAbbreviated)
                    .HasColumnType("text")
                    .HasColumnName("groups_name_abbreviated");

                entity.Property(e => e.GroupsPrefix)
                    .HasColumnType("text")
                    .HasColumnName("groups_prefix");

                entity.Property(e => e.TeachersIdteachers).HasColumnName("teachers_idteachers");

                entity.Property(e => e.TypelearningIdtypelearning).HasColumnName("typelearning_idtypelearning");

                entity.HasOne(d => d.CourseIdcourseNavigation)
                    .WithMany(p => p.Groups)
                    .HasForeignKey(d => d.CourseIdcourse)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_groups_course1");

                entity.HasOne(d => d.TypelearningIdtypelearningNavigation)
                    .WithMany(p => p.Groups)
                    .HasForeignKey(d => d.TypelearningIdtypelearning)
                    .HasConstraintName("fk_groups_typelearning1");
            });

            modelBuilder.Entity<Housing>(entity =>
            {
                entity.HasKey(e => e.Idhousing)
                    .HasName("PRIMARY");

                entity.ToTable("housing");

                entity.HasIndex(e => e.Idhousing, "idhousing_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idhousing).HasColumnName("idhousing");

                entity.Property(e => e.HousingAddress)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("housing_address");

                entity.Property(e => e.HousingCountCabinet).HasColumnName("housing_count_cabinet");

                entity.Property(e => e.HousingName)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("housing_name");
            });

            modelBuilder.Entity<Journal>(entity =>
            {
                entity.HasKey(e => e.Idjournal)
                    .HasName("PRIMARY");

                entity.ToTable("journal");

                entity.HasIndex(e => e.DisciplinesIddisciplines, "fk_journal_disciplines1_idx");

                entity.HasIndex(e => e.StudentsIdstudents, "fk_journal_students1_idx");

                entity.HasIndex(e => e.TeachersIdteachers, "fk_journal_teachers1_idx");

                entity.HasIndex(e => e.Idjournal, "idjournal_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idjournal).HasColumnName("idjournal");

                entity.Property(e => e.DisciplinesIddisciplines).HasColumnName("disciplines_iddisciplines");

                entity.Property(e => e.JournalDate)
                    .HasColumnType("date")
                    .HasColumnName("journal_date");

                entity.Property(e => e.JournalScore).HasColumnName("journal_score");

                entity.Property(e => e.StudentsIdstudents).HasColumnName("students_idstudents");

                entity.Property(e => e.TeachersIdteachers).HasColumnName("teachers_idteachers");

                entity.HasOne(d => d.DisciplinesIddisciplinesNavigation)
                    .WithMany(p => p.Journals)
                    .HasForeignKey(d => d.DisciplinesIddisciplines)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_journal_disciplines1");

                entity.HasOne(d => d.StudentsIdstudentsNavigation)
                    .WithMany(p => p.Journals)
                    .HasForeignKey(d => d.StudentsIdstudents)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_journal_students1");

                entity.HasOne(d => d.TeachersIdteachersNavigation)
                    .WithMany(p => p.Journals)
                    .HasForeignKey(d => d.TeachersIdteachers)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_journal_teachers1");
            });

            modelBuilder.Entity<Journaltheme>(entity =>
            {
                entity.HasKey(e => e.Idjournaltheme)
                    .HasName("PRIMARY");

                entity.ToTable("journaltheme");

                entity.HasIndex(e => e.DisciplinesIddisciplines, "fk_journaltheme_disciplines1_idx");

                entity.HasIndex(e => e.GroupsIdgroups, "fk_journaltheme_groups1_idx");

                entity.HasIndex(e => e.Idjournaltheme, "idjournaltheme_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idjournaltheme).HasColumnName("idjournaltheme");

                entity.Property(e => e.DisciplinesIddisciplines).HasColumnName("disciplines_iddisciplines");

                entity.Property(e => e.GroupsIdgroups).HasColumnName("groups_idgroups");

                entity.Property(e => e.JournalthemeDate)
                    .HasColumnType("date")
                    .HasColumnName("journaltheme_date");

                entity.Property(e => e.JournalthemeHomework)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("journaltheme_homework");

                entity.Property(e => e.JournalthemeHours).HasColumnName("journaltheme_hours");

                entity.Property(e => e.JournalthemeName)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("journaltheme_name");

                entity.HasOne(d => d.DisciplinesIddisciplinesNavigation)
                    .WithMany(p => p.Journalthemes)
                    .HasForeignKey(d => d.DisciplinesIddisciplines)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_journaltheme_disciplines1");

                entity.HasOne(d => d.GroupsIdgroupsNavigation)
                    .WithMany(p => p.Journalthemes)
                    .HasForeignKey(d => d.GroupsIdgroups)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_journaltheme_groups1");
            });

            modelBuilder.Entity<Periodclass>(entity =>
            {
                entity.HasKey(e => e.Idperiodclasses)
                    .HasName("PRIMARY");

                entity.ToTable("periodclasses");

                entity.HasIndex(e => e.Idperiodclasses, "idperiodclasses_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idperiodclasses).HasColumnName("idperiodclasses");

                entity.Property(e => e.PeriodclassesEnd)
                    .HasColumnType("time")
                    .HasColumnName("periodclasses_end");

                entity.Property(e => e.PeriodclassesNumber).HasColumnName("periodclasses_number");

                entity.Property(e => e.PeriodclassesStart)
                    .HasColumnType("time")
                    .HasColumnName("periodclasses_start");
            });

            modelBuilder.Entity<Requestcabinet>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("requestcabinet");

                entity.Property(e => e.CabinetFeatures)
                    .HasMaxLength(45)
                    .HasColumnName("cabinet_features")
                    .HasComment("особенности кабинета");

                entity.Property(e => e.CabinetFloor)
                    .HasMaxLength(3)
                    .HasColumnName("cabinet_floor")
                    .HasComment("этаж кабинета");

                entity.Property(e => e.CabinetName)
                    .HasMaxLength(45)
                    .HasColumnName("cabinet_name")
                    .HasComment("название кабинета");

                entity.Property(e => e.CabinetNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .HasColumnName("cabinet_number")
                    .HasComment("номер кабинета");

                entity.Property(e => e.CabinetNumberSeats)
                    .HasColumnName("cabinet_number_seats")
                    .HasComment("количество мест");

                entity.Property(e => e.Idcabinet).HasColumnName("idcabinet");

                entity.Property(e => e.IfnullHousingName)
                    .IsRequired()
                    .HasColumnType("mediumtext")
                    .HasColumnName("IFNULL(housing_name, '')");
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.HasKey(e => e.Idschedule)
                    .HasName("PRIMARY");

                entity.ToTable("schedule");

                entity.HasIndex(e => e.CabinetIdcabinet, "fk_schedule_cabinet1_idx");

                entity.HasIndex(e => e.GroupsIdgroups, "fk_schedule_groups1_idx");

                entity.HasIndex(e => e.PeriodclassesIdperiodclasses, "fk_schedule_periodclasses1_idx");

                entity.HasIndex(e => e.TeachersIdteachers, "fk_schedule_teachers1_idx");

                entity.HasIndex(e => e.TypeclassesIdtypeclasses, "fk_schedule_typeclasses1_idx");

                entity.HasIndex(e => e.WeekdayIdweekday, "fk_schedule_weekday1_idx");

                entity.HasIndex(e => e.Idschedule, "idschedule_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idschedule).HasColumnName("idschedule");

                entity.Property(e => e.CabinetIdcabinet).HasColumnName("cabinet_idcabinet");

                entity.Property(e => e.GroupsIdgroups).HasColumnName("groups_idgroups");

                entity.Property(e => e.PeriodclassesIdperiodclasses).HasColumnName("periodclasses_idperiodclasses");

                entity.Property(e => e.ScheduleDate)
                    .HasColumnType("date")
                    .HasColumnName("schedule_date");

                entity.Property(e => e.TeachersIdteachers).HasColumnName("teachers_idteachers");

                entity.Property(e => e.TypeclassesIdtypeclasses).HasColumnName("typeclasses_idtypeclasses");

                entity.Property(e => e.WeekdayIdweekday).HasColumnName("weekday_idweekday");

                entity.HasOne(d => d.CabinetIdcabinetNavigation)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.CabinetIdcabinet)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_schedule_cabinet1");

                entity.HasOne(d => d.GroupsIdgroupsNavigation)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.GroupsIdgroups)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_schedule_groups1");

                entity.HasOne(d => d.PeriodclassesIdperiodclassesNavigation)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.PeriodclassesIdperiodclasses)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_schedule_periodclasses1");

                entity.HasOne(d => d.TeachersIdteachersNavigation)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.TeachersIdteachers)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_schedule_teachers1");

                entity.HasOne(d => d.TypeclassesIdtypeclassesNavigation)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.TypeclassesIdtypeclasses)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_schedule_typeclasses1");

                entity.HasOne(d => d.WeekdayIdweekdayNavigation)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.WeekdayIdweekday)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_schedule_weekday1");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Idstudents)
                    .HasName("PRIMARY");

                entity.ToTable("students");

                entity.HasIndex(e => e.GroupsIdgroups, "fk_students_groups1_idx");

                entity.HasIndex(e => e.Idstudents, "idstudents_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idstudents).HasColumnName("idstudents");

                entity.Property(e => e.GroupsIdgroups).HasColumnName("groups_idgroups");

                entity.Property(e => e.StudentsBirthday)
                    .HasColumnType("date")
                    .HasColumnName("students_birthday");

                entity.Property(e => e.StudentsDormitory)
                    .HasMaxLength(5)
                    .HasColumnName("students_dormitory")
                    .HasComment("общежитие\\\\n");

                entity.Property(e => e.StudentsName)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("students_name");

                entity.Property(e => e.StudentsParent)
                    .HasColumnType("text")
                    .HasColumnName("students_parent");

                entity.Property(e => e.StudentsParentPhone)
                    .HasMaxLength(12)
                    .HasColumnName("students_parent_phone");

                entity.Property(e => e.StudentsPatronymic)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("students_patronymic");

                entity.Property(e => e.StudentsPhone)
                    .HasMaxLength(12)
                    .HasColumnName("students_phone");

                entity.Property(e => e.StudentsResidence)
                    .HasColumnType("text")
                    .HasColumnName("students_residence")
                    .HasComment("место жительства\n");

                entity.Property(e => e.StudentsSurname)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("students_surname");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(e => e.Idteachers)
                    .HasName("PRIMARY");

                entity.ToTable("teachers");

                entity.HasIndex(e => e.Idteachers, "idteachers_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idteachers).HasColumnName("idteachers");

                entity.Property(e => e.TeachersAccesAdminPanel)
                    .IsRequired()
                    .HasMaxLength(5)
                    .HasColumnName("teachers_acces_admin_panel");

                entity.Property(e => e.TeachersImage)
                    .HasColumnType("mediumblob")
                    .HasColumnName("teachers_image");

                entity.Property(e => e.TeachersLogin)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("teachers_login");

                entity.Property(e => e.TeachersMail)
                    .HasColumnType("text")
                    .HasColumnName("teachers_mail");

                entity.Property(e => e.TeachersName)
                    .HasMaxLength(45)
                    .HasColumnName("teachers_name");

                entity.Property(e => e.TeachersPassword)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("teachers_password");

                entity.Property(e => e.TeachersPatronymic)
                    .HasMaxLength(45)
                    .HasColumnName("teachers_patronymic");

                entity.Property(e => e.TeachersPhone)
                    .HasMaxLength(12)
                    .HasColumnName("teachers_phone");

                entity.Property(e => e.TeachersSurname)
                    .HasMaxLength(45)
                    .HasColumnName("teachers_surname");
            });

            modelBuilder.Entity<TeachersHasGroup>(entity =>
            {
                entity.HasKey(e => new { e.TeachersIdteachers, e.GroupsIdgroups })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("teachers_has_groups");

                entity.HasIndex(e => e.GroupsIdgroups, "fk_teachers_has_groups_groups1_idx");

                entity.HasIndex(e => e.TeachersIdteachers, "fk_teachers_has_groups_teachers1_idx");

                entity.Property(e => e.TeachersIdteachers).HasColumnName("teachers_idteachers");

                entity.Property(e => e.GroupsIdgroups).HasColumnName("groups_idgroups");

                entity.HasOne(d => d.GroupsIdgroupsNavigation)
                    .WithMany(p => p.TeachersHasGroups)
                    .HasForeignKey(d => d.GroupsIdgroups)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_teachers_has_groups_groups1");

                entity.HasOne(d => d.TeachersIdteachersNavigation)
                    .WithMany(p => p.TeachersHasGroups)
                    .HasForeignKey(d => d.TeachersIdteachers)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_teachers_has_groups_teachers1");
            });

            modelBuilder.Entity<Typeclass>(entity =>
            {
                entity.HasKey(e => e.Idtypeclasses)
                    .HasName("PRIMARY");

                entity.ToTable("typeclasses");

                entity.HasIndex(e => e.Idtypeclasses, "idtypeclasses_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idtypeclasses).HasColumnName("idtypeclasses");

                entity.Property(e => e.TypeclassesName)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("typeclasses_name");
            });

            modelBuilder.Entity<Typelearning>(entity =>
            {
                entity.HasKey(e => e.Idtypelearning)
                    .HasName("PRIMARY");

                entity.ToTable("typelearning");

                entity.HasIndex(e => e.Idtypelearning, "idtypelearning_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idtypelearning).HasColumnName("idtypelearning");

                entity.Property(e => e.TypelearningName)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("typelearning_name")
                    .HasComment("форма обучения");
            });

            modelBuilder.Entity<Typescore>(entity =>
            {
                entity.HasKey(e => e.Idtypescore)
                    .HasName("PRIMARY");

                entity.ToTable("typescore");

                entity.HasIndex(e => e.Idtypescore, "idtypescore_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idtypescore)
                    .ValueGeneratedNever()
                    .HasColumnName("idtypescore");

                entity.Property(e => e.TypescoreName)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("typescore_name");
            });

            modelBuilder.Entity<Update>(entity =>
            {
                entity.HasKey(e => e.Idupdate)
                    .HasName("PRIMARY");

                entity.ToTable("update");

                entity.HasIndex(e => e.Idupdate, "idupdate_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idupdate).HasColumnName("idupdate");

                entity.Property(e => e.UpdateNew)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("update_new");

                entity.Property(e => e.UpdateVersion)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("update_version");
            });

            modelBuilder.Entity<Weekday>(entity =>
            {
                entity.HasKey(e => e.Idweekday)
                    .HasName("PRIMARY");

                entity.ToTable("weekday");

                entity.HasIndex(e => e.Idweekday, "idweekday_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Idweekday).HasColumnName("idweekday");

                entity.Property(e => e.WeekdayName)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("weekday_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}