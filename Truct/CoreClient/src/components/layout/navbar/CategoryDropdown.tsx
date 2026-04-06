import { useEffect, useRef, useState } from "react";
import CategoryItem from './CategoryItem';

interface Props {
  triggerRef: React.RefObject<HTMLButtonElement | null>;
  categories: CategoryResponse[];
  t: (key: string) => string;
  onSelectCategory?: () => void;
}

const CategoryDropdown: React.FC<Props> = ({ triggerRef, categories, t, onSelectCategory}) => {
  const dropdownRef = useRef<HTMLDivElement>(null);
  const [position, setPosition] = useState<{ top: number; left: number } | null>(null);
  const [dropdownHeight, setDropdownHeight] = useState<number>(0);

  const updatePosition = () => {
    if (triggerRef.current) {
      const rect = triggerRef.current.getBoundingClientRect();
      const dropdownWidth = 288;
      const safeLeft = Math.min(rect.left, window.innerWidth - dropdownWidth);
      const safeTop = rect.bottom + 8;
      setPosition({ top: safeTop, left: safeLeft });
    }
  };

  useEffect(() => {
    const handleScroll = () => updatePosition();
    window.addEventListener("scroll", handleScroll);
    window.addEventListener("resize", handleScroll);
    const observer = new ResizeObserver(() => {
      updatePosition();
    });

    if (triggerRef.current) {
      observer.observe(triggerRef.current);
    }

    return () => {
      window.removeEventListener("scroll", handleScroll);
      window.removeEventListener("resize", handleScroll);
      observer.disconnect();
    };
  }, [triggerRef]);
    
  useEffect(() => {
    if (dropdownRef.current) {
      const rect = dropdownRef.current.getBoundingClientRect();
      setDropdownHeight(rect.height);
    }
  }, [position]); 

  return (
    position && (
      <div
        ref={dropdownRef}
        className="fixed w-72 bg-white shadow rounded z-[999] text-black"
        style={{ top: position.top, left: position.left }}
      >
        <div className="bg-[#02CECF] text-white px-4 py-2 text-sm font-semibold">
          <span className="truncate max-w-[120px]">{t('home.category')}</span>
        </div>
        <div className="p-4 h-[400px] overflow-y-auto">
          {categories.map((cat) => (
            <CategoryItem key={cat.id} category={cat} asideHeight={dropdownHeight} top={position.top} onSelectCategory={onSelectCategory}/>
          ))}
        </div>
      </div>
    )
  );
};

export default CategoryDropdown;
