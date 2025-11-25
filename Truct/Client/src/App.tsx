import "./App.css";
import AppRouterProvider from "./routes/layout";
import { CategoryProvider } from './hooks/useCategory';
import { BrowserRouter } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import { DynamicTitle } from "./components/common/DynamicTitle";
import { CartProvider } from "./context/CartContext";

function App() {
  return (
    <CategoryProvider>
      <CartProvider>
        <BrowserRouter>
          <DynamicTitle />
          <AppRouterProvider />
          <ToastContainer />
        </BrowserRouter>
      </CartProvider>
    </CategoryProvider>
  );
}

export default App;
