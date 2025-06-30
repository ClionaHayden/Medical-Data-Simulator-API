using System.Text.Json.Serialization;
namespace MedicalApiSimulator.Models
{
    /// <summary>
    /// Represents a patient in the medical system.
    /// </summary>
    public class Patient
    {
        /// <summary>
        /// Unique identifier for the patient.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Full name of the patient.
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        
        /// <summary>
        /// Age of the patient.
        /// </summary>
        public int Age { get; set; }
        
        /// <summary>
        /// Gender of the patient.
        /// </summary>
        public string Gender { get; set; } = string.Empty;
        
        /// <summary>
        /// Current diagnosis of the patient.
        /// </summary>
        public string Diagnosis { get; set; } = string.Empty;
        
        /// <summary>
        /// Date of the patient's last checkup.
        /// </summary>
        public DateTime LastCheckup { get; set; }

        /// <summary>
        /// Collection of vitals related to this patient.
        /// Marked with <see cref="JsonIgnore"/> to avoid circular references during serialization.
        /// </summary>
        [JsonIgnore] 
        public ICollection<Vital> Vitals { get; set; } = new List<Vital>();
    }
}