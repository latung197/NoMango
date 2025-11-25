import { Suspense } from "react";
import { Route, Routes, Outlet } from "react-router-dom";
import { routes } from "./routes";
import Navbar from "../components/layout/Navbar";
import Footer from "../components/layout/Footer";

const Layout = () => {
  return (
      <div className="flex flex-col min-h-screen">
        <Navbar />
        <main className="flex-grow container mx-auto px-4 py-6 pt-16 mt-5 max-w-[100%]">
          <Outlet />
        </main>
        <Footer />
      </div>
  );
};

const AppRouterProvider = () => {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        {routes.map((route) => {
          const { page, path } = route;
          const Element = page;
          return (
            <Route
              key={path}
              path={path}
              element={
                <Suspense fallback={<>Loading ...</>}>
                  <Element />
                </Suspense>
              }
            />
          );
        })}
      </Route>
    </Routes>
  );
};

export default AppRouterProvider;
