import React from 'react';
import { useState, useEffect, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import 'swiper/css';
import { ProductByCategoryPriorityResponse } from '@/types/Product';
import { getProductGroupByCategory } from '@/services/product.service';
import { ApiResponse } from '@/types/ApiResponse';
import {House} from "lucide-react"


const Home: React.FC = () => {
  const { t, i18n } = useTranslation();
  const [loading, setLoading] = useState(true);
  const asideRef = useRef<HTMLDivElement>(null);
  const [asideHeight, setAsideHeight] = useState<number>(0);
  const [asideTop, setAsideTop] = useState<number>(0);

  useEffect(() => {
    if (asideRef.current) {
      const rect = asideRef.current.getBoundingClientRect();
      setAsideHeight(rect.height);
      setAsideTop(rect.top);
    }
  }, []);



  const [productGroups, setProductGroups] = useState<ApiResponse<ProductByCategoryPriorityResponse[]>>();
  useEffect(() => {
    const fetchCategories = async () => {
      try {
        setLoading(true);
        const data = await getProductGroupByCategory();
        setProductGroups(data);
      } finally {
        setLoading(false);
      }
    };
    fetchCategories();
  }, [i18n.language]);

  return (
    <>
      <main className="flex flex-col items-center">
    <nav className="show-md h-screen w-60 p-2">
        
          <div className='border px-3 py-2 h-20'>
            <House size={24} className="" />
          </div>
      </nav>

        <div className="hidden md:grid items-center">
          {/* Khối banner + category */}
          <div className="flex w-[100%] px-4 py-2 gap-4 h-[500px] mx-auto">
            <aside 
              ref={asideRef}
              className="w-1/5 h-full bg-white rounded shadow overflow-y-auto"
              >
              <div className="bg-[#02CECF] text-white px-4 py-2 text-sm font-semibold">
                {t('home.category')}
              </div>
              <div className="p-4">
               
              </div>
            </aside>
            <section className="flex-1 h-full">
            </section>
          </div>

          {/* Khối products */}
          <div className="w-[100%] mt-6 mx-auto">
          </div>
        </div>
    
        <div className="block md:hidden">
           <section className="flex-1 h-full">
            </section>

            {/* Khối products */}
          <div className="w-full mt-6">
          </div>
        </div>
      </main>

    </>
  );
};

export default Home;