using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class EmployeeDTO
    {
        public int EmployeeId { get; set; }
        
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
       
        public string? Email { get; set; }
    }
}

  