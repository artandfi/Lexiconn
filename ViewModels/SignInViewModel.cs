using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconn.ViewModels
{
    public class SignInViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public String Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запам\'ятати мене")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
