export type LanguageOption = {
  code: string;
  label: string;
};

export const supportedLanguages: LanguageOption[] = [
  { code: 'vi', label: 'Tiếng Việt'},
  { code: 'en', label: 'English'},
  { code: 'ja', label: '日本語'},
];

export const supportedLanguageCodes = supportedLanguages.map(l => l.code);
