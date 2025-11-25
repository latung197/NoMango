import Home from "../features/home/pages/Home";
import NotFoundComponent from "../components/errors/NotFound.component";
import LoginPage from "@/features/auth/pages/LoginPage"

import { t } from "i18next";
import SignUpForm from "@/features/auth/pages/SignUpForm";
import VerifyAccount from "@/features/auth/pages/SignUpVerifyAccount";
import ForgotPassword from "@/features/auth/pages/ForgotPassword";
import ForgotPasswordNotice from "@/features/auth/pages/ForgotPasswordNotice";
import ResetPassword from "@/features/auth/pages/ResetPassword";
import Search from "@/features/search/pages/SearchText";
import SearchByCategory from "@/features/search/pages/SearchByCategory"
import Wishlist from "@/features/wishlist/pages/Wishlist"
import ProductDetail from "@/features/product/pages/ProductDetail";
import Cart from "@/features/cart/pages/Cart";
import QuotationHistory from "@/features/quotationHistory/pages/QuotationHistory";

export const routes = [
  {
    path: "/",
    page: Home,
    getTitle: () => t("title.home"),
  },
  {
    path: "/search",
    page: Search,
    getTitle: () => t("title.search"),
  },
  {
    path: "/search-by-category",
    page: SearchByCategory,
    getTitle: () => t("title.search"),
  },
  {
    path: "/product/:id",
    page: ProductDetail,
    getTitle: () => t("title.product"),
  },
  {
    path: "/login",
    page: LoginPage,
    getTitle: () => t("title.login"),
  },
    {
    path: "/sign-up",
    page: SignUpForm,
    getTitle: () => t("title.sign_up"),
  },
  {
    path: "/sign-up/verify",
    page: VerifyAccount,
    getTitle: () => "アルミ安全冊見積もり | アカウント認証",
  },
  {
    path: "/forgot-password",
    page: ForgotPassword,
    getTitle: () => "アルミ安全冊見積もり | パスワードを忘れた場合",
  },
  {
    path: "/forgot-password/notice",
    page: ForgotPasswordNotice,
    getTitle: () => "アルミ安全冊見積もり | パスワードを忘れた場合",
  },
  {
    path: "/forgot-password/reset",
    page: ResetPassword,
    getTitle: () => "アルミ安全冊見積もり | パスワードを忘れた場合",
  },
  {
    path: "/user/wishlist",
    page: Wishlist,
    getTitle: () => "アルミ安全冊見積もり | お気に入り商品一覧",
  },
  {
    path: "/user/cart",
    page: Cart,
    getTitle: () => "アルミ安全冊見積もり | カート",
  },
  {
    path: "/user/quotation/history",
    page: QuotationHistory,
    getTitle: () => "アルミ安全冊見積もり | 見積履歴",
  },
  {
    path: "/*",
    page: NotFoundComponent,
    getTitle: () => "アルミ安全冊見積もり | ページが見つかりません",
  },
  {
    path: "/not-found",
    page: NotFoundComponent,
    getTitle: () => "アルミ安全冊見積もり | ページが見つかりません",
  }
];
