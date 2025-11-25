import { useState, useEffect } from "react";
import { ensureUserInfo } from "@/utils/authUtils";

export function useUserInfo() {
  const [userInfo, setUserInfo] = useState<any>(null);

  useEffect(() => {
    const fetchUser = async () => {
      const user = await ensureUserInfo();
      setUserInfo(user);
    };
    fetchUser();
  }, []);

  return { userInfo, setUserInfo };
}