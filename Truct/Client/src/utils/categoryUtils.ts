
export function findCategoryPath(
  categories: CategoryResponse[],
  targetId: number
): CategoryResponse[] | null {
  for (const category of categories) {
    if (category.id === targetId) {
      return [category];
    }
    if (category.children?.length) {
      const childPath = findCategoryPath(category.children, targetId);
      if (childPath) {
        return [category, ...childPath];
      }
    }
  }
  return null;
}