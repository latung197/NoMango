import Home from "../features/pages/Home";
import NotFoundComponent from "../components/errors/NotFound.component";
import { t } from "i18next";

export const routes = [
  {
    path: "/",
    page: Home,
    getTitle: () => t("title.home"),
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
