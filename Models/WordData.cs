using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Lexiconn.Models
{
    public class WordData
    {
        private const string _fillMessage = "Поле необхідно заповнити";

        [Display(Name = "Слово")]
        [Required(ErrorMessage = _fillMessage)]
        public string Word { get; set; }

        [Display(Name = "Мова")]
        [Required(ErrorMessage = _fillMessage)]
        public int LanguageId { get; set; }

        [Display(Name = "Мова")]
        public string Language { get; set; }

        [Display(Name = "Категорія")]
        [Required(ErrorMessage = _fillMessage)]
        public int CategoryId { get; set; }

        [Display(Name = "Категорія")]
        public string Category { get; set; }

        [Display(Name = "Переклад")]
        [Required(ErrorMessage = _fillMessage)]
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
