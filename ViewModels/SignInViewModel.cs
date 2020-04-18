using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconn.ViewModels
{
    public class SignInViewModel
    {
        private const string ERR_REQ = "Поле необхідно заповнити";
        private const string RGX_EMAIL = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

        [Required(ErrorMessage = ERR_REQ)]
        [RegularExpression(RGX_EMAIL, ErrorMessage = "Введіть коректну адресу")]
        [Display(Name = "Email")]
        public String Email { get; set; }

        [Required(ErrorMessage = ERR_REQ)]
        [StringLength(20, ErrorMessage = "Введіть пароль довжиною {2}-{1} символів", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запам\'ятати мене")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
