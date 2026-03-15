import { getUserInfo } from "@/services/user.service";

const accessToken = "accessToken";
const accessTokenExpiresIn = "accessTokenExpiresIn";
const userInfo = "userInfo";

export function isTokenValid(): boolean {
  const token = localStorage.getItem(accessToken);
  const expiresAt = localStorage.getItem(accessTokenExpiresIn);

  if (!token || !expiresAt) return false;

  const expiryDate = new Date(expiresAt);
  return expiryDate > new Date();
}

export function logout() {
  localStorage.removeItem(accessToken);
  localStorage.removeItem(accessTokenExpiresIn);
  sessionStorage.removeItem(userInfo);
}

export function setToken(token: string, expiresAt: string) {
  const expiresDate = new Date(expiresAt);
  expiresDate.setHours(expiresDate.getHours() - 1);
  const adjustedExpiresAt = expiresDate.toISOString();

  localStorage.setItem(accessToken, token);
  localStorage.setItem(accessTokenExpiresIn, adjustedExpiresAt);
}

export function getToken(): string | null {
  const token = localStorage.getItem(accessToken);
  const expiresAt = localStorage.getItem(accessTokenExpiresIn);
  if (!token || !expiresAt) return null;

  const expiryDate = new Date(expiresAt);
  const now = new Date();

  if (expiryDate <= now) {
    logout();
    return null;
  }

  return token;
}

export async function ensureUserInfo(): Promise<object | null> {
  const token = getToken();
  if (!token) return null;
  const cached = sessionStorage.getItem(userInfo);
  if (cached && cached !== "undefined") {
    return JSON.parse(cached);
  }
  try {
    const user = await getUserInfo();
    sessionStorage.setItem(userInfo, JSON.stringify(user));
    return user;
  } catch (error) {
    return null;
  }
}

export function setUserInfoUtils(user: any) {
  if (!user) return;
  sessionStorage.setItem(userInfo, JSON.stringify(user));
}

