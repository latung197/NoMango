import React from 'react'
import { Link } from 'react-router-dom'

import MainMenu from './MainMenu'
import type { MenuItem } from './menu.config'

interface MenuRendererProps {
  items: MenuItem[]
  closeMenu?: () => void
  level?: number
}

const menuItemClass = `
  flex
  w-full
  items-center
  justify-between
  gap-4
  whitespace-nowrap
  rounded-lg
  px-4
  py-2.5
  text-sm
  font-medium
  text-slate-700
  transition-colors
  duration-150
  hover:bg-emerald-50
  hover:text-emerald-700
`

const rootMenuClass = `
  absolute
  right-0
  top-full
  z-[200]
  mt-3
  min-w-56
  overflow-visible
  rounded-xl
  border
  border-slate-200
  bg-white
  p-2
  shadow-xl
  ring-1
  ring-black/5
`

const submenuClass = `
  absolute
  right-full
  top-0
  z-[210]
  mr-2
  min-w-56
  rounded-xl
  border
  border-slate-200
  bg-white
  p-2
  shadow-xl
  ring-1
  ring-black/5
`

const MenuRenderer: React.FC<MenuRendererProps> = ({
  items,
  closeMenu,
  level = 0,
}) => {
  return (
    <>
      {items.map((item) => {
        const hasChildren =
          item.children && item.children.length > 0

        if (hasChildren) {
          return (
            <MainMenu
              key={item.id}
              trigger={
                <button
                  type="button"
                  className={menuItemClass}
                >
                  <span>{item.label}</span>

                  <span
                    aria-hidden="true"
                    className="text-lg leading-none text-slate-400"
                  >
                    ‹
                  </span>
                </button>
              }
            >
              {(closeSubmenu) => (
                <div
                  className={
                    level === 0
                      ? rootMenuClass
                      : submenuClass
                  }
                >
                  <MenuRenderer
                    items={item.children!}
                    closeMenu={() => {
                      closeSubmenu()
                      closeMenu?.()
                    }}
                    level={level + 1}
                  />
                </div>
              )}
            </MainMenu>
          )
        }

        return (
          <Link
            key={item.id}
            to={item.path ?? '#'}
            onClick={closeMenu}
            className={menuItemClass}
          >
            {item.label}
          </Link>
        )
      })}
    </>
  )
}

export default MenuRenderer