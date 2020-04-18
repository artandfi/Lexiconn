using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Lexiconn.ViewModels
{
    public class SignUpViewModel
    {
        private const string ERR_REQ = "Поле необхідно заповнити";
        private const string RGX_EMAIL = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

        [Required(ErrorMessage = ERR_REQ)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(RGX_EMAIL, ErrorMessage = "Введіть коректну адресу")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = ERR_REQ)]
        [Display(Name = "Ім\'я користувача")]
        public string UserName { get; set; }

        [Required(ErrorMessage = ERR_REQ)]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Пароль має складатися з {2}-{1} символів", MinimumLength = 6)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = ERR_REQ)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        [StringLength(20)]
        [Display(Name = "Підтвердження паролю")]
        public string PasswordConfirm { get; set; }
    }
}