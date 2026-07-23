

export interface AddToCartRequest {
  productId: number;
  quantity: number;
}


export interface CartResponse {
  id: number;
  productId: number;
  productName: string;
  code: string;
  thumbnailUrl: string;
  quantity: number;
}