
import { ApiResponse } from "@/types/ApiResponse";
import api from "../configs/api";
import { ProductMetaResponse, ProductByCategoryPriorityResponse, ProductContentResponse, ProductDetailResponse, ProductResponse } from "@/types/Product";

const path = "/products";

export async function getProducts(params?: Record<string, any>) {
    try {
        const res = await api.get<ApiResponse<ProductResponse[]>>(path, { params });
        return res?.data;
    } catch (error) {
        console.error("Get products error: ", error);
        throw new Error("Get products error");
    }
}

export async function getProductGroupByCategory(params?: Record<string, any>) {
  try {
      const res = await api.get<ApiResponse<ProductByCategoryPriorityResponse[]>>(path + "/group-by-category", { params });
      return res?.data;
  } catch (error) {
      console.error("Get products by category priority error: ", error);
      throw new Error("Get products by category priority error");
  }
}

export async function getProductDetail(id: number) {
  try {
    const res = await api.get<ApiResponse<ProductDetailResponse>>(path + "/" + id);
    return res?.data.data;
  } catch (error) {
    console.error(`Get product detail error (id=${id}): `, error);
    throw new Error("Get product detail error");
  }
}

export async function searchFilters(params?: Record<string, any>) {
   try {
        const res = await api.get(path + "/search/filters", { params });
        return res?.data;
    } catch (error) {
        console.error("Get Filter Search error: ", error);
        throw new Error("Get Filter Search error");
    }
}

export async function searchByCategoryFilters(params?: Record<string, any>) {
   try {
        const res = await api.get(path + "/search-by-category/filters", { params });
        return res?.data;
    } catch (error) {
        console.error("Get Filter by category error: ", error);
        throw new Error("Get Filter by category error");
    }
}

export async function searchByCategory(data: any) {
    try {
        const res = await api.post(path + "/search-by-category", data);
        return res?.data;
    } catch (error : any) {
        console.error("Get By Category error: ", error);
         throw new Error("Get Filter by category error");
    }
}

export async function getProductMeta(id: number) {
  try {
    const res = await api.get<ApiResponse<ProductMetaResponse>>(path + "/" + id + "/meta");
    return res?.data.data;
  } catch (error) {
    console.error(`Get product meta error (id=${id}): `, error);
    throw new Error("Get product meta error");
  }
}


export async function getProductContent(id: number, type: string) {
  try {
    const res = await api.get<ApiResponse<ProductContentResponse>>("/product-content/" + type + "/product/" + id);
    return res?.data.data;
  } catch (error) {
    console.error(`Get product content error (id=${id}): `, error);
    throw new Error("Get product content error");
  }
}

export async function getListProductRelatedViews(id: number) {
    try {
        const res = await api.get<ApiResponse<ProductResponse[]>>(path + "/" + id + "/related-views");
        return res?.data;
    } catch (error) {
        console.error("Get products related views error: ", error);
        throw new Error("Get products related views error");
    }
}

export async function getListProductRelatedByCategory(id: number) {
    try {
        const res = await api.get<ApiResponse<ProductResponse[]>>(path + "/" + id + "/related-by-category");
        return res?.data;
    } catch (error) {
        console.error("Get products related by category error: ", error);
        throw new Error("Get products related by category error");
    }
}
