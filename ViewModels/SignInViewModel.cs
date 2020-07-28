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

        [Required(ErrorMessage = ERR_REQ)]
        [Display(Name = "Ім\'я користувача")]
        public String UserName { get; set; }

        [Required(ErrorMessage = ERR_REQ)]
        [StringLength(20, ErrorMessage = "Введіть пароль довжиною {2}-{1} символів", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запам\'ятати мене")]
        public bool RememberMe { get; set; }
    }
}
