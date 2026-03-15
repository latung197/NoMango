
import { ApiResponse } from "@/types/ApiResponse";
import api from "../configs/api";
import { getApiMessage } from "@/utils/apiMessage";
import { toast } from "sonner";
import { AddToCartRequest, CartResponse } from "@/types/Cart";

const path = "/cart";

export async function addToCart(data: AddToCartRequest) {
    try {
        const res = await api.post<ApiResponse<void>>(path + "/add", data);
        return res?.data;
    } catch (error : any) {
        console.error("addToCart error: ", error);
        const message = getApiMessage(error);
        toast.error(message);
        throw error;
    }
}

export async function getCartCount() {
   try {
  const res = await api.get(path + "/count");
  return res?.data.data;
  } catch (error) {
    console.log("Get cart count error: ", error);
    throw new Error("Get cart count error");
  }
}

export async function getListCarts() {
    try {
        const res = await api.get<ApiResponse<CartResponse[]>>(path, {});
        return res?.data;
    } catch (error) {
        console.error("Get list carts error: ", error);
        throw new Error("Get list carts error");
    }
}

export async function deleteById(id: number) {
  try {
    const res = await api.delete(path + "/" + id);
    return res?.data;
  } catch (error) {
    console.error(`Delete cart by id error (id=${id}): `, error);
    throw new Error("Delete cart by id error");
  }
}