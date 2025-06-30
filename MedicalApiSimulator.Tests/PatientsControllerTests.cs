using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using MedicalApiSimulator.Controllers;
using MedicalApiSimulator.Data;
using MedicalApiSimulator.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class PatientsControllerTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // <-- Unique DB name
        .Options;

        var context = new AppDbContext(options);

        // Seed test data with fixed IDs (safe now)
        context.Patients.AddRange(
            new Patient { Id = 1, FullName = "Test Patient 1", Age = 30, Gender = "Male", Diagnosis = "Testing" },
            new Patient { Id = 2, FullName = "Test Patient 2", Age = 40, Gender = "Female", Diagnosis = "Sample" }
        );
        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task GetPatient_ReturnsPatient_WhenPatientExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new PatientsController(context);

        // Act
        var result = await controller.GetPatient(1);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Patient>>(result);
        var patient = Assert.IsType<Patient>(actionResult.Value);
        Assert.Equal(1, patient.Id);
    }

    [Fact]
    public async Task GetPatient_ReturnsNotFound_WhenPatientDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new PatientsController(context);

        // Act
        var result = await controller.GetPatient(999); // ID that doesn't exist

        // Assert
        var actionResult = Assert.IsType<ActionResult<Patient>>(result);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }
    [Fact]
    public async Task PostPatient_ReturnsBadRequest_WhenModelInvalid()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var controller = new PatientsController(dbContext);

        var patient = new Patient
        {
            // FullName is missing
            Age = 30,
            Gender = "Male",
            Diagnosis = "Hypertension",
            LastCheckup = DateTime.Now
        };

        controller.ModelState.AddModelError("FullName", "Full name is required.");

        // Act
        var result = await controller.CreatePatient(patient);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var error = Assert.IsType<SerializableError>(badRequestResult.Value);
        Assert.True(error.ContainsKey("FullName"));
    }
    
    [Fact]
    public async Task PutPatient_UpdatesPatient_WhenDataIsValid()
    {
        using var context = GetInMemoryDbContext();
        var controller = new PatientsController(context);

        var patient = await context.Patients.FirstAsync();
        patient.FullName = "Updated Name";

        var result = await controller.UpdatePatient(patient.Id, patient);

        Assert.IsType<NoContentResult>(result);
        var updatedPatient = await context.Patients.FindAsync(patient.Id);
        Assert.Equal("Updated Name", updatedPatient.FullName);
    }

    [Fact]
    public async Task PutPatient_ReturnsNotFound_WhenPatientDoesNotExist()
    {
        using var context = GetInMemoryDbContext();
        var controller = new PatientsController(context);

        var nonExistentId = 9999;
        var patient = new Patient { Id = nonExistentId, FullName = "Non Existent" };

        var result = await controller.UpdatePatient(nonExistentId, patient);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task PutPatient_ReturnsBadRequest_WhenIdsMismatch()
    {
        using var context = GetInMemoryDbContext();
        var controller = new PatientsController(context);

        var patient = await context.Patients.FirstAsync();
        var result = await controller.UpdatePatient(patient.Id + 1, patient);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeletePatient_DeletesPatient_WhenExists()
    {
        using var context = GetInMemoryDbContext();
        var controller = new PatientsController(context);

        var patient = await context.Patients.FirstAsync();

        var result = await controller.DeletePatient(patient.Id);

        Assert.IsType<NoContentResult>(result);

        var deletedPatient = await context.Patients.FindAsync(patient.Id);
        Assert.Null(deletedPatient);
    }

    [Fact]
    public async Task DeletePatient_ReturnsNotFound_WhenPatientDoesNotExist()
    {
        using var context = GetInMemoryDbContext();
        var controller = new PatientsController(context);

        var result = await controller.DeletePatient(9999);

        Assert.IsType<NotFoundResult>(result);
    }
}
