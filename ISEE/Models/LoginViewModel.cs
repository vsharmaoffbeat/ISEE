using System.ComponentModel.DataAnnotations;
namespace ISEEREGION.Models
{

    public class LoginViewModel
    {
        //[Required]
        //[Display(Name = "User name")]
        [Required(ErrorMessage = "Username is Required", AllowEmptyStrings = false)]
        public string UserName { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "Password")]
        [Required(ErrorMessage="Password is Required",AllowEmptyStrings=false)]
        public string Password { get; set; }

    }
}