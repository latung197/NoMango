import { ProductResponse } from "@/types/Product";
import { useEffect, useRef, useState } from "react";

interface Props {
    query: string;
    loading: boolean;
    results: ProductResponse[];
    onSelect: (id: string) => void;
    t: (key: string) => string;
    inputRef: React.RefObject<HTMLInputElement | null>;
    onClose: () => void;
}

const ProductSearchDropdown: React.FC<Props> = ({ query, loading, results, onSelect, t, inputRef, onClose }) => {
  const dropdownRef = useRef<HTMLDivElement>(null);
  const [position, setPosition] = useState({ top: 0, left: 0, width: 0 });

  useEffect(() => {
    if (inputRef.current) {
      const rect = inputRef.current.getBoundingClientRect();
      setPosition({
        top: rect.bottom + 8,
        left: rect.left,
        width: rect.width,
      });
    }
  }, [query, results]);

    useEffect(() => {
        const handleClickOutside = (e: MouseEvent) => {
            const target = e.target as Node;

            if (
            dropdownRef.current &&
            !dropdownRef.current.contains(target) &&
            inputRef.current &&
            !inputRef.current.contains(target)
            ) {
                onClose();
            }
        };

        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, [onClose, inputRef]);

  if (!query) return null;
  return (
    <div
      ref={dropdownRef}
      className="fixed bg-white shadow-md rounded-md max-h-60 overflow-y-auto z-[999]"
      style={{ top: position.top, left: position.left, width: position.width }}
    >
      {results.length ? (
        results.map((item) => (
          <div
            key={item.id}
            className="flex items-center gap-3 p-3 hover:bg-gray-100 cursor-pointer"
            onClick={() => onSelect(String(item.id))}
          >
            <img src={item.thumbnailUrl || "/default.png"} alt={item.name} className="w-10 h-10 object-cover rounded-md border" />
            <span className="truncate text-sm font-medium text-gray-700">{item.name}</span>
          </div>
        ))
      ) : (
        <div className="flex items-center justify-center h-8 text-gray-500 italic">{t('search.noResults')}</div>
      )}
    </div>
  );
};

export default ProductSearchDropdown;