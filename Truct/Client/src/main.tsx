import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { LanguageProvider } from './hooks/useLanguage'; 
import './configs/i18n';
import './index.css'
import App from './App'

createRoot(document.getElementById('root')!).render(
  // <StrictMode>
    <LanguageProvider>
      <App />
    </LanguageProvider>
  // </StrictMode>
)
