
import { createContext, useContext, useState, useEffect } from "react";
import { getCategories } from "@/services/category.service"; 
import { useLanguage } from "./useLanguage";

type CategoryContextType = {
  show: boolean;
  toggle: () => void;
  categories: CategoryResponse[];
  reloadCategories: () => Promise<void>;
};

const CategoryContext = createContext<CategoryContextType | undefined>(undefined);

export const useCategory = () => {
  const context = useContext(CategoryContext);
  if (!context) {
    throw new Error("useCategory must be used within a CategoryProvider");
  }
  return context;
};

export const CategoryProvider = ({ children }: { children: React.ReactNode }) => {
  const [show, setShow] = useState(false);
  const [categories, setCategories] = useState<CategoryResponse[]>([]);
  const { language } = useLanguage();

  const toggle = () => setShow((prev) => !prev);

  const reloadCategories = async () => {
    try {
      setCategories(await getCategories());
    } catch (error) {
      console.error("Failed to load categories", error);
    }
  };

  useEffect(() => {
    if (categories.length === 0) {
      reloadCategories();
    }
  }, [language]);


  return (
    <CategoryContext.Provider value={{ show, toggle, categories, reloadCategories }}>
        {children}
    </CategoryContext.Provider>
  );
};
