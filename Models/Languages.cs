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

        [Required(ErrorMessage = "Поле необхідно заповнити")]
        [Display(Name = "Мова")]
        public string Name { get; set; }

        public virtual ICollection<Word> Words { get; set; }
    }
}
