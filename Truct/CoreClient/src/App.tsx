import { useState } from 'react'
import './App.css'
import { BrowserRouter, Routes } from "react-router-dom";
import AppRouterProvider from './routes/layout';
function App() {
  const [count, setCount] = useState(0)

  return (
 <BrowserRouter>
  <AppRouterProvider>
  </AppRouterProvider>
</BrowserRouter>
  )
}

export default App
