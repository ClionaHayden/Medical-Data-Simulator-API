namespace MedicalApiSimulator.Models
{
    /// <summary>
    /// Represents a medical device used for monitoring patient vitals.
    /// </summary>
    public class MedicalDevice
    {
        /// <summary>
        /// Unique identifier for the medical device.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The name or model of the medical device.
        /// </summary>
        public string DeviceName { get; set; } = string.Empty;
        
        /// <summary>
        /// The type/category of the medical device (e.g., Heart Rate Monitor).
        /// </summary>
        public string DeviceType { get; set; } = string.Empty;

        /// <summary>
        /// Foreign key referring to the associated medical device.
        /// </summary>
        public int DeviceId { get; set; }
        
        /// <summary>
        /// Collection of vitals recorded by this device.
        /// </summary>
        public ICollection<Vital> Vitals { get; set; } = new List<Vital>();
    }
}