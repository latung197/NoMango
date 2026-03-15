interface CategoryResponse {
  id: number;
  languageCode: string;
  name: string;
  children: CategoryResponse[]; 
  createdAt: string;
}