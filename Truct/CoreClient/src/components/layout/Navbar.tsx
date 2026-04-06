import React from 'react';
import { useState, useEffect, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate } from "react-router-dom";
import { useAutocompleteSearch } from './navbar/AutoCompleteSearch';
import CategoryDropdown from './navbar/CategoryDropdown';
import ProductSearchDropdown from './navbar/ProductSearchDropdown';
import HoverMenu from './navbar/HoverMenu';
import { ensureUserInfo, getToken, logout } from '@/utils/authUtils';
import ProfileDropdown from './navbar/ProfileDropdown';


const Navbar: React.FC = () => {

  const {t, i18n} = useTranslation();
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


  const toggleCategory = () => {
    setShowCategory(prev => !prev);
  };


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
      </div>
    </>
  );
};

export default Navbar;