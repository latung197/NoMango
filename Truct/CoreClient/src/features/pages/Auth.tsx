import { FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import { signUp } from "@/services/auth.service";
import { setToken, setUserInfoUtils } from "@/utils/authUtils";

type AuthMode = "login" | "signUp";

const Auth = () => {
  const navigate = useNavigate();
  const [mode, setMode] = useState<AuthMode>("login");
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [submitting, setSubmitting] = useState(false);

  const isLogin = mode === "login";

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError("");

    if (!isLogin && password !== confirmPassword) {
      setError("Mật khẩu xác nhận không khớp.");
      return;
    }

    setSubmitting(true);
    try {
      if (isLogin) {
        // Mock login for local testing: accept any credentials
        const fakeToken = "mock-token"
        const expiresAt = new Date(Date.now() + 1000 * 60 * 60).toISOString() // 1 hour

        setToken(fakeToken, expiresAt)
        setUserInfoUtils({ name: "Test User", email })
        navigate("/home")
        return
      }

      await signUp({ name, email, password });
      setMode("login");
      setConfirmPassword("");
      setError("Tạo tài khoản thành công. Hãy đăng nhập.");
    } catch {
      setError(isLogin ? "Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin." : "Không thể tạo tài khoản.");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="min-w-screen">
    <main className="flex min-h-screen items-center justify-center bg-slate-100 px-4 pt-2">
      <section className="w-full max-w-md rounded-xl bg-white p-8 text-left shadow-lg">
        <h1 className="mb-2 text-2xl font-bold text-slate-900">
          {isLogin ? "Đăng nhập" : "Tạo tài khoản"}
        </h1>
        <p className="mb-6 text-sm text-slate-500">
          {isLogin ? "Đăng nhập để tiếp tục sử dụng tài khoản." : "Tạo tài khoản mới để bắt đầu."}
        </p>

        <form className="space-y-4" onSubmit={handleSubmit}>
          {!isLogin && (
            <label className="block text-sm font-medium text-slate-700">
              Họ và tên
              <input
                required
                value={name}
                onChange={(event) => setName(event.target.value)}
                className="mt-1 w-full rounded border border-slate-300 px-3 py-2 font-normal outline-none focus:border-green-500"
              />
            </label>
          )}

          <label className="block text-sm font-medium text-slate-700">
            Email
            <input
              required
              type="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              className="mt-1 w-full rounded border border-slate-300 px-3 py-2 font-normal outline-none focus:border-green-500"
            />
          </label>

          <label className="block text-sm font-medium text-slate-700">
            Mật khẩu
            <input
              required
              minLength={6}
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              className="mt-1 w-full rounded border border-slate-300 px-3 py-2 font-normal outline-none focus:border-green-500"
            />
          </label>

          {!isLogin && (
            <label className="block text-sm font-medium text-slate-700">
              Xác nhận mật khẩu
              <input
                required
                minLength={6}
                type="password"
                value={confirmPassword}
                onChange={(event) => setConfirmPassword(event.target.value)}
                className="mt-1 w-full rounded border border-slate-300 px-3 py-2 font-normal outline-none focus:border-green-500"
              />
            </label>
          )}

          {error && <p className="text-sm text-red-600">{error}</p>}

          <button
            type="submit"
            disabled={submitting}
            className="w-full rounded bg-green-600 px-4 py-2 font-semibold text-white hover:bg-green-700 disabled:cursor-not-allowed disabled:opacity-60"
          >
            {submitting ? "Đang xử lý..." : isLogin ? "Đăng nhập" : "Đăng ký"}
          </button>
        </form>

        <button
          type="button"
          onClick={() => {
            setMode(isLogin ? "signUp" : "login");
            setError("");
          }}
          className="mt-5 w-full text-sm text-green-700 hover:underline"
        >
          {isLogin ? "Chưa có tài khoản? Đăng ký" : "Đã có tài khoản? Đăng nhập"}
        </button>
      </section>
    </main>
    </div>
  );
};

export default Auth;