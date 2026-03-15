import { ApiResponse } from "@/types/ApiResponse";
import api from "../configs/api";
import { ProductResponse } from "@/types/Product";
import { handleApiError } from "@/utils/handleApiError";

const path = "/wishlist";

export async function getWishlists(params?: Record<string, any>) {
    try {
        const res = await api.get<ApiResponse<ProductResponse[]>>(path, { params });
        return res?.data;
    } catch (error) {
        console.error("Get Wishlist error: ", error);
        throw new Error("Get Wishlist error");
    }
}

export async function getWishlistFilter(params?: Record<string, any>) {
    try {
        const res = await api.get(path + "/filter", { params });
        return res?.data;
    } catch (error) {
        console.error("Get Wishlist filter error: ", error);
        throw new Error("Get Wishlist filter error");
    }
}

export async function toggleWishlist(data: any) {
    try {
        const res = await api.post(path + "/toggle", JSON.stringify(data));
        return res?.data;
    } catch (error : any) {
        console.error("toggle Wishlist error: ", error);
        handleApiError(error);
    }
}