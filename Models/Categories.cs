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
        [StringLength(50, ErrorMessage = "Довжина має не перевищувати 50 символів")]
        public string Name { get; set; }

        public string UserName { get; set; }

        public virtual ICollection<CategorizedWord> CategorizedWords { get; set; }
    }
}
