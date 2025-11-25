import React, { createContext, useContext, useReducer, ReactNode } from 'react';
import { languageReducer, initialState } from '../reducer/languageReducer';

interface LanguageContextType {
  language: string;
  changeLanguage: (lang: string) => void;
}

const LanguageContext = createContext<LanguageContextType | undefined>(undefined);

interface LanguageProviderProps {
  children: ReactNode;
}

export const LanguageProvider: React.FC<LanguageProviderProps> = ({ children }) => {
  const [state, dispatch] = useReducer(languageReducer, initialState);

  const changeLanguage = (lang: string) => {
    dispatch({ type: 'SET_LANGUAGE', payload: lang });
  };

  return (
    <LanguageContext.Provider value={{ language: state.language, changeLanguage }}>
      {children}
    </LanguageContext.Provider>
  );
};

// Custom hook to use language context
export const useLanguage = () => {
  const context = useContext(LanguageContext);
  if (!context) {
    throw new Error('useLanguage must be used within a LanguageProvider');
  }
  return context;
};
