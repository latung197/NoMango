import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import translationVI from '../locales/vi.json';
import translationEN from '../locales/en.json';
import translationJA from '../locales/ja.json';

const resources = {
    vi: { translation: translationVI },
    en: { translation: translationEN },
    ja: { translation: translationJA },
  };

export const getDefaultLanguage = () => {
  // const languageInStorage = localStorage.getItem("lang");
  // if (languageInStorage) return languageInStorage;

  // const language = navigator.language;
  // if (language.startsWith('vi')) return 'vi';
  // if (language.startsWith('ja')) return 'ja';
  return 'ja';
};

i18n.use(initReactI18next).init({
  resources,
  lng: getDefaultLanguage(),
  fallbackLng: 'ja',
  interpolation: { escapeValue: false }
});

export default i18n;