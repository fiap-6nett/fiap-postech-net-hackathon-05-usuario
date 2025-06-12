using System.ComponentModel.DataAnnotations;

namespace FastTech.Usuarios.Domain.Enums;

public enum UserRole
{
    [Display(Name = "Admin")] Admin = 0,

    [Display(Name = "Manager")] Manager = 1,

    [Display(Name = "Employee")] Employee = 2,

    [Display(Name = "Customer")] Customer = 3
}