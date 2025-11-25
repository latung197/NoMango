import React from 'react';
import { useState, useEffect, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { useCategory } from '../../hooks/useCategory';
import { Link, useNavigate } from "react-router-dom";

import cartIcon from '@/assets/icons/cart.png';
import cartWishListIcon from '@/assets/icons/cart_wishlist.png';
import categoryIcon from '@/assets/icons/categories.png';
import searchIcon from '@/assets/icons/search.png';
import { useAutocompleteSearch } from './navbar/AutoCompleteSearch';
import CategoryDropdown from './navbar/CategoryDropdown';
import ProductSearchDropdown from './navbar/ProductSearchDropdown';
import HoverMenu from './navbar/HoverMenu';
import { ensureUserInfo, getToken, logout } from '@/utils/authUtils';
import ProfileDropdown from './navbar/ProfileDropdown';
import { useUserInfo } from '@/hooks/useUserInfo';
import { getCartCount } from '@/services/cart.service';
import { useCart } from '@/context/CartContext';

const Navbar: React.FC = () => {

  const {t, i18n} = useTranslation();
  const { show, toggle } = useCategory();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const dropdownRef = useRef<HTMLDivElement>(null);
  const [showCategory, setShowCategory] = useState(false);
  const categoryButtonRef = useRef<HTMLButtonElement>(null);
   const profileButtonRef = useRef<HTMLButtonElement>(null);
  const [showDropdown, setShowDropdown] = useState(false);
  const [showDropdownProfile, setShowDropdownProfile] = useState(false);
  const [showProductSearch, setShowProductSearch] = useState(false);
  const [showMobileMenu, setShowMobileMenu] = useState(false);
  const searchInputRef = useRef<HTMLInputElement>(null);
  const [query, setQuery] = useState("");
  const { results } = useAutocompleteSearch(query, 30);
  const { categories } = useCategory();
  const [localCategories, setLocalCategories] = useState(categories);
  const { userInfo, setUserInfo } = useUserInfo();
  const { cartCount, setCartCount } = useCart();


  const toggleCategory = () => {
    setShowCategory(prev => !prev);
  };

  useEffect(() => {
    setLocalCategories(categories);
  }, [categories]);

  useEffect(() => {
    if (query.trim().length > 0) {
      setShowProductSearch(true);
    } else {
      setShowProductSearch(false);
    }
  }, [query]);

  const handleSearchSubmit = () => {
    if (!query.trim()) return;
    navigate(`/search?name=${encodeURIComponent(query)}`);
    setShowDropdown(false);
  }

  const toggleMobileMenu = () => setShowMobileMenu(!showMobileMenu);

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      const target = event.target as HTMLElement;
      if (dropdownRef.current && !dropdownRef.current.contains(target)) {
        toggle();
      }
    }
    if (show) {
      document.addEventListener("mousedown", handleClickOutside);
    }
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [show]);

  useEffect(() => {
    const fetchUser = async () => {
      const user = await ensureUserInfo();
      setUserInfo(user);
    };

    fetchUser();
  }, []);

  const handleLogout = async () => {
    const out = await logout();
    window.location.reload();
  };

  const [dropdownProfile, setDropdownProfile] = useState({ top: 0, left: 0 });

  useEffect(() => {
    if (showDropdownProfile && profileButtonRef.current) {
      const rect = profileButtonRef.current.getBoundingClientRect();
      setDropdownProfile({
        top: rect.bottom + window.scrollY,
        left: rect.right,
      });
    }
  }, [showDropdownProfile]);

  const updateDropdownProfile = () => {
    if (profileButtonRef.current) {
      const rect = profileButtonRef.current.getBoundingClientRect();
      setDropdownProfile({
        top: rect.bottom,
        left: rect.right,
      });
    }
  };

  useEffect(() => {
    if (showDropdownProfile) {
      updateDropdownProfile();
      window.addEventListener("scroll", updateDropdownProfile);
      window.addEventListener("resize", updateDropdownProfile);
      return () => {
        window.removeEventListener("scroll", updateDropdownProfile);
        window.removeEventListener("resize", updateDropdownProfile);
      };
    }
  }, [showDropdownProfile]);


  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target as Node)) {
        setShowDropdownProfile(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  useEffect(() => {
    const token = getToken();
    if (!token) {
        return;
    }
    async function loadCartCount() {
      try {
        const count = await getCartCount();
        setCartCount(count);
      } catch (error) {
        console.error('Failed to fetch cart count', error);
      }
    }
    loadCartCount();
  }, []);

  const getCart = () => {
    const token = getToken();
    if (!getToken()) {
        return;
    }
    navigate(`/user/cart`);
  }

  return (
    <>
      <div>
        <header className="w-full fixed top-0 left-0 z-50 bg-[#02CECF] text-white shadow overflow-x-hidden">
          <div className="flex items-center justify-between px-6 py-3">
            {/* Menu desktop */}
            <div className="hidden md:flex items-center gap-x-6 w-full justify-center">
              <HoverMenu
                trigger={
                  <button ref={categoryButtonRef} 
                    className="flex items-center space-x-2 bg-white text-[#02CECF] px-3 py-2 rounded hover:bg-gray-100"
                    title={t('navbar.category')}
                  >
                    <img src={categoryIcon} alt={t('navbar.category')} className="h-4 w-4" />
                    <span className="text-sm font-semibold">{t('navbar.category')}</span>
                    <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                    </svg>
                  </button>
                }
              >
                {(closeMenu) => (
                  <CategoryDropdown
                    triggerRef={categoryButtonRef}
                    categories={localCategories}
                    t={t}
                    onSelectCategory={() => {
                      closeMenu();
                    }}
                  />
                )}
              </HoverMenu>
              {/* Ô tìm kiếm */}
              <div className="relative flex-grow max-w-3xl">
                <input
                 ref={searchInputRef}
                  type="text"
                  value={query}
                  onChange={(e) => {
                    setQuery(e.target.value);
                    setShowProductSearch(true);
                  }}
                  onKeyDown={(e) => e.key === "Enter" && handleSearchSubmit()}
                  placeholder={t('navbar.search')}
                  className="w-full px-4 py-2 pr-10 rounded-md text-sm text-black outline-none border border-gray-300"
                />
                <button onClick={handleSearchSubmit} className="absolute right-3 top-1/2 transform -translate-y-1/2 transition hover:scale-110 hover:brightness-90">
                  <img src={searchIcon} alt="Search" className="h-5 w-5" />
                </button>
              </div>

              {/* Icon và nút đăng nhập */}
              <div className="flex items-center space-x-14 text-sm font-medium pl-12">
                <div className="relative" 
                  onClick={getCart}
                >
                  <img
                    src={cartIcon}
                    alt={t('navbar.cart')}
                    className="h-6 cursor-pointer"
                    title={t('navbar.cart')}
                  />
                  {cartCount > 0 && (
                    <span className="absolute -top-1 -right-2 bg-red-500 text-white text-[10px] rounded-full px-1 py-0.5 leading-none">
                      {cartCount}
                    </span>
                  )}
                </div>
                <Link to="/user/wishlist">
                  <img
                    src={cartWishListIcon}
                    alt={t('navbar.wishlist')}
                    className="h-6 cursor-pointer"
                  />
                </Link>
                {userInfo ? (
                  <div className="relative flex items-center space-x-2">
                    <span 
                      ref={profileButtonRef}
                      className="text-base font-medium text-white cursor-pointer hover:underline"
                      onClick={() => setShowDropdownProfile(!showDropdownProfile)}
                    >
                      {userInfo.name}
                    </span>
                  </div>
                ) : (
                  <button 
                    onClick={() => navigate('/login')}
                    className="flex items-center space-x-2 bg-white text-[#02CECF] px-3 py-1 rounded hover:bg-gray-100"
                  >
                    <span className="text-sm font-medium">{t('navbar.login')}</span>
                  </button>
                )}
              </div>
            </div>

            {/* Menu mobile */}
            <div className="md:hidden flex items-center justify-between w-full gap-4 px-0">
              {/* Nút danh mục */}
              <button
                className="flex items-center space-x-2 bg-white text-[#02CECF] px-2 py-2 rounded hover:bg-gray-100"
                title={t('navbar.category')}
              >
                <img src={categoryIcon} alt={t('navbar.category')} className="h-4 w-4" />
                <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                </svg>
              </button>

              {/* Ô tìm kiếm */}
              <div className="relative w-[200px] md:w-[300px]">
                <input
                  // ref={searchInputRef}
                  type="text"
                  value={query}
                  onChange={(e) => {
                    setQuery(e.target.value);
                    // setShowProductSearch(true);
                  }}
                  onKeyDown={(e) => e.key === "Enter" && handleSearchSubmit()}
                  placeholder={t('navbar.search')}
                  className="w-full px-2 py-2 pr-10 rounded-md text-sm text-black outline-none border border-gray-300"
                />
                <button
                  onClick={handleSearchSubmit}
                  className="absolute right-3 top-1/2 transform -translate-y-1/2 transition hover:scale-110 hover:brightness-90"
                >
                  <img src={searchIcon} alt="Search" className="h-5 w-5" />
                </button>
              </div>

              {/* Giỏ hàng, wishlist, đăng nhập */}
              <div className="flex items-center space-x-2 text-sm font-medium">
                <img src={cartIcon} alt={t('navbar.cart')} className="h-6 cursor-pointer" title={t('navbar.cart')} />
                <img src={cartWishListIcon} alt={t('navbar.wishlist')} className="h-6 cursor-pointer" />
                <button className="flex items-center space-x-2 bg-white text-[#02CECF] px-3 py-1 rounded hover:bg-gray-100">
                  <span className="text-xs font-medium">{t('navbar.login')}</span>
                </button>
              </div>
            </div>
          </div>
        </header>
      </div>
      <div>
        {showProductSearch && (
          <ProductSearchDropdown
            query={query}
            loading={loading}
            results={results}
            onClose={() => setShowProductSearch(false)}
            onSelect={(id) => navigate(`/product/${id}`)}
            t={t}
            inputRef={searchInputRef}
          />
        )}
      </div>
      <div>
        {showDropdownProfile && (
          <div
           ref={dropdownRef}
            className="fixed bg-white shadow-lg rounded z-50"
            style={{
              top: dropdownProfile.top,
              left: dropdownProfile.left,
            }}
          >
            <ProfileDropdown userInfo={userInfo} onLogout={handleLogout} />
          </div>
        )}
      </div>
    </>
  );
};

export default Navbar;