using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lexiconn
{
    public partial class Language
    {
        public Language()
        {
            Words = new HashSet<Word>();
        }

        public int Id { get; set; }

        [Display(Name = "Мова")]
        [Required(ErrorMessage = "Поле необхідно заповнити")]
        [StringLength(50, ErrorMessage = "Довжина має не перевищувати 50 символів")]
        public string Name { get; set; }

        public virtual ICollection<Word> Words { get; set; }
    }
}
