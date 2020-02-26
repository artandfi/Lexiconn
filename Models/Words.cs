using System;
using System.Collections.Generic;

namespace Lexiconn
{
    public partial class Word
    {
        public Word()
        {
            CategorizedWords = new HashSet<CategorizedWord>();
        }

        public int Id { get; set; }
        public string ThisWord { get; set; }
        public int LanguageId { get; set; }

        public virtual Language Language { get; set; }
        public virtual ICollection<CategorizedWord> CategorizedWords { get; set; }
    }
}
