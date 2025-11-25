import React from 'react';
import { useState, useEffect, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import 'swiper/css';
import CategoryTree from '@/components/layout/navbar/CategoryItem';
import ProductSection from '@/features/home/components/ProductSection';
import { ProductByCategoryPriorityResponse } from '@/types/Product';
import { getProductGroupByCategory } from '@/services/product.service';
import { ApiResponse } from '@/types/ApiResponse';
import { useCategory } from "@/hooks/useCategory";

import bannerImg from '@/assets/images/banner.png';

const Home: React.FC = () => {
  const { t, i18n } = useTranslation();
  const [loading, setLoading] = useState(true);
  const { categories } = useCategory();
  const [localCategories, setLocalCategories] = useState(categories);
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


  useEffect(() => {
    setLocalCategories(categories);
  }, [categories]);

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
                {localCategories.map((cat) => (
                  <CategoryTree key={cat.id} category={cat} asideHeight={asideHeight} top={asideTop}/>
                ))}
              </div>
            </aside>

            <section className="flex-1 h-full">
              <img
                src={bannerImg}
                className="w-full h-full shadow object-scale-down"
              />
            </section>
          </div>

          {/* Khối products */}
          <div className="w-[100%] mt-6 mx-auto">
            {productGroups?.data.map((group) => (
              <ProductSection
                key={group?.categoryId}
                title={group?.categoryName}
                products={group.products}
                categoryId={group?.categoryId}
                setProductGroups={setProductGroups}
              />
            ))}
          </div>
        </div>
    
        <div className="block md:hidden">
           <section className="flex-1 h-full">
              <img
                src={bannerImg}
                className="w-full h-full shadow object-scale-down"
              />
            </section>

            {/* Khối products */}
          <div className="w-full mt-6">
            {productGroups?.data.map((group) => (
              <ProductSection
                key={group?.categoryId}
                title={group?.categoryName}
                products={group?.products}
                categoryId={group?.categoryId}
                setProductGroups={setProductGroups}
              />
            ))}
          </div>
        </div>
      </main>

    </>
  );
};

export default Home;