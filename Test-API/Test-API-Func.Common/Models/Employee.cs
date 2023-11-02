using System.ComponentModel.DataAnnotations;

namespace Test_API_Func.Common.Models
{
    public class Employee
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Country { get; set; }

        public string Occupation { get; set; }
    }
}
