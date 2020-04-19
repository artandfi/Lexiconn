using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconn.ViewModels
{
    public class ChangePasswordViewModel
    {
        private const string ERR_REQ = "Поле необхідно заповнити";

        [Required(ErrorMessage = ERR_REQ)]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Пароль має складатися з {2}-{1} символів", MinimumLength = 6)]
        [Display(Name = "Старий пароль")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = ERR_REQ)]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Пароль має складатися з {2}-{1} символів", MinimumLength = 6)]
        [Display(Name = "Новий пароль")]
        public string NewPassword { get; set; }
    }
}
