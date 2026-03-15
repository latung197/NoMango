import i18n from "i18next";
import { initReactI18next } from "react-i18next";

import en from "../locales/en.json";
import vi from "../locales/vi.json";

const resources = {
    vi: { translation: en },
    en: { translation: vi },
  };

export const getDefaultLanguage = () => {
  const languageInStorage = localStorage.getItem("lang");
  if (languageInStorage) return languageInStorage;

  const language = navigator.language;
  if (language.startsWith('vi')) return 'vi';
  if (language.startsWith('ja')) return 'ja';
  return 'vi';
};

i18n.use(initReactI18next).init({
  resources,
  lng: getDefaultLanguage(),
  fallbackLng: 'vi',
  interpolation: { escapeValue: false }
});

export default i18n;