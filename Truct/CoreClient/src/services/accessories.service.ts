import { ApiResponse } from "@/types/ApiResponse";
import api from "../configs/api";
import {AccessoryResponse } from "@/types/Product";

const path = "/accessories";

export async function getAccessoriesByProductId(productId: number) {
    try {
        const res = await api.get<ApiResponse<AccessoryResponse[]>>(path + "/product/" + productId);
        return res?.data;
    } catch (error) {
        console.error("get Accessories By ProductId error: ", error);
        throw new Error("get Accessories By ProductId error");
    }
}