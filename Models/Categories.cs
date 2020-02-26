using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lexiconn
{
    public partial class Category
    {
        public Category()
        {
            CategorizedWords = new HashSet<CategorizedWord>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Поле необхідно заповнити")]
        [Display(Name = "Назва")]
        public string Name { get; set; }

        public virtual ICollection<CategorizedWord> CategorizedWords { get; set; }
    }
}
