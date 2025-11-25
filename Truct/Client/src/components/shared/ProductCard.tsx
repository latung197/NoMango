import { toggleWishlist } from "@/services/wishlist.service";
import { getToken, isTokenValid } from "@/utils/authUtils";
import React from "react";
import { useNavigate } from "react-router-dom";

interface ProductCardProps {
  item: {
    id: number | string;
    name: string;
    thumbnailUrl?: string;
    shortDescription?: string;
    leadTimeDays?: number;
    isWishlist?: boolean;
  };
  wishlistIcon: string;
  wishlistWhileIcon: string;
  warehouseIcon: string;
  onToggleWishlist: (productId: number | string) => void;
}

const ProductCard: React.FC<ProductCardProps> = ({
  item,
  wishlistIcon,
  wishlistWhileIcon,
  warehouseIcon,
  onToggleWishlist
}) => {
    const navigate = useNavigate();

    const handleToggle = async (e: React.MouseEvent) => {
        e.stopPropagation();

        const token = getToken();
        if (!token) {
            navigate("/login");
        return;
        }
        const res = await toggleWishlist({ productId: item.id });
        onToggleWishlist(item.id);
    };

    return (
        <div
            key={item.id}
            className="group relative border border-gray-300 rounded p-4 text-center shadow-sm hover:shadow-md hover:border-[#00B0F0] transition cursor-pointer"
            onClick={() => navigate(`/product/${item.id}`)}
        >
            {/* Wishlist icon */}
            <div className="absolute top-0 right-1">
                <img
                    src={item.isWishlist ? wishlistIcon : wishlistWhileIcon}
                    alt="Wishlist"
                    className="w-6 h-6 cursor-pointer"
                    onClick={(e) => {
                        e.stopPropagation();
                        const token = getToken();
                        if (!token) return;
                        handleToggle(e);
                    }}
                />
            </div>

            {/* Product image */}
            <img
                src={item.thumbnailUrl}
                alt={item.name}
                className="mb-2 w-full h-48 object-contain"
            />

            {/* Product name */}
            <h3 className="group-hover:underline mt-4 text-sm font-medium text-left">
                {item.name}
            </h3>

            {/* Lead time */}
            <div className="flex items-center gap-1 text-xs mt-2">
                <img src={warehouseIcon} alt="Lead time" className="w-4 h-4" />
                <span>{item.leadTimeDays} 日</span>
            </div>

            {/* Short description */}
            <div>
                <span
                className="mt-4 block text-[11px] font-light text-left overflow-hidden"
                style={{
                    display: "-webkit-box",
                    WebkitLineClamp: 4,
                    WebkitBoxOrient: "vertical",
                }}
                >
                {item.shortDescription}
                </span>
            </div>
        </div>
    );
};

export default ProductCard;
