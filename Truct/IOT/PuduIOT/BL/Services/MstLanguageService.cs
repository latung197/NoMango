using PuduIOT.BL.Interfaces;
using PuduIOT.DA;
using PuduIOT.DA.Respository;
using PuduIOT.Models;

namespace PuduIOT.BL.Services
{
    public class MstLanguageService : ILanguageService
    {
        private readonly MstLanguageRespository _repository;
        
        public MstLanguageService(HamadenDbContext context)
        {
            _repository = new MstLanguageRespository(context);
        }
        public IEnumerable<MstLanguage> GetAllLanguages()
        {
            return _repository.GetAllLanguages();
        }

        public MstLanguage GetLanguageByCulture(string culture)
        {
            return _repository.GetLanguageByCulture(culture);
        }

        public MstLanguage GetStringResource(string resourceKey, string languageCode)
        {
            return _repository.GetStringResource(resourceKey, languageCode);
        }


        public string GetLangCodeByCulture(string culture)
        {
            return _repository.GetLangCodeByCulture(culture);
        }

        public MstLanguage GetLineByCode(string code)
        {
            return _repository.GetLanguageByCode(code);
        }

        public List<MstLanguage> GetLanguageError()
        {
            return _repository.GetLanguageError();
        }
        public void AddLine(MstLanguage language)
        {
            _repository.AddLanguages(language);
        }

      
    }
}
