import { useSearchParams } from "react-router-dom";
import { useState, useEffect, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from "react-router-dom";
import homeIcon from '@/assets/icons/home.png';
import { useNavigate } from "react-router-dom";
import { ProductResponse } from "@/types/Product";
import ProductList from "@/components/shared/ProductList";
import Pagination from "@/components/common/Pagination";
import { PaginationMeta } from "@/types/ApiResponse";
import { getWishlistFilter, getWishlists } from "@/services/wishlist.service";
import { getToken } from "@/utils/authUtils";
import Breadcrumb from "@/components/common/Breadcrumb";

export default function SearchText() {
    const token = getToken();
    if (!token) {
        window.location.href = "/login";
    }

    const { t, i18n } = useTranslation();
    const [meta, setMeta] = useState<PaginationMeta | null>(null);
    const [page, setPage] = useState(0);
    const [selectedCategories, setSelectedCategories] = useState<number[]>([]);
    const [selectedLeadTimes, setSelectedLeadTimes] = useState<number[]>([]);

    type FilterData = {
        categories: { id: number; name: string }[];
        leadTimeDaysList: number[];
    };
    const [filters, setFilters] = useState<FilterData>({
        categories: [],
        leadTimeDaysList: [],
    });
    const [products, setProducts] = useState<{ data: ProductResponse[]; meta: any } | null>(null);
    useEffect(() => {
        const fetchFilters = async () => {
            const data = await getWishlistFilter();
            setFilters(data.data);
        };
        fetchFilters();
    }, []);

    useEffect(() => {
        const fetchProducts = async () => {
            const response = await getWishlists({ 
                page,
                categoryId: selectedCategories,
                leadTimeDays: selectedLeadTimes,
            });
            setProducts(response);
            setMeta(response.meta);
        };
        fetchProducts();
    }, [page, selectedCategories, selectedLeadTimes]);

    const handleCategoryChange = (id: number) => {
        setSelectedCategories((prev) =>
            prev.includes(id) ? prev.filter((item) => item !== id) : [...prev, id]
        );
    };

    const handleLeadTimeChange = (day: number) => {
        setSelectedLeadTimes((prev) =>
            prev.includes(day) ? prev.filter((item) => item !== day) : [...prev, day]
        );
    };


    return (
        <div className="p-4">
            <Breadcrumb
                name={"お気に入り"}
                total={products?.meta?.total}
            />
            <div className="flex justify-between items-start mb-4 w-[100%] mx-auto gap-x-4">
                <div className="w-[20%] ">
                    <div className="text-left border border-gray-300 rounded">
                        <div className="bg-[#02CECF] text-white text-center px-4 py-2 text-sm font-medium w-full sticky top-0">
                            絞り込み条件
                        </div>
                        <div className="mb-4 px-4 my-4">
                            {filters?.categories?.length > 0 && (
                                <div className="">
                                    <h3 className="font-semibold mb-2 text-sm">商品カテゴリ</h3>
                                    <div className="border border-gray-300 rounded p-4 space-y-2 text-xs">
                                    {filters.categories
                                        .filter((cat) => selectedCategories.length === 0 || selectedCategories.includes(cat.id))
                                        .map((cat) => (
                                        <label key={cat.id} className="flex items-center space-x-2">
                                            <input
                                            type="checkbox"
                                            className="mr-2"
                                            checked={selectedCategories.includes(cat.id)}
                                            onChange={() => handleCategoryChange(cat.id)}
                                            />
                                            {cat.name}
                                        </label>
                                        ))}
                                    {selectedCategories.length > 0 && (
                                        <div className="text-right mt-2">
                                        <button
                                            onClick={() => setSelectedCategories([])}
                                            className="text-blue-600 text-xs underline hover:text-blue-800"
                                        >
                                            選択を解除する
                                        </button>
                                        </div>
                                    )}
                                    </div>
                                </div>
                                )}

                            {filters?.leadTimeDaysList?.length > 0 && (
                                <div className="mt-4">
                                    <h3 className="font-semibold mb-2 text-sm">発送日</h3>
                                    <div className="border border-gray-300 rounded p-4 space-y-2 text-xs">
                                    {filters.leadTimeDaysList
                                        .filter((day) => selectedLeadTimes.length === 0 || selectedLeadTimes.includes(day))
                                        .map((day, index) => (
                                        <label key={index} className="flex items-center space-x-2">
                                            <input
                                            type="checkbox"
                                            className="mr-2"
                                            checked={selectedLeadTimes.includes(day)}
                                            onChange={() => handleLeadTimeChange(day)}
                                            />
                                            {day} 日
                                        </label>
                                        ))}
                                    {selectedLeadTimes.length > 0 && (
                                        <div className="text-right mt-2">
                                        <button
                                            onClick={() => setSelectedLeadTimes([])}
                                            className="text-blue-600 text-xs underline hover:text-blue-800"
                                        >
                                            選択を解除する
                                        </button>
                                        </div>
                                    )}
                                    </div>
                                </div>
                                )}
                        </div>
                    </div>
                </div>
                <div className="w-[80%] text-left">
                    {products && <ProductList products={products} setProducts={setProducts}/>}
                    {meta && meta.totalPage > 1 && (
                        <Pagination
                            currentPage={meta?.currentPage ?? 0}
                            totalPage={meta.totalPage ?? 1}
                            onPageChange={(page) => setPage(page)}
                        />
                    )}

                </div>
            </div>
        </div>
    );
}
