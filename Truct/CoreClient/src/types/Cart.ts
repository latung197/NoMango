import { QuotationAttributeRequest } from "./Quotation";

export interface AddToCartRequest {
  productId: number;
  quantity: number;
  attributes: QuotationAttributeRequest[] | null;
}


export interface CartResponse {
  id: number;
  productId: number;
  productName: string;
  code: string;
  thumbnailUrl: string;
  quantity: number;
}