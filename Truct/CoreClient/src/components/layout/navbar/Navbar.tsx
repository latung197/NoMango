import React from 'react'
import { Link } from 'react-router-dom'

import MainMenu from './MainMenu'
import MenuRenderer from './MenuRenderer'
import {
  accountMenuItems,
  mainMenuItems,
} from './menu.config'

const Navbar: React.FC = () => {
  return (
    <header className="fixed left-0 top-0 z-[100] w-full border-b border-emerald-600/30 bg-fuchsia-100 text-white shadow-lg">
      <div className="mx-auto flex h-16 max-w-8xl  px-2 sm:px-2 lg:px-2">

        {/* Logo */}
        <Link
          to="/"
          className="flex items-center gap-1 text-xl font-bold tracking-tight"
        >
          <span className="flex h-15 w-30 items-center justify-center rounded-lg bg-white/15">
            <img src="/src/assets/LOGO FSTV-01.png" alt="Logo" className="h-full w-full object-contain" />
          </span>
        </Link>

        {/* Navigation */}
        <nav className="flex items-center gap-2">

          {/* Home */}
          <Link
            to="/"
            className="
              rounded-lg
              px-4
              py-2
              text-sm
              font-medium
              text-white/90
              transition
              hover:bg-white/10
              hover:text-white
            "
          >
            Trang chủ
          </Link>

          {/* Products */}
          {/* <MainMenu
            trigger={
              <button
                type="button"
                className="
                  rounded-lg
                  px-4
                  py-2
                  text-sm
                  font-medium
                  text-white/90
                  transition
                  hover:bg-white/10
                  hover:text-white
                "
              >
                Sản phẩm
              </button>
            }
          >
            {(closeMenu) => (
              <div className="
                absolute
                right-0
                top-full
                z-[200]
                mt-3
                min-w-56
                rounded-xl
                border
                border-slate-200
                bg-white
                p-2
                shadow-xl
                ring-1
                ring-black/5
              ">
                <MenuRenderer
                  items={mainMenuItems}
                  closeMenu={closeMenu}
                />
              </div>
            )}
          </MainMenu> */}

          {/* Account */}
          {/* <MainMenu
            trigger={
              <button
                type="button"
                className="
                  rounded-lg
                  border
                  border-white/70
                  px-4
                  py-2
                  text-sm
                  font-medium
                  text-white
                  transition
                  hover:bg-white
                  hover:text-emerald-700
                "
              >
                Tài khoản
              </button>
            }
          >
            {(closeMenu) => (
              <div className="
                absolute
                right-0
                top-full
                z-[200]
                mt-3
                min-w-48
                rounded-xl
                border
                border-slate-200
                bg-white
                p-2
                shadow-xl
                ring-1
                ring-black/5
              ">
                <MenuRenderer
                  items={accountMenuItems}
                  closeMenu={closeMenu}
                />
              </div>
            )}
          </MainMenu> */}

        </nav>
      </div>
    </header>
  )
}

export default Navbar