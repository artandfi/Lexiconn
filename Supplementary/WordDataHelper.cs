using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Lexiconn.Models;
using Microsoft.AspNetCore.Http;

namespace Lexiconn.Supplementary
{
    public class WordDataHelper
    {
        private readonly DBDictionaryContext _context;
        private readonly ClaimsPrincipal _user;

        /// <summary>
        /// Creates a Word Helper object to attach to the classes
        /// who need its supplementary methods.
        /// </summary>
        /// <param name="context">An object to work with the DB from the host class.</param>
        public WordDataHelper(DBDictionaryContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _user = accessor.HttpContext.User;
        }

        /// <summary>
        /// Creates a new word in the corresponding table if it isn't present there yet,
        /// gives away the existing one's ID otherwise, for the specified ford record.
        /// </summary>
        /// <param name="model">The word record to add to the database.</param>
        /// <param name="wordId">The word's future or actual ID.</param>
        public void CreateWord(WordData model, out int wordId)
        {
            var word = new Word();
            var sameWord = _context.Words.FirstOrDefault(w => w.ThisWord.Equals(model.Word) && w.LanguageId == model.LanguageId);
            word.LanguageId = model.LanguageId;
            word.ThisWord = model.Word;

            if (sameWord == null)
            {
                _context.Words.Add(word);
                _context.SaveChanges();
                wordId = word.Id;
            }
            else
            {
                wordId = sameWord.Id;
            }
        }

        /// <summary>
        /// Adds a new categorized word in the corresponding table,
        /// giving away its ID, for the specified word record.
        /// </summary>
        /// <param name="model">The word record to add to the database.</param>
        /// <param name="wordId">The added word's ID.</param>
        /// <param name="catWordId">The categorized word's ID to give away.</param>
        public void CreateCatWord(WordData model, int wordId, out int catWordId)
        {
            var catWord = new CategorizedWord();
            catWord.WordId = wordId;
            catWord.CategoryId = model.CategoryId;
            catWord.UserName = _user.Identity.Name;

            _context.CategorizedWords.Add(catWord);
            _context.SaveChanges();
            catWordId = catWord.Id;
        }

        /// <summary>
        /// Adds new translations to the corresponding table,
        /// giving away a possibly occured input format error flag,
        /// for the specified word record.
        /// </summary>
        /// <param name="model">The word record to add to the database.</param>
        /// <param name="catWordId">The added categorized word's ID.</param>
        /// <param name="error">The error flag to give away.</param>
        public void CreateTranslations(WordData model, int catWordId, out bool error)
        {
            error = false;
            List<Translation> translations = new List<Translation>();

            if (!SplitTranslations(model.Translation, ref translations))
            {
                error = true;
                return;
            }

            foreach (var translation in translations)
            {
                translation.CategorizedWordId = catWordId;
            }

            _context.Translations.AddRange(translations);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the word and its language for the specified word record.
        /// </summary>
        /// <param name="model">The word record to update.</param>
        public void UpdateWord(WordData model)
        {
            var wordEntity = _context.Words.First(w => w.Id == model.WordId);
            wordEntity.ThisWord = model.Word;

            _context.Update(wordEntity);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the categorized word's category for the specified word record.
        /// </summary>
        /// <param name="model">The word record to update.</param>
        public void UpdateCatWord(WordData model)
        {
            var catWordEntity = _context.CategorizedWords.First(cw => cw.Id == model.CatWordId);
            catWordEntity.CategoryId = model.CategoryId;

            _context.Update(catWordEntity);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates translations for the specified word record.
        /// </summary>
        /// <param name="model">The word record to update.</param>
        public bool UpdateTranslations(WordData model)
        {
            var oldTranslationIds = model.TranslationIds.Split(',').Select(Int32.Parse).ToList();
            var oldTranslations = new List<Translation>();
            var newTranslations = new List<Translation>();

            if (!SplitTranslations(model.Translation, ref newTranslations))
            {
                return false;
            }

            for (int i = 0; i < oldTranslationIds.Count; i++)
            {
                oldTranslations.Add(_context.Translations.Find(oldTranslationIds[i]));
                if (i < newTranslations.Count)
                {
                    oldTranslations[i].ThisTranslation = newTranslations[i].ThisTranslation;
                }
            }

            for (int i = 0; i < newTranslations.Count; i++)
            {
                newTranslations[i].CategorizedWordId = model.CatWordId;
            }

            ResolveTranslationUpdate(oldTranslations, newTranslations);
            return true;
        }

        /// <summary>
        /// Turns string-listed translations into a list of translations object.
        /// </summary>
        /// <param name="raw">The string-listed translations.</param>
        /// <param name="translations">List of translations object to fill in and give away.</param>
        public bool SplitTranslations(string raw, ref List<Translation> translations)
        {
            string curTranslation = "";
            char cur = raw[0];

            if (!IsFirstCharacterCorrect(cur, ref curTranslation))
            {
                return false;
            }

            for (int i = 1; i < raw.Length; i++)
            {
                cur = raw[i];
                char prev = raw[i - 1];

                if (!ResolveCharacters(prev, cur, ref curTranslation, ref translations))
                {
                    return false;
                }
            }

            if (curTranslation.Last() == ' ')
                curTranslation = curTranslation[0..^1];

            translations.Add(new Translation() { ThisTranslation = curTranslation });
            return true;
        }

        /// <summary>
        /// Determines whether a first character input is correct,
        /// i.e. whether it's not a comma or a whitespace. 
        /// </summary>
        /// <param name="cur">Current character read.</param>
        /// <param name="curTranslation">Current translation read.</param>
        private bool IsFirstCharacterCorrect(char cur, ref string curTranslation)
        {
            curTranslation += cur;
            return cur != ',' && cur != ' ';
        }

        /// <summary>
        /// Defines the correctness of all further characters and forges them into translations
        /// as they appear and if they are correct.
        /// </summary>
        /// <param name="prev">Previous character read.</param>
        /// <param name="cur">Current character read.</param>
        /// <param name="curTranslation">Current translation read.</param>
        /// <param name="translations">Translations' list to give away.</param>
        private bool ResolveCharacters(char prev, char cur, ref string curTranslation, ref List<Translation> translations)
        {
            if (cur == ',')
            {
                if (prev == ',')
                {
                    return false;
                }

                translations.Add(new Translation() { ThisTranslation = curTranslation });
                curTranslation = "";
            }
            else
            {
                if (cur != ' ' || cur == ' ' && prev != ' ' && prev != ',')
                {
                    curTranslation += cur;
                }
            }
            return true;
        }

        /// <summary>
        /// Resolves translations' update type:
        /// - if their amount increased, the existing are updated and the oncome added;
        /// - if their amount didn't change, they are updated;
        /// - if their amount decreased, the remaining are updated and the extra are removed.
        /// </summary>
        /// <param name="oldTs"></param>
        /// <param name="newTs"></param>
        private void ResolveTranslationUpdate(List<Translation> oldTs, List<Translation> newTs)
        {
            if (oldTs.Count < newTs.Count)
            {
                for (int i = 0; i < oldTs.Count; i++)
                {
                    _context.Update(oldTs[i]);
                }
                for (int i = oldTs.Count; i < newTs.Count; i++)
                {
                    _context.Translations.Add(newTs[i]);
                }
            }
            else
            {
                for (int i = newTs.Count; i < oldTs.Count; i++)
                {
                    _context.Translations.Remove(oldTs[i]);
                }

                for (int i = 0; i < newTs.Count; i++)
                {
                    _context.Update(oldTs[i]);
                }
            }
            _context.SaveChanges();
        }

        /// <summary>
        /// Determines whether an extra comma error was encountered during reading translations.
        /// If so, the creation of new word record is rolled back.
        /// </summary>
        /// <param name="commaError">A flag of extra comma error.</param>
        /// <param name="wordId">The ID of recently added word to delete.</param>
        public bool ValidateComma(bool commaError, int wordId)
        {
            if (commaError)
            {
                var word = _context.Words.Find(wordId);
                _context.Words.Remove(word);
                _context.SaveChanges();

                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Transforms the given list of translation IDs into string (with commas).
        /// </summary>
        /// <param name="translations">List of the translations whose IDs need to be set.</param>
        public string TranslationIdsToString(List<Translation> translations)
        {
            var translationIds = new List<int>();

            foreach (var translation in translations)
            {
                translationIds.Add(translation.Id);
            }
            return string.Join(",", translationIds);
        }

        /// <summary>
        /// Transforms the given translations list into string (with commas).
        /// </summary>
        /// <param name="translations">Translations list to be transformed.</param>
        public string TranslationsToString(List<Translation> translations)
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
            return commaTranslations;
        }
    }
}
