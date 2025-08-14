
using PuduIOT.DA;
using PuduIOT.Models;

namespace PuduIOT.DA.Respository
{

    public class MstLanguageRespository
    {
        private readonly HamadenDbContext _context;

        public MstLanguageRespository(HamadenDbContext context)
        {
            _context = context;
        }

        public IEnumerable<MstLanguage> GetAllLanguages()
        {

            return _context.MstLanguages.ToList();
        }

        public MstLanguage GetLanguageByCulture(string culture)
        {
            return _context.MstLanguages.FirstOrDefault(x =>
                x.lang_code.Trim().ToLower() == culture.Trim().ToLower());
        }
        public string GetLangCodeByCulture(string culture)
        {
            MstLanguage language = _context.MstLanguages.FirstOrDefault(x =>
                x.lang_code.Trim().ToLower() == culture.Trim().ToLower());
            return language.lang_code;
        }

        public MstLanguage GetStringResource(string resourceKey, string languageCode)
        {
            return _context.MstLanguages.FirstOrDefault(x =>
                    x.item_code.Trim().ToLower() == resourceKey.Trim().ToLower()
                    && x.lang_code == languageCode);
        }

        public MstLanguage GetLanguageByCode(string code)
        {
            return _context.MstLanguages.FirstOrDefault(c => c.lang_code == code);
        }

        public List<MstLanguage> GetLanguageError()
        {
            return _context.MstLanguages.Where(language => language.lang_code == Thread.CurrentThread.CurrentUICulture.Name).ToList();
        }

        public void AddLanguages(MstLanguage language)
        {
            _context.MstLanguages.Add(language);
            _context.SaveChanges();
        }

       
    }
}
