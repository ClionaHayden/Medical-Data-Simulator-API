using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalApiSimulator.Data;
using MedicalApiSimulator.Models;
using Microsoft.AspNetCore.Authorization;

namespace MedicalApiSimulator.Controllers
{
    /// <summary>
    /// Controller to manage patient records.
    /// Supports CRUD operations including creating, updating, retrieving, and deleting patients.
    /// Secured with JWT authentication and role-based authorization.
    /// Only Admins can create, update, or delete patients. Users have read-only access to certain endpoints.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PatientsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all patients.
        /// </summary>
        /// <returns>A list of patients.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var totalCount = await _context.Patients.CountAsync();

            var patients = await _context.Patients
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers["X-Total-Count"] = totalCount.ToString();
            Response.Headers["X-Page-Number"] = pageNumber.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return patients;
        }

        /// <summary>
        /// Retrieves a patient by their unique ID.
        /// </summary>
        /// <param name="id">The ID of the patient to retrieve.</param>
        /// <returns>The patient details if found; otherwise, a 404 Not Found response.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            return patient;
        }

        /// <summary>
        /// Creates a new patient.
        /// Restricted to users with Admin role.
        /// </summary>
        /// <param name="patient">The patient object to create.</param>
        /// <returns>The created patient object.</returns>
        /// <response code="201">Returns the newly created patient.</response>
        /// <response code="400">If the patient object is invalid.</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]  // Only admins can create
        public async Task<ActionResult<Patient>> CreatePatient(Patient patient)
        {
            if (patient == null)
                return BadRequest();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        /// <summary>
        /// Updates an existing patient.
        /// Restricted to users with Admin role.
        /// </summary>
        /// <param name="id">The id of the patient to update.</param>
        /// <param name="patient">The updated patient object.</param>
        /// <returns>No content if successful; otherwise, appropriate error response.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]  // Only admins can update
        public async Task<IActionResult> UpdatePatient(int id, Patient patient)
        {
            if (id != patient.Id)
                return BadRequest();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Entry(patient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a patient by id.
        /// Restricted to users with Admin role.
        /// </summary>
        /// <param name="id">The id of the patient to delete.</param>
        /// <returns>No content if successful; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  // Only admins can delete
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if a patient with the specified ID exists in the database.
        /// </summary>
        /// <param name="id">The ID of the patient to check.</param>
        /// <returns>True if the patient exists; otherwise, false.</returns>
        private bool PatientExists(int id)
        {
            return _context.Patients.Any(p => p.Id == id);
        }

    }
}