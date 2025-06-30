using Microsoft.EntityFrameworkCore;
using MedicalApiSimulator.Models;

namespace MedicalApiSimulator.Data
{
    /// <summary>
    /// Represents the database context for the Medical API Simulator.
    /// Manages entities including Patients, Medical Devices, and Vitals.
    /// Configures relationships and seeds initial mock data.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        /// <summary>
        /// Gets or sets the Patients table.
        /// </summary>
        public DbSet<Patient> Patients { get; set; } = null!;
        
        /// <summary>
        /// Gets or sets the Medical Devices table.
        /// </summary>
        public DbSet<MedicalDevice> MedicalDevices { get; set; } = null!;
        
        /// <summary>
        /// Gets or sets the Vitals table.
        /// </summary>
        public DbSet<Vital> Vitals { get; set; } = null!;
        
        /// <summary>
        /// Configures the entity relationships and seeds initial data.
        /// </summary>
        /// <param name="modelBuilder">The builder used to configure entities.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<MedicalDevice>()
            .HasMany(d => d.Vitals)
            .WithOne(v => v.MedicalDevice)
            .HasForeignKey(v => v.MedicalDeviceId)
            .OnDelete(DeleteBehavior.Cascade);


            // Seed mock data

            // Seed Patients
            modelBuilder.Entity<Patient>().HasData(
                new Patient { Id = 1, FullName = "Alice Thompson", Age = 45, Gender = "Female", Diagnosis = "Hypertension", LastCheckup = new DateTime(2024, 6, 1) },
                new Patient { Id = 2, FullName = "John Smith", Age = 60, Gender = "Male", Diagnosis = "Diabetes", LastCheckup = new DateTime(2024, 5, 15) },
                new Patient { Id = 3, FullName = "Linda Park", Age = 32, Gender = "Female", Diagnosis = "Asthma", LastCheckup = new DateTime(2024, 6, 20) }
            );
            // Seed MedicalDevices
            modelBuilder.Entity<MedicalDevice>().HasData(
                new MedicalDevice { Id = 1, DeviceName = "Heart Monitor A100", DeviceType = "Heart Rate Monitor" },
                new MedicalDevice { Id = 2, DeviceName = "Blood Pressure Cuff X2", DeviceType = "Blood Pressure Monitor" }
            );

            // Seed Vitals
            modelBuilder.Entity<Vital>().HasData(
            new Vital
            {
                Id = 1,
                PatientId = 1,
                MedicalDeviceId = 1,
                Timestamp = new DateTime(2025, 6, 20, 8, 30, 0),
                HeartRate = 72,
                BloodPressureSystolic = 120,
                BloodPressureDiastolic = 80,
                OxygenSaturation = 98,
                Temperature = 36.6f
            },
            new Vital
            {
                Id = 2,
                PatientId = 2,
                MedicalDeviceId = 2,
                Timestamp = new DateTime(2025, 6, 20, 8, 45, 0),
                HeartRate = 75,
                BloodPressureSystolic = 130,
                BloodPressureDiastolic = 85,
                OxygenSaturation = 97,
                Temperature = 37.0f
            }
            );
        }
    }
    
}