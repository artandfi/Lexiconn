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

        public WordDataHelper(DBDictionaryContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _user = accessor.HttpContext.User;
        }

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

        public void CreateCatWord(WordData model, int wordId, out int catWordId)
        {
            var catWord = new CategorizedWord
            {
                WordId = wordId,
                CategoryId = model.CategoryId,
                UserName = _user.Identity.Name
            };

            _context.CategorizedWords.Add(catWord);
            _context.SaveChanges();
            catWordId = catWord.Id;
        }

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

        public void UpdateWord(WordData model)
        {
            var wordEntity = _context.Words.First(w => w.Id == model.WordId);
            wordEntity.ThisWord = model.Word;

            _context.Update(wordEntity);
            _context.SaveChanges();
        }

        public void UpdateCatWord(WordData model)
        {
            var catWordEntity = _context.CategorizedWords.First(cw => cw.Id == model.CatWordId);
            catWordEntity.CategoryId = model.CategoryId;

            _context.Update(catWordEntity);
            _context.SaveChanges();
        }

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

        private bool IsFirstCharacterCorrect(char cur, ref string curTranslation)
        {
            curTranslation += cur;
            return cur != ',' && cur != ' ';
        }

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

        public string TranslationIdsToString(List<Translation> translations)
        {
            var translationIds = new List<int>();

            foreach (var translation in translations)
            {
                translationIds.Add(translation.Id);
            }
            return string.Join(",", translationIds);
        }

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
