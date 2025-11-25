import { useState, useRef } from 'react';
import { useNavigate } from "react-router-dom";

const CategoryItem = ({ category, level = 0, asideHeight, top, onSelectCategory }: { category: any; level?: number; asideHeight : number; top: number, onSelectCategory?: () => void; }) => {  
  const navigate = useNavigate();
  const [hoveredCategory, setHoveredCategory] = useState<CategoryResponse | null>(null);
  const [hoverPosition, setHoverPosition] = useState<{ top: number; left: number }>({ top: 0, left: 0 });
  const buttonRef = useRef<HTMLButtonElement>(null);
  const timeoutRef = useRef<NodeJS.Timeout | null>(null);

  const handleMouseLeave = () => {
    timeoutRef.current = setTimeout(() => {
      setHoveredCategory(null);
    }, 150);
  };

  const handleMouseEnter = () => {
    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current);
      timeoutRef.current = null;
    }
    if (category?.children?.length > 0) {
      setHoveredCategory(category);
      if (buttonRef.current) {
        const rect = buttonRef.current.getBoundingClientRect();
        setHoverPosition({
          top: top,
          left: rect.right + 10,
        });
      }
    }
  };

  return (
    <div className="mb-1"
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
     >
      <button
        ref={buttonRef}
        onMouseEnter={() => {
          if (category?.children?.length > 0) {
            setHoveredCategory(category);
          }
        }}
        className="flex items-center justify-between w-full px-4 py-2 rounded hover:bg-gray-100 transition"
        style={{ paddingLeft: `${level * 16}px` }}
      >
        <div className="flex items-center space-x-3 flex-grow overflow-hidden">
          <span
            className=" text-xs font-medium block w-full text-left"
            title={category?.name}
          >
            {category?.name}
          </span>
        </div>
        {category?.children?.length > 0 && (
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className={`h-4 w-4 text-gray-500 transform transition-transform`}
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M9 5l7 7-7 7"
            />
          </svg>
        )}
      </button>
      {hoveredCategory?.id === category?.id && category?.children?.length > 0 && (
        <div
          className="fixed bg-white shadow-lg rounded p-4 z-50 w-[800px]"
          style={{
            top: hoverPosition.top,
            left: hoverPosition.left,
            minHeight: `${asideHeight}px`,
          }}
        >
          <div className="grid grid-cols-3 gap-4">
            {category.children.map((child: any) => (
              <div
                key={child.id}
                className="flex flex-col items-center text-center hover:bg-gray-100 p-2 rounded cursor-pointer"
                onClick={() => {
                  onSelectCategory?.()
                  navigate(`/search-by-category?categoryId=${child.id}`, {replace: true});
                  setHoveredCategory(null);
                }}
              >
                <img
                  src={child.thumbnailUrl}
                  alt={child.name}
                  className="h-16 w-16 object-contain mb-2"
                />
                <span className="text-sm font-medium truncate">{child.name}</span>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};
export default CategoryItem;