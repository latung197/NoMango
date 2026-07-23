import { useState, useEffect, useRef } from "react";

import userProfileIcon from '@/assets/icons/user_profile.png';
import quoteHistoryIcon from '@/assets/icons/quote_history.png';
import settingIcon from '@/assets/icons/settings.png';
import logoutIcon from '@/assets/icons/logout.png';
import { getToken } from "@/utils/authUtils";
import { useNavigate } from "react-router-dom";

interface UserInfo {
  name: string;
  email: string;
  avatar?: string;
}

interface ProfileDropdownProps {
  userInfo: UserInfo;
  onLogout: () => void;
}

export default function ProfileDropdown({ userInfo, onLogout }: ProfileDropdownProps) {
    const navigate = useNavigate();


  return (
    <div className="relative">
      <div className="absolute right-0 mt-2 w-48 bg-white rounded shadow-xl z-50 border border-gray-300">
        <div className="bg-white p-4 rounded-t text-black text-left">
          <div className="text-base font-bold">{userInfo.name}</div>
          <div className="text-sm text-gray-500 border-b border-gray-300 pb-2">{userInfo.email}</div>
        </div>
        <ul className="text-sm text-gray-700">
          <li className="px-4 py-1 hover:bg-gray-100 cursor-pointer flex items-center gap-1">
            <img src={userProfileIcon} className="h-4 w-4 align-middle" />
            <i className="fas fa-user"/>
          </li>
          <li className="px-4 py-1 hover:bg-gray-100 cursor-pointer flex items-center gap-1">
            <img src={settingIcon} className="h-4 w-4 align-middle" />
            <i className="fas fa-cog" /> 
          </li>
          <li
            className="px-4 py-1 hover:bg-gray-100 cursor-pointer flex items-center gap-1"
            onClick={onLogout}
          >
            <img src={logoutIcon} className="h-4 w-4 align-middle" />
            <i className="fas fa-sign-out-alt" /> ログアウト
          </li>
        </ul>
      </div>
    </div>
  );
}