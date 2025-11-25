// import { useState, useEffect, useCallback } from "react";
// import { getProducts } from '@/services/productService';
// import { ProductResponse } from "@/types/products/Product";

// export function usePaginatedProducts(
//   name?: string,
//   categoryId?: number,
//   limit = 30,
//   language?: string // thêm param language
// ) {
//   const [results, setResults] = useState<{ data: ProductResponse[]; meta: any } | null>(null);
//   const [loading, setLoading] = useState(false);
//   const [page, setPage] = useState(0);

//   const fetchData = useCallback(
//     async (pageNumber: number) => {
//       setLoading(true);
//       try {
//         const params: Record<string, any> = { page: pageNumber, limit };
//         if (name) params.name = name;
//         if (categoryId) params.categoryId = categoryId;
//         if (language) params.language = language;
        
//         const res = await getProducts(params);
//         setResults(res);
//         setPage(pageNumber);
//       } catch (err) {
//         console.error(err);
//       } finally {
//         setLoading(false);
//       }
//     },
//     [name, categoryId, limit, language]
//   );

//   useEffect(() => {
//     fetchData(0);
//   }, [fetchData]);

//   const goToPage = (pageNumber: number) => {
//     if (pageNumber >= 0 && results && pageNumber < results.meta.totalPage) {
//       fetchData(pageNumber);
//     }
//   };

//   return { results, loading, page, goToPage };
// }
