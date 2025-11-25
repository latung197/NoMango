import { ShippingMethod } from "./Shipping";

export interface ProductResponse {
  id: number;
  name: string;
  thumbnailUrl: string;
  shortDescription: string;
  categoryId: number;
  leadTimeDays: number;
  isWishlist?: boolean;
}

export interface ProductGroup {
  id: string;
  title: string;
  products: ProductResponse[];
}


export interface ProductByCategoryPriorityResponse {
  categoryId?: number;
  categoryName: string;
  products: ProductResponse[];
}

export interface ProductDetailResponse {
  id: number;
  categoryId?: number;
  name?: string;
  shortDescription?: string;
  thumbnailUrl?: string;
  imageUrls?: string[];
  leadTimeDays? : number;
  note: string;
  isWishlist: boolean
}

export enum AttributeType {
  ENTER_NUMBER = "ENTER_NUMBER",
  SELECT_NUMBER = "SELECT_NUMBER",
  SELECT_TEXT = "SELECT_TEXT",
  FIX_NUMBER = "FIX_NUMBER",
  FIX_TEXT = "FIX_TEXT"
}



export interface AttributeOption {
  productAttributeId: number;
  valueText: string | null;
  valueNumber: number | null;
}

export interface Attribute {
  attributeId: number;
  productAttributeId: number;
  code: string | null;
  name: string;
  type: AttributeType
  minValue: number | null;
  maxValue: number | null;
  valueText: string | null;
  valueNumber: number | null;
  unit: string | null;
  options: AttributeOption[] | null;
}

export interface ProductMetaResponse {
  shippingMethods: ShippingMethod[];
  attributes: Attribute[];
  skus: SkuAttrResponse[];
}


export interface SkuAttrResponse {
  skuId: number;
  sku: string;
  productAttributeIds: number[];
  attributeIdMap: Record<number, number>;
}

export interface ProductContentResponse {
  productId: number;
  contentType: string;
  htmlContent: string;
}

export interface AccessoryResponse {
  id: number;
  code: string;
  name: string;
  description: string;
  thumbnailUrl: string;
  quantity: number;
}
