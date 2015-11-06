using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class CambiarContrasenaModel
    {
        [Required(ErrorMessage = "Campo requerido.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string Contrasena { get; set; }

        [Required(ErrorMessage = "Campo requerido.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string NuevaContrasena { get; set; }

        [Required(ErrorMessage = "Campo requerido.")]
        [DataType(DataType.Password)]
        [Display(Name = "Repetir Nueva Contraseña")]
        public string RNuevaContrasena { get; set; }
    }

    public class CambiarCorreoModel
    {
        [Required(ErrorMessage="Campo requerido.")]
        [EmailAddress(ErrorMessage="Formato incorrecto. (usuario@correo.com)")]
        [Display(Name = "Correo Electronico:")]
        public string Email { get; set; }
    }
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Correo")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "No cerrar sesion")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage="Campo Requerido.")]
        [EmailAddress(ErrorMessage="Formato incorrecto. (usuario@correo.com)")]
        [Display(Name = "Correo Electronico")]
        public string Email { get; set; }

        [Required(ErrorMessage="Campo Requerido.")]
        [StringLength(100, ErrorMessage = "La {0} debe ser de al menos {2} caracteres de largo.", MinimumLength = 6)]
        [DataType(DataType.Password,ErrorMessage = "Contraseña debe tener al menos un símbolo o un dígito. También al menos una minúscula y una mayúscula")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Range(1, 3)]
        [Display(Name = "Tipo de Doc.")]
        public int tipoDoc { get; set; }

        [Required(ErrorMessage = "Campo Requerido.")]
        [Display(Name = "#Doc")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Numero de documento debe ser numerico.")]
        public string codDoc { get; set; }

        [Required(ErrorMessage = "Campo Requerido.")]
        [Display(Name = "Nombres")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Nombre no debe ser alfanumerico.")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "Campo Requerido.")]
        [Display(Name = "Apellidos")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Apellidos no deben ser alfanumericos.")]
        public string apellido { get; set; }

        [Required]
        [Display(Name = "Direccion")]
        public string direccion { get; set; }

        [Display(Name = "Telefono")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Telefono debe ser numerico.")]
        public string telefono { get; set; }

        [Display(Name = "Telefono Movil")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Telefono Movil debe ser numerico.")]
        public string telMovil { get; set; }

        [Required]
        [Display(Name = "Sexo")]
        public string sexo { get; set; }

        [Required(ErrorMessage = "Campo Requerido.")]   
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Nac. ")]
        public System.DateTime fechaNac { get; set; }

    }

    public class EditViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo Electronico")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Nombres")]
        public string nombre { get; set; }

        [Required]
        [Display(Name = "Apellidos")]
        public string apellido { get; set; }

        [Display(Name = "Direccion")]
        public string direccion { get; set; }

        [Display(Name = "Telefono")]
        public string telefono { get; set; }

        [Display(Name = "Telefono Movil")]
        public string telMovil { get; set; }
    }

    public class EditClientModel
    {
        [Required(ErrorMessage = "Campo Requerido.")]
        [Display(Name = "Nombres")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Nombre no debe ser alfanumerico.")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "Campo Requerido.")]
        [Display(Name = "Apellidos")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Apellidos no deben ser alfanumericos.")]
        public string apellido { get; set; }

        [Required(ErrorMessage = "Campo Requerido.")]
        [Range(1, 3)]
        [Display(Name = "Tipo de Doc.")]
        public int tipoDoc { get; set; }

        [Required(ErrorMessage = "Campo Requerido.")]
        [Display(Name = "#Doc")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Numero de documento debe ser numerico.")]
        public string codDoc { get; set; }

        [Required(ErrorMessage = "Campo Requerido.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Nac. ")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fechaNac { get; set; }

        [Required(ErrorMessage = "Campo Requerido.")]
        [Display(Name = "Direccion")]
        public string direccion { get; set; }

        [Required(ErrorMessage = "Campo Requerido.")]
        [Display(Name = "Telefono")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Telefono debe ser numerico.")]
        public string telefono { get; set; }

        [Required(ErrorMessage = "Campo Requerido.")]
        [Display(Name = "Telefono Movil")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Telefono Movil debe ser numerico.")]
        public string telMovil { get; set; }
    }
    public class EmpleadoSearchModel
    {

        [Required]
        [Display(Name = "Nombre: ")]
        public string nombre { get; set; }
    }

    public class ClienteSearchModel
    {
        [Required]
        [Range(1, 3)]
        [Display(Name = "Tipo de Doc.")]
        public int tipoDoc { get; set; }

        [Required]
        [Display(Name = "# Doc")]
        public string codDoc { get; set; }
    }

    public class ReporteClienteModel
    {

        [Required]
        [Display(Name = "Puntos: ")]
        public int puntos { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} debe ser de al menos {2} caracteres de largo.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RegistrarUsuarioVendedorModel
    {
        [Required(ErrorMessage = "Campo requerido.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nombres:")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Campo requerido.")]
        [DataType(DataType.Password)]
        [Display(Name = "Apellidos:")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "Campo requerido.")]
        [DataType(DataType.Password)]
        [Display(Name = "Correo:")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Campo requerido.")]
        [DataType(DataType.Password)]
        [Display(Name = "DNI:")]
        public string Dni { get; set; }

        [Required(ErrorMessage = "Campo requerido.")]
        [Range(1, 3)]
        [Display(Name = "Tipo Documento:")]
        public int TipoDoc { get; set; }
    }
}
