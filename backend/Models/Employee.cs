using System.ComponentModel.DataAnnotations;

public enum Gender
{
    Homme,Femme
}

public class Employee
{
    [Key]
    public int EmployeeID { get; set; }

    [Required]
    [StringLength(50)]
    public string? FullName { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}
