import { useEffect } from "react";
import i18next from "i18next";
import { useLocation } from "react-router-dom";
import { routes } from "../../routes/routes";

export const DynamicTitle = () => {
  const location = useLocation();

  const updateTitle = () => {
    const matchedRoute = routes.find((r) =>
      location.pathname.match(new RegExp(`^${r.path.replace(/:\w+/g, "\\w+")}$`))
    );

    const title = matchedRoute?.getTitle?.() || "ページが見つかりません | アルミ安全冊見積もり";
    document.title = title;
  };

  useEffect(() => {
    updateTitle();

    const onLangChange = () => updateTitle();
    i18next.on("languageChanged", onLangChange);

    return () => {
      i18next.off("languageChanged", onLangChange);
    };
  }, [location.pathname]);

  return null;
};
