using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalApiSimulator.Data;
using MedicalApiSimulator.Models;
using Microsoft.AspNetCore.Authorization;

namespace MedicalApiSimulator.Controllers
{
    /// <summary>
    /// Controller to manage medical devices and their associated vitals data.
    /// Supports CRUD operations on devices and allows retrieval and creation of vitals tied to devices.
    /// Secured with JWT authentication and role-based authorization.
    /// Admins have full access. Users have read-only access to certain endpoints.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicalDevicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MedicalDevicesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all medical devices.
        /// </summary>
        /// <returns>A list of all medical devices.</returns>
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalDevice>>> GetDevices(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var totalCount = await _context.MedicalDevices.CountAsync();

            var devices = await _context.MedicalDevices
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers["X-Total-Count"] = totalCount.ToString();
            Response.Headers["X-Page-Number"] = pageNumber.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return devices;
        }

        /// <summary>
        /// Creates a new medical device.
        /// Restricted to users with Admin role.
        /// </summary>
        /// <param name="device">The medical device to create.</param>
        /// <returns>The created device with its generated ID.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateDevice([FromBody] MedicalDevice device)
        {
            if (device == null)
                return BadRequest("Device data is required.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.MedicalDevices.Add(device);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, device);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while saving the device.");
            }
        }

        /// <summary>
        /// Gets a medical device by its ID.
        /// </summary>
        /// <param name="id">The ID of the device to retrieve.</param>
        /// <returns>The device if found; otherwise, NotFound.</returns>
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeviceById(int id)
        {
            var device = await _context.MedicalDevices.FindAsync(id);
            if (device == null)
                return NotFound();
            return Ok(device);
        }

        /// <summary>
        /// Gets all vitals recorded by a specific device.
        /// </summary>
        /// <param name="id">The ID of the medical device.</param>
        /// <returns>A list of vitals related to the device.</returns>
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}/vitals")]
        public async Task<IActionResult> GetDeviceVitals(int id)
        {
            var device = await _context.MedicalDevices
                .Include(d => d.Vitals)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
                return NotFound();

            return Ok(device.Vitals);
        }

        /// <summary>
        /// Updates an existing medical device.
        /// Restricted to users with Admin role.
        /// </summary>
        /// <param name="id">The ID of the device to update.</param>
        /// <param name="updatedDevice">The updated device data.</param>
        /// <returns>NoContent on success, or appropriate error response.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevice(int id, [FromBody] MedicalDevice updatedDevice)
        {
            if (id != updatedDevice.Id)
                return BadRequest("Device ID mismatch.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingDevice = await _context.MedicalDevices.FindAsync(id);
            if (existingDevice == null)
                return NotFound();

            existingDevice.DeviceName = updatedDevice.DeviceName;
            existingDevice.DeviceType = updatedDevice.DeviceType;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the device.");
            }
        }

        /// <summary>
        /// Adds a vital record to a medical device.
        /// Restricted to users with Admin role.
        /// </summary>
        /// <param name="id">The ID of the medical device.</param>
        /// <param name="vital">The vital record to add.</param>
        /// <returns>The created vital record.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/vitals")]
        public async Task<IActionResult> AddVitalToDevice(int id, [FromBody] Vital vital)
        {
            var device = await _context.MedicalDevices.FindAsync(id);
            if (device == null)
                return NotFound();

            vital.MedicalDeviceId = id; // set FK if not already set
            _context.Vitals.Add(vital);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDeviceVitals), new { id = id }, vital);
        }

        /// <summary>
        /// Deletes a medical device.
        /// Restricted to users with Admin role.
        /// </summary>
        /// <param name="id">The ID of the device to delete.</param>
        /// <returns>NoContent on success, or NotFound if device does not exist.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            var device = await _context.MedicalDevices.FindAsync(id);
            if (device == null)
                return NotFound();

            _context.MedicalDevices.Remove(device);

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while deleting the device.");
            }
        }


        /// <summary>
        /// Checks if a medical device exists in the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the device to check.</param>
        /// <returns>True if the device exists; otherwise, false.</returns>
        private bool DeviceExists(int id)
        {
            return _context.MedicalDevices.Any(d => d.Id == id);
        }
    }
}
