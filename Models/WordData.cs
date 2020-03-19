using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Lexiconn.Models
{
    public class WordData
    {
        private const string MSG_FILL = "Поле необхідно заповнити";

        public int WordId { get; set; }

        [Display(Name = "Слово")]
        [Required(ErrorMessage = MSG_FILL)]
        [StringLength(50)]
        public string Word { get; set; }

        [Display(Name = "Мова")]
        public int LanguageId { get; set; }

        [Display(Name = "Мова")]
        public string Language { get; set; }

        [Display(Name = "Категорія")]
        public int CategoryId { get; set; }

        [Display(Name = "Категорія")]
        public string Category { get; set; }

        public int CatWordId { get; set; }

        [Display(Name = "Переклад")]
        [Required(ErrorMessage = MSG_FILL)]
        [StringLength(50)]
        public string Translation { get; set; }

        public string TranslationIds { get; set; }

        public void SetCommaTranslations(List<Translation> translations)
        {
            string commaTranslations = "";

            for (int i = 0; i < translations.Count - 1; i++)
            {
                commaTranslations += translations[i].ThisTranslation + ", ";
            }

            if (translations.Count != 0)
            {
                commaTranslations += translations[translations.Count - 1].ThisTranslation;
            }
            this.Translation = commaTranslations;
        }
    }
}
