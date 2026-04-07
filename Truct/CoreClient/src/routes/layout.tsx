import { Suspense } from 'react'
import { Route, Routes, Outlet } from 'react-router-dom'
import { routes } from './routes'
import Navbar from '../components/layout/Navbar'
import Footer from '../components/layout/Footer'

// Layout chỉ có 3 phần: Navbar, Outlet, Footer
const Layout = () => {
  return (
    <div className="flex min-h-screen flex-col">
      <Navbar />
      <Outlet />
      <Footer />
    </div>
  )
}
const AppRouterProvider = () => {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        {routes.map((route) => {
          const { page, path } = route
          const Element = page
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
          )
        })}
      </Route>
    </Routes>
  )
}

export default AppRouterProvider
