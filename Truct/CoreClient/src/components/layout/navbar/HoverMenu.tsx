import React, { useState, useRef } from "react";

type HoverMenuProps = {
  trigger: React.ReactNode;
  children?: ((closeMenu: () => void) => React.ReactNode);
  delay?: number;
  onClose?: () => void;
};

const HoverMenu: React.FC<HoverMenuProps> = ({ trigger, children, delay = 150, onClose }) => {
  const [show, setShow] = useState(false);
  const timeoutRef = useRef<NodeJS.Timeout | null>(null);

  const handleMouseEnter = () => {
    if (timeoutRef.current) clearTimeout(timeoutRef.current);
    setShow(true);
  };

  const handleMouseLeave = () => {
    timeoutRef.current = setTimeout(() => setShow(false), delay);
  };

  const closeMenu = () => {
    setShow(false);
    onClose?.();
  };


  return (
    <div
      className="relative inline-block"
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
    >
      {trigger}
      {show && (
        <div className="absolute left-0 mt-2 z-50 bg-white shadow-lg rounded">
          {typeof children === 'function' ? children(closeMenu) : children}
        </div>
      )}
    </div>
  );
};

export default HoverMenu;
