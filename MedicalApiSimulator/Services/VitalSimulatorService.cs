using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MedicalApiSimulator.Data;
using MedicalApiSimulator.Models;
using System.Linq;

/// <summary>
/// Background service that simulates vital sign data for patients by periodically generating
/// randomized vital measurements and saving them to the database.
/// </summary>
public class VitalSimulatorService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly Random _random = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="VitalSimulatorService"/> class.
    /// </summary>
    /// <param name="services">The service provider used to create scoped services such as the database context.</param>
    public VitalSimulatorService(IServiceProvider services)
    {
        _services = services;
    }

    /// <summary>
    /// Executes the background task that simulates vitals indefinitely until cancellation is requested.
    /// It retrieves all patients and medical devices, then generates and stores random vital sign data
    /// for each patient at regular intervals.
    /// </summary>
    /// <param name="stoppingToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task representing the background operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var patients = await db.Patients.ToListAsync(stoppingToken);
                var devices = await db.MedicalDevices.ToListAsync(stoppingToken);

                foreach (var patient in patients)
                {
                    var device = devices[_random.Next(devices.Count)];

                    var vital = new Vital
                    {
                        PatientId = patient.Id,
                        MedicalDeviceId = device.Id,
                        Timestamp = DateTime.UtcNow,
                        HeartRate = 60 + _random.Next(40), // 60-100 bpm
                        BloodPressureSystolic = 110 + _random.Next(30), // 110-140 mmHg
                        BloodPressureDiastolic = 70 + _random.Next(20), // 70-90 mmHg
                        OxygenSaturation = 95 + _random.Next(5), // 95-99%
                        Temperature = 36 + (float)_random.NextDouble() * 2 // 36-38 °C
                    };

                    db.Vitals.Add(vital);
                }

                await db.SaveChangesAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        catch (Exception ex)
        {
            // Log error - for now just Console
            Console.WriteLine($"Error in VitalSimulatorService: {ex.Message}");
        }

    }
}
