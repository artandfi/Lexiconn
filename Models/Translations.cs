using System;
using System.Collections.Generic;

namespace Lexiconn
{
    public partial class Translation
    {
        public int Id { get; set; }
        public int CategorizedWordId { get; set; }
        public string ThisTranslation { get; set; }

        public virtual CategorizedWord CategorizedWord { get; set; }
    }
}
