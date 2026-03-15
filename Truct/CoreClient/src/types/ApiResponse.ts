
export interface PaginationMeta {
  totalPage: number;
  currentPage: number;
  size: number;
  total: number;
}

export interface ApiResponse<T> {
  data: T;
  meta: PaginationMeta;
}