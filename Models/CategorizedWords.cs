using System;
using System.Collections.Generic;

namespace Lexiconn
{
    public partial class CategorizedWord
    {
        public CategorizedWord()
        {
            Translations = new HashSet<Translation>();
        }

        public int Id { get; set; }
        public int WordId { get; set; }
        public int CategoryId { get; set; }

        public string UserName { get; set; }

        public virtual Category Category { get; set; }
        public virtual Word Word { get; set; }
        public virtual ICollection<Translation> Translations { get; set; }
    }
}
