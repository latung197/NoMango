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

    <div></div>
  );
};

export default CategoryDropdown;
