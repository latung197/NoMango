    // src/features/auth/pages/LoginPage.tsx

import LoginForm from '../components/LoginForm'

const LoginPage = () => {
  return (
    <div className="flex min-h-screen items-center justify-center px-4">
      <div className="w-full max-w-md">
        <div className="rounded-2xl bg-white p-8 shadow-xl">
          {/* Header */}
          <div className="mb-8 text-center">
            <div className="mb-4 text-3xl font-bold text-emerald-600">
              Logo
            </div>

            <h1 className="text-2xl font-bold text-slate-800">
              Đăng nhập
            </h1>

            <p className="mt-2 text-sm text-slate-500">
              Đăng nhập vào tài khoản của bạn
            </p>
          </div>

          {/* Form */}
          <LoginForm />
        </div>
      </div>
    </div>
  )
}

export default LoginPage