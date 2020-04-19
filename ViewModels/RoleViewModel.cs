using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconn.ViewModels
{
    public class RoleViewModel
    {
        [Required(ErrorMessage = "Поле необхідно заповнити")]
        [StringLength(25)]
        [Display(Name = "Назва")]
        public string Name { get; set; }
    }
}
