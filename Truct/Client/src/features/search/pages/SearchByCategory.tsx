import { useSearchParams } from "react-router-dom";
import { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from "react-router-dom";
import homeIcon from '@/assets/icons/home.png';
import { searchByCategory, searchByCategoryFilters } from "@/services/product.service";
import { ProductResponse } from "@/types/Product";
import ProductList from "@/components/shared/ProductList";
import Pagination from "@/components/common/Pagination";
import { PaginationMeta } from "@/types/ApiResponse";
import { useCategory } from "@/hooks/useCategory";
import { findCategoryPath } from "@/utils/categoryUtils";
import Breadcrumb from "@/components/common/Breadcrumb";

type FilterAttrOptionResponse = {
    id: number;
    valueText: string | null;
    valueNumber: number | null;
};

type FilterAttrResponse = {
    id: number;
    code: string | null;
    name: string;
    type: 'SELECT_TEXT' | 'SELECT_NUMBER';
    options: FilterAttrOptionResponse[];
};

export default function SearchText() {
    const [searchParams] = useSearchParams();
    const { t, i18n } = useTranslation();
    const categoryId = searchParams.get("categoryId"); 
    const { categories } = useCategory();
    const path = findCategoryPath(categories, Number(categoryId)) || [];

    const [meta, setMeta] = useState<PaginationMeta | null>(null);
    const [page, setPage] = useState(0);

    const [selectedLeadTimes, setSelectedLeadTimes] = useState<number[]>([]);
    const [optionIds, setSelectedOptions] = useState<number[]>([]);

    type FilterData = {
        attributes: FilterAttrResponse[];
        leadTimeDaysList: number[];
    };
    const [filters, setFilters] = useState<FilterData>({
        attributes: [],
        leadTimeDaysList: [],
    });
    const [products, setProducts] = useState<{ data: ProductResponse[]; meta: any } | null>(null);

    useEffect(() => {
        const fetchFilters = async () => {
            const data = await searchByCategoryFilters({ categoryId });
            setFilters(data.data);
        };
        fetchFilters();
    }, [categoryId]);

    useEffect(() => {
        const fetchProducts = async () => {
            const response = await searchByCategory({ 
                page,
                leadTimeDaysList : selectedLeadTimes,
                categoryId: Number(categoryId),
                optionIds
            });
            setProducts(response);
            setMeta(response.meta);
        };
        fetchProducts();
    }, [page , selectedLeadTimes, categoryId, optionIds]);

    const handleLeadTimeChange = (day: number) => {
        setSelectedLeadTimes((prev) =>
            prev.includes(day) ? prev.filter((item) => item !== day) : [...prev, day]
        );
    };

    const handleAttributeOptionChange = (optionId: number) => {
        setSelectedOptions((prev) => {
            const exists = prev.includes(optionId);
            return exists
            ? prev.filter((id) => id !== optionId)
            : [...prev, optionId];
        });
    };


    return (
        <div className="p-4">
            <Breadcrumb
                path={path}
                categoryId={Number(categoryId)}
                total={products?.meta?.total}
            />
            <div className="flex justify-between items-start mb-4 w-[100%] mx-auto gap-x-4">
                <div className="w-[20%] ">
                    <div className="text-left border border-gray-300 rounded max-h-screen overflow-y-auto">
                        <div className="bg-[#02CECF] text-white text-center px-4 py-2 text-sm font-medium w-full sticky top-0">
                            絞り込み条件
                        </div>
                        <div className="mb-4 px-4 my-4">
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

                            {filters?.attributes?.map((attr) => {
                                const selectedOptions = attr.options.filter((opt) => optionIds.includes(opt.id));
                                return (
                                    <div key={attr.id} className="mt-4">
                                        <h3 className="font-semibold mb-2 text-sm">{attr.name}</h3>
                                        <div className="border border-gray-300 rounded p-4 space-y-2 text-xs">
                                            {attr.options
                                            .filter((opt) => selectedOptions.length === 0 || optionIds.includes(opt.id))
                                            .map((opt) => (
                                                <label key={opt.id} className="flex items-center space-x-2">
                                                <input
                                                    type="checkbox"
                                                    className="mr-2"
                                                    checked={optionIds.includes(opt.id)}
                                                    onChange={() => handleAttributeOptionChange(opt.id)}
                                                />
                                                {attr.type === 'SELECT_TEXT' && opt.valueText}
                                                {attr.type === 'SELECT_NUMBER' && `${opt.valueNumber}`}
                                                </label>
                                            ))}
                                            {selectedOptions.length > 0 && (
                                            <div className="text-right mt-2">
                                                <button
                                                onClick={() => {
                                                    selectedOptions.forEach((opt) => handleAttributeOptionChange(opt.id));
                                                }}
                                                className="text-blue-600 text-xs underline hover:text-blue-800"
                                                >
                                                選択を解除する
                                                </button>
                                            </div>
                                            )}
                                        </div>
                                    </div>
                                );
                            })}
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
