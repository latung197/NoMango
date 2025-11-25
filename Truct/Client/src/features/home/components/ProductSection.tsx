import React from 'react';
import { useState } from 'react';
import { ProductByCategoryPriorityResponse, ProductResponse } from '@/types/Product';
import { useNavigate } from "react-router-dom";
import { useTranslation } from 'react-i18next';

import wishlist from '@/assets/icons/bookmark.png';
import wishlistWhile from '@/assets/icons/bookmark-white.png';
import warehouse from '@/assets/icons/warehouse.png';
import ProductCard from '../../../components/shared/ProductCard';
import { ApiResponse } from '@/types/ApiResponse';

interface Props {
  categoryId?: number;
  title: string;
  products: ProductResponse[];
  setProductGroups: React.Dispatch<React.SetStateAction<ApiResponse<ProductByCategoryPriorityResponse[]> | undefined>>;
}

const ProductSection: React.FC<Props> = ({ categoryId, title, products, setProductGroups }) => {
  const navigate = useNavigate();
  const {t, i18n} = useTranslation();
  const [visibleCount, setVisibleCount] = useState(6);

  const handleNavigateToSearch = async () => {
    try {
      navigate(`/search-by-category?categoryId=${categoryId}`, {
        replace: true,
      });
    } catch (error) {
      console.error(error);
    }
  };

  const visibleProducts = products.slice(0, visibleCount);

  return (
    <section className="py-8 px-4">
      <h2 className="w-full text-xl font-medium mb-6 px-4 py-1 rounded bg-[rgb(192,255,254)]">
        {title}
      </h2>
      <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4">
        {visibleProducts?.map((item, index) => (
          <ProductCard
            key={index}
            item={item}
            wishlistIcon={wishlist}
            wishlistWhileIcon={wishlistWhile}
            warehouseIcon={warehouse}
            onToggleWishlist={(id) => {
            setProductGroups((prev) => {
                if (!prev) return prev;

                const updatedGroups = prev.data.map((group) => ({
                  ...group,
                  products: group.products.map((p) =>
                    p.id === id ? { ...p, isWishlist: !p.isWishlist } : p
                  ),
                }));

                return {
                  ...prev,
                  data: updatedGroups,
                };
              });
            }}
          />
        ))}
      </div>

      {visibleCount <= products.length && (
        <div className="mt-4 text-center">
          <button
            onClick={handleNavigateToSearch}
            className="inline-flex items-center px-4 py-2 text-sm font-medium text-[#00B0F0] border border-[#00B0F0] rounded hover:bg-[#00B0F0] hover:text-white transition"
          >
            {t('home.learn_more')}
          </button>
        </div>
      )}
    </section>

  );
};
export default ProductSection;