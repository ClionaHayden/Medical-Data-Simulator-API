using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalApiSimulator.Data;
using MedicalApiSimulator.Models;
using Microsoft.AspNetCore.Authorization;

namespace MedicalApiSimulator.Controllers
{
    /// <summary>
    /// Controller to manage patient vitals data.
    /// Supports CRUD operations and querying vitals by patient.
    /// Secured with JWT authentication and role-based authorization.
    /// Only Admins can create, update, or delete vital records. Users have read-only access to certain endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VitalsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VitalsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all vitals including related patient and device information.
        /// </summary>
        /// <returns>List of all vitals.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vital>>> GetVitals(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var totalCount = await _context.Vitals.CountAsync();

            // Include related Patient and MedicalDevice data for convenience
            var vitals = await _context.Vitals
                .Include(v => v.Patient)
                .Include(v => v.MedicalDevice)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers["X-Total-Count"] = totalCount.ToString();
            Response.Headers["X-Page-Number"] = pageNumber.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return vitals;
    
        }

        /// <summary>
        /// Gets a single vital record by ID including related patient and device.
        /// </summary>
        /// <param name="id">Vital ID.</param>
        /// <returns>Vital record if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Vital>> GetVital(int id)
        {
            var vital = await _context.Vitals
                .Include(v => v.Patient)
                .Include(v => v.MedicalDevice)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vital == null) return NotFound();
            return vital;
        }

        /// <summary>
        /// Gets all vitals for a specific patient.
        /// </summary>
        /// <param name="patientId">Patient ID.</param>
        /// <returns>List of vitals associated with the patient.</returns>
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Vital>>> GetVitalsByPatient(int patientId)
        {
            var vitals = await _context.Vitals
                .Where(v => v.PatientId == patientId)
                .Include(v => v.MedicalDevice)
                .ToListAsync();

            return vitals;
        }

        /// <summary>
        /// Creates a new vital record.
        /// Restricted to users with Admin role.
        /// </summary>
        /// <param name="vital">Vital data to create.</param>
        /// <returns>The created vital with location header.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<Vital>> CreateVital(Vital vital)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Vitals.Add(vital);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVital), new { id = vital.Id }, vital);
        }

        /// <summary>
        /// Updates an existing vital record by ID.
        /// Restricted to users with Admin role.
        /// </summary>
        /// <param name="id">Vital ID to update.</param>
        /// <param name="vital">Updated vital data.</param>
        /// <returns>No content on success or appropriate error.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Restrict updates to Admins
        public async Task<IActionResult> UpdateVital(int id, Vital vital)
        {
            if (id != vital.Id)
                return BadRequest();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Entry(vital).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VitalExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a vital record by ID.
        /// Restricted to users with Admin role.
        /// </summary>
        /// <param name="id">Vital ID to delete.</param>
        /// <returns>No content on success or NotFound.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Restrict deletes to Admins
        public async Task<IActionResult> DeleteVital(int id)
        {
            var vital = await _context.Vitals.FindAsync(id);
            if (vital == null)
                return NotFound();

            _context.Vitals.Remove(vital);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if a vital record exists by ID.
        /// </summary>
        /// <param name="id">Vital ID.</param>
        /// <returns>True if exists, false otherwise.</returns>
        private bool VitalExists(int id)
        {
            return _context.Vitals.Any(v => v.Id == id);
        }
    
    }
}
