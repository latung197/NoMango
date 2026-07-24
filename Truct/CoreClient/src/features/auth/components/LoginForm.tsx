// src/features/auth/components/LoginForm.tsx

import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

import { login } from '../services/authService'

const LoginForm = () => {
  const navigate = useNavigate()

  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')

  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (
    event: React.FormEvent<HTMLFormElement>,
  ) => {
    event.preventDefault()

    setError('')
    setLoading(true)

    try {
      const result = await login({
        email,
        password,
      })

      localStorage.setItem(
        'accessToken',
        result.accessToken,
      )

      navigate('/')
    } catch (error) {
      setError('Email hoặc mật khẩu không chính xác')
    } finally {
      setLoading(false)
    }
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="space-y-5"
    >
      {/* Email */}
      <div>
        <label
          htmlFor="email"
          className="mb-1 block text-sm font-medium text-slate-700"
        >
          Email
        </label>

        <input
          id="email"
          type="email"
          value={email}
          onChange={(event) =>
            setEmail(event.target.value)
          }
          placeholder="you@example.com"
          required
          className="
            w-full
            rounded-lg
            border
            border-slate-300
            px-4
            py-2.5
            outline-none
            transition
            focus:border-emerald-500
            focus:ring-2
            focus:ring-emerald-500/20
          "
        />
      </div>

      {/* Password */}
      <div>
        <label
          htmlFor="password"
          className="mb-1 block text-sm font-medium text-slate-700"
        >
          Mật khẩu
        </label>

        <input
          id="password"
          type="password"
          value={password}
          onChange={(event) =>
            setPassword(event.target.value)
          }
          placeholder="••••••••"
          required
          className="
            w-full
            rounded-lg
            border
            border-slate-300
            px-4
            py-2.5
            outline-none
            transition
            focus:border-emerald-500
            focus:ring-2
            focus:ring-emerald-500/20
          "
        />
      </div>

      {/* Error */}
      {error && (
        <div className="rounded-lg bg-red-50 px-4 py-3 text-sm text-red-600">
          {error}
        </div>
      )}

      {/* Submit */}
      <button
        type="submit"
        disabled={loading}
        className="
          w-full
          rounded-lg
          bg-emerald-600
          px-4
          py-2.5
          font-medium
          text-white
          transition
          hover:bg-emerald-700
          disabled:cursor-not-allowed
          disabled:opacity-50
        "
      >
        {loading ? 'Đang đăng nhập...' : 'Đăng nhập'}
      </button>
    </form>
  )
}

export default LoginForm