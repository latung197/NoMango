import React, { useEffect, useRef, useState } from 'react'

interface MainMenuProps {
  trigger: React.ReactNode
  children: (closeMenu: () => void) => React.ReactNode
  className?: string
}

const MainMenu: React.FC<MainMenuProps> = ({
  trigger,
  children,
  className = '',
}) => {
  const [isOpen, setIsOpen] = useState(false)
  const menuRef = useRef<HTMLDivElement>(null)

  const closeMenu = () => {
    setIsOpen(false)
  }

  const toggleMenu = () => {
    setIsOpen((previous) => !previous)
  }

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (
        menuRef.current &&
        !menuRef.current.contains(event.target as Node)
      ) {
        closeMenu()
      }
    }

    document.addEventListener('mousedown', handleClickOutside)

    return () => {
      document.removeEventListener('mousedown', handleClickOutside)
    }
  }, [])

  return (
    <div
      ref={menuRef}
      className={`relative ${className}`}
    >
      <div
        onClick={toggleMenu}
        className="cursor-pointer"
      >
        {trigger}
      </div>

      {isOpen && children(closeMenu)}
    </div>
  )
}

export default MainMenu