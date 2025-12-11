using System.ComponentModel.DataAnnotations;

public class Login
{
    [Key]
    public int IdLogin { get; set; }

    [Required]
    public string Correo { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string Rol { get; set; } 

    public Guid? IdNetUser { get; set; }
}