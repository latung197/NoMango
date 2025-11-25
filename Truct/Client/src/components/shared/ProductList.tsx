// ProductList.tsx
import React from "react";
import ProductCard from "./ProductCard";
import { ProductResponse } from "@/types/Product";

import wishlist from '@/assets/icons/bookmark.png';
import wishlistWhile from '@/assets/icons/bookmark-white.png';
import warehouse from '@/assets/icons/warehouse.png';

type ProductsListProps = {
  products: { data: ProductResponse[]; meta: any } | null;
  setProducts: React.Dispatch<React.SetStateAction<{ data: ProductResponse[]; meta: any } | null>>;
};


const ProductList: React.FC<ProductsListProps> = ({ products, setProducts }) => {
  if (!products || products.data.length === 0) {
    return <p className="text-center text-gray-500">商品がありません</p>;
  }

  return (
    <div className="grid grid-cols-5 gap-4">
      {products.data.map((product, index) => (
        <ProductCard
          key={index}
          item={product}
          wishlistIcon={wishlist}
          wishlistWhileIcon={wishlistWhile}
          warehouseIcon={warehouse}
          onToggleWishlist={(id) => {
            setProducts((prev) => {
              if (!prev) return prev;
              return {
                ...prev,
                data: prev.data.map((p) =>
                  p.id === id ? { ...p, isWishlist: !p.isWishlist } : p
                ),
              };
            });
          }}
        />
      ))}
    </div>
  );
};

export default ProductList;
