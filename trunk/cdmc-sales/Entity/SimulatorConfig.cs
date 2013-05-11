using System.ComponentModel.DataAnnotations;

namespace Entity
{
    /// <summary>
    /// Entity of simulator config.
    /// </summary>
    public class SimulatorConfig : EntityBase
    {
        public string AdminName { get; set; }

        [Display(Name = "模拟人"), Required]
        public string SimulatorName { get; set; }
    }
}
