using System.Text.Json.Serialization;
namespace MedicalApiSimulator.Models
{
    /// <summary>
    /// Represents a vital sign measurement for a patient, collected by a medical device.
    /// </summary>
    public class Vital
    {
        /// <summary>
        /// Gets or sets the unique identifier for the vital record.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the foreign key referencing the patient who this vital belongs to.
        /// </summary>
        public int PatientId { get; set; }
        
        /// <summary>
        /// Gets or sets the foreign key referencing the medical device that collected this vital.
        /// </summary>
        public int MedicalDeviceId { get; set; }
        
        /// <summary>
        /// Gets or sets the timestamp when the vital measurement was recorded.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the patient's heart rate in beats per minute.
        /// </summary>
        public float HeartRate { get; set; }
        
        /// <summary>
        /// Gets or sets the systolic blood pressure value (mmHg).
        /// </summary>
        public float BloodPressureSystolic { get; set; }
        
        /// <summary>
        /// Gets or sets the diastolic blood pressure value (mmHg).
        /// </summary>
        public float BloodPressureDiastolic { get; set; }
        
        /// <summary>
        /// Gets or sets the oxygen saturation percentage.
        /// </summary>
        public float OxygenSaturation { get; set; }
        
        /// <summary>
        /// Gets or sets the body temperature in degrees Celsius.
        /// </summary>
        public float Temperature { get; set; }

        /// <summary>
        /// Navigation property to the associated patient.
        /// Marked with <see cref="JsonIgnore"/> to avoid circular references during serialization.
        /// </summary>
        [JsonIgnore]      
        public Patient Patient { get; set; } = null!;
        
        /// <summary>
        /// Navigation property to the associated medical device.
        /// Marked with <see cref="JsonIgnore"/> to avoid circular references during serialization.
        /// </summary>
        [JsonIgnore]
        public MedicalDevice MedicalDevice { get; set; } = null!;
    }
}