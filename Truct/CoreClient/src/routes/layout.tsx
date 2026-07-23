import { Suspense } from 'react'
import { Route, Routes, Outlet } from 'react-router-dom'
import { routes } from './routes'
import Navbar from '../components/layout/navbar/Navbar'
<<<<<<< HEAD
import Footer from '../components/layout/Footer'
=======
import Footer from '../components/layout/footer/Footer'
>>>>>>> 16b0b8095612890ee1878c16d3c404e090a1ad5e

// Layout chỉ có 3 phần: Navbar, Outlet, Footer
const Layout = () => {
  return (
    <div className="flex min-h-screen flex-col">
      <Navbar />
      <main className="pt-16">
        <Outlet />
      </main>
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
