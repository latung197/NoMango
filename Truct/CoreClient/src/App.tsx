import { useState } from 'react'
import './App.css'
import { BrowserRouter, Routes } from "react-router-dom";
import AppRouterProvider from './routes/layout';
function App() {
  return (
 <BrowserRouter>
  <AppRouterProvider>
  </AppRouterProvider>
</BrowserRouter>
  )
}

export default App
