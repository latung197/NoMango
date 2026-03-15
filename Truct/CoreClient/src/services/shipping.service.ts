
import { ApiResponse } from "@/types/ApiResponse";
import api from "../configs/api";
import { ShippingMethod } from "@/types/Shipping";

const path = "/shipping-methods";

export async function findAllShippingMethods() {
   try {
  const res = await api.get<ApiResponse<ShippingMethod[]>>(path);
  return res?.data.data;
  } catch (error) {
    console.log("Find all shipping methods error: ", error);
    throw new Error("Find all shipping methods error");
  }
}