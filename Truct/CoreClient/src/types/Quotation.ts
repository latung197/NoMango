
export interface QuotationRequest {
  productId: number;
  name: string | null;
  company: string | null;
  position: string | null;
  address: string | null;
  email: string | null;
  phoneNumber: string | null;
  message: string | null;
  shippingMethodId: number | null;
  quantity: number;
  attributes: QuotationAttributeRequest[] | null;
}

export interface QuotationAttributeRequest {
    productAttributeId: number;
    valueNumber: number | null;
    valueText: string | null;
}

export interface QuotationResponse {
  id: number;
  company: string;
  address: string;
  phoneNumber: string;
  email: string;
  name: string;
  createdAt: string;
  expiredAt: string;
  products: QuotationProductResponse[];
  subTotalPrice: number;
  vat: number;
  vatFee: number;
  shippingFee: number;
  total: number;
}

export interface QuotationProductResponse {
  name: string;
  code: string;
  quantity: number;
  unit: string;
  unitPrice: number;
  // vat: number;
  amount: number;
}

export interface FormDataRequest {
  name?: string;
  email?: string;
  phoneNumber?: string;
  company?: string;
  position?: string;
  address?: string;
  message?: string;
}

export interface QuotationFromCartRequest {
  message: string | null;
  shippingMethodId: number | null;
  cartIds: number[];
}

export interface QuotationHistoryResponse {
  id: number;
  message: string;
  shippingMethodId: number;
  shippingMethodCode: string;
  subTotalPrice: number;
  vat: number;
  vatFee: number;
  shippingFee: number;
  total: number;
  createdAt: string;
  expiredAt: string;
}

export interface QuotationHistoryRequest {
  fromDate?: string;
  toDate?: string;
  page?: number;
  limit?: number;
}