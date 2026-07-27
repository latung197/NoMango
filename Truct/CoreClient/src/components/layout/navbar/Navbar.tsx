import React from 'react'
<<<<<<< HEAD
import { useState, useEffect, useRef } from 'react'
import { useTranslation } from 'react-i18next'
import { Link, useNavigate } from 'react-router-dom'
import CategoryDropdown from './CategoryDropdown'
import HoverMenu from './HoverMenu'
import { ensureUserInfo, getToken, logout } from '@/utils/authUtils'
import ProfileDropdown from './ProfileDropdown'

const Navbar: React.FC = () => {
  const { t, i18n } = useTranslation()
  const navigate = useNavigate()
  const [loading, setLoading] = useState(true)
  const dropdownRef = useRef<HTMLDivElement>(null)
  const [showCategory, setShowCategory] = useState(false)
  const categoryButtonRef = useRef<HTMLButtonElement>(null)
  const profileButtonRef = useRef<HTMLButtonElement>(null)
  const [showDropdown, setShowDropdown] = useState(false)
  const [showDropdownProfile, setShowDropdownProfile] = useState(false)
  const [showProductSearch, setShowProductSearch] = useState(false)
  const [showMobileMenu, setShowMobileMenu] = useState(false)
  const searchInputRef = useRef<HTMLInputElement>(null)
  const [query, setQuery] = useState('')

  const toggleCategory = () => {
    setShowCategory((prev) => !prev)
  }

  useEffect(() => {
    if (query.trim().length > 0) {
      setShowProductSearch(true)
    } else {
      setShowProductSearch(false)
    }
  }, [query])

  const handleSearchSubmit = () => {
    if (!query.trim()) return
    navigate(`/search?name=${encodeURIComponent(query)}`)
    setShowDropdown(false)
  }

  const toggleMobileMenu = () => setShowMobileMenu(!showMobileMenu)

  const handleLogout = async () => {
    const out = await logout()
    window.location.reload()
  }

  const [dropdownProfile, setDropdownProfile] = useState({ top: 0, left: 0 })

  useEffect(() => {
    if (showDropdownProfile && profileButtonRef.current) {
      const rect = profileButtonRef.current.getBoundingClientRect()
      setDropdownProfile({
        top: rect.bottom + window.scrollY,
        left: rect.right,
      })
    }
  }, [showDropdownProfile])

  const updateDropdownProfile = () => {
    if (profileButtonRef.current) {
      const rect = profileButtonRef.current.getBoundingClientRect()
      setDropdownProfile({
        top: rect.bottom,
        left: rect.right,
      })
    }
  }

  useEffect(() => {
    if (showDropdownProfile) {
      updateDropdownProfile()
      window.addEventListener('scroll', updateDropdownProfile)
      window.addEventListener('resize', updateDropdownProfile)
      return () => {
        window.removeEventListener('scroll', updateDropdownProfile)
        window.removeEventListener('resize', updateDropdownProfile)
      }
    }
  }, [showDropdownProfile])

  // useEffect(() => {
  //   const handleClickOutside = (e: MouseEvent) => {
  //     if (dropdownRef.current && !dropdownRef.current.contains(e.target as Node)) {
  //       setShowDropdownProfile(false);
  //     }
  //   };
  //   document.addEventListener("mousedown", handleClickOutside);
  //   return () => document.removeEventListener("mousedown", handleClickOutside);
  // }, []);

  // const getCart = () => {
  //   const token = getToken();
  //   if (!getToken()) {
  //       return;
  //   }
  //   navigate(`/user/cart`);
  // }

  return (
    <header className="fixed top-0 left-0 z-50 w-full overflow-x-hidden bg-yellow-500 text-white shadow-md">
      <div className="container mx-auto flex items-center justify-between px-4 py-3">
        <div className="text-xl font-bold">Logo</div>
        <nav className="space-x-4">
          <a href="/" className="hover:text-yellow-200">
            Trang chủ
          </a>
          <a href="/about" className="hover:text-yellow-200">
            Giới thiệu
          </a>
          <a href="/contact" className="hover:text-yellow-200">
            Liên hệ
          </a>
          <a href="/products" className="hover:text-yellow-200">
            Sản phẩm
          </a>
=======
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

>>>>>>> 16b0b8095612890ee1878c16d3c404e090a1ad5e
        </nav>
      </div>
    </header>
  )
}

<<<<<<< HEAD
export default Navbar
=======
export default Navbar
>>>>>>> 16b0b8095612890ee1878c16d3c404e090a1ad5e
