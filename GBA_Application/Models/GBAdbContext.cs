using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GBA_Application.Models
{
    public partial class GBAdbContext : DbContext
    {
        public GBAdbContext()
        {
        }

        public GBAdbContext(DbContextOptions<GBAdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Appointments> Appointments { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<Members> Members { get; set; }
        public virtual DbSet<OrderStatuses> OrderStatuses { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<Services> Services { get; set; }
        public virtual DbSet<Statuses> Statuses { get; set; }
        public virtual DbSet<Vehicles> Vehicles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=SLOTH;Database=GBAdb;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointments>(entity =>
            {
                entity.HasKey(e => e.AppointmentId);

                entity.ToTable("appointments");

                entity.Property(e => e.AppointmentId).HasColumnName("Appointment_Id");

                entity.Property(e => e.AppointmentDate).HasColumnType("date");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.MemberId).HasColumnName("Member_id");

                entity.Property(e => e.ServiceId).HasColumnName("Service_Id");

                entity.Property(e => e.VehicleId).HasColumnName("Vehicle_Id");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.MemberId)
                    .HasConstraintName("FK_appointments_members");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK_appointments_services");

                entity.HasOne(d => d.Vehicle)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.VehicleId)
                    .HasConstraintName("FK_appointments_vehicles");
            });

            modelBuilder.Entity<Employees>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);

                entity.ToTable("employees");

                entity.Property(e => e.EmployeeId).HasColumnName("Employee_Id");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Members>(entity =>
            {
                entity.HasKey(e => e.MemberId);

                entity.ToTable("members");

                entity.Property(e => e.MemberId).HasColumnName("Member_Id");

                entity.Property(e => e.Address)
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.Province)
                    .HasMaxLength(2)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<OrderStatuses>(entity =>
            {
                entity.ToTable("order_statuses");

                entity.Property(e => e.OrderStatusesId).HasColumnName("Order_Statuses_Id");

                entity.Property(e => e.EmployeeId).HasColumnName("Employee_Id");

                entity.Property(e => e.EstimatedFinishDate).HasColumnType("date");

                entity.Property(e => e.FinishDate).HasColumnType("date");

                entity.Property(e => e.OrderId).HasColumnName("Order_Id");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.StatusId).HasColumnName("Status_Id");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.OrderStatuses)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_order_statuses_employees");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderStatuses)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_order_statuses_orders");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.OrderStatuses)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_order_statuses_statuses");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.ToTable("orders");

                entity.Property(e => e.OrderId).HasColumnName("Order_Id");

                entity.Property(e => e.AppointmentId).HasColumnName("Appointment_Id");

                entity.Property(e => e.OrderDate).HasColumnType("date");

                entity.Property(e => e.OrderDescription).IsUnicode(false);

                entity.Property(e => e.OrderTotal).HasColumnType("decimal(2, 0)");

                entity.HasOne(d => d.Appointment)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.AppointmentId)
                    .HasConstraintName("FK_orders_appointments");
            });

            modelBuilder.Entity<Services>(entity =>
            {
                entity.HasKey(e => e.ServiceId);

                entity.ToTable("services");

                entity.Property(e => e.ServiceId).HasColumnName("Service_Id");

                entity.Property(e => e.Cost).HasColumnType("decimal(2, 0)");

                entity.Property(e => e.Description)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TimeToComplete)
                    .HasMaxLength(35)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Statuses>(entity =>
            {
                entity.HasKey(e => e.StatusId);

                entity.ToTable("statuses");

                entity.Property(e => e.StatusId).HasColumnName("Status_Id");

                entity.Property(e => e.StatusDescription)
                    .HasMaxLength(75)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Vehicles>(entity =>
            {
                entity.HasKey(e => e.VehicleId);

                entity.ToTable("vehicles");

                //entity.Property(e => e.VehicleId)
                //    .HasColumnName("Vehicle_Id")
                //    .ValueGeneratedOnAdd();

                entity.Property(e => e.LicensePlate)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Make)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.VehicleId).HasColumnName("Vehicle_Id");
                
                entity.Property(e => e.MemberId).HasColumnName("Member_Id");

                entity.Property(e => e.Model)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Year)
                    .HasMaxLength(4)
                    .IsUnicode(false);

                //entity.HasOne(d => d.Vehicle)
                //    .WithOne(p => p.Vehicles)
                //    .HasForeignKey<Vehicles>(d => d.VehicleId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_vehicles_members");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.Vehicles)
                    .HasForeignKey(d => d.MemberId)
                    .HasConstraintName("FK_vehicles_members");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
