import { useState } from 'react';
import { login } from '@/services/auth.service';

import emailIcon from '@/assets/icons/login-sms.png';
import passwordIcon from '@/assets/icons/login-lock.png';
import { useNavigate } from "react-router-dom";
import { setToken } from '@/utils/authUtils';
import { getUserInfo } from '@/services/user.service';

const LoginForm = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [emailError, setEmailError] = useState(false);
    const [passwordError, setPasswordError] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        let hasError = false;
        if (!email.trim()) {
            setEmailError(true);
            hasError = true;
        } else {
            setEmailError(false);
        }
        if (!password.trim()) {
            setPasswordError(true);
            hasError = true;
        } else {
            setPasswordError(false);
        }

        if (hasError) return;

        try {
            setLoading(true);
            //const data = await login({ email, password });
            //setToken(data.data.accessToken, data.data.accessTokenExpiresIn);
            //const result = await getUserInfo();
            window.location.href = "/";
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="max-w-md mx-auto mt-20 p-8 bg-white shadow-lg rounded-lg">
            <div className="text-center mb-6">
                <h1 className="text-6xl font-bold text-[#009EE2]">Chào mừng</h1>
                <p className="text-gray-500 text-sm">Đăng nhập bằng Email</p>
            </div>

            <form className="flex flex-col gap-4" onSubmit={handleSubmit}>
                <div className="relative mb-6">
                    <span className="absolute top-0 left-3 -translate-y-1/2 bg-white px-1 text-sm text-[#009EE2] font-semibold">
                        Tài Khoản
                    </span>
                    <span className="absolute inset-y-0 left-3 flex items-center text-[#009EE2]">
                        <img src={emailIcon} alt="Địa chỉ Email" className="w-5 h-5 object-contain" />
                    </span>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => {
                            setEmail(e.target.value);
                            if (emailError && e.target.value.trim()) {
                            setEmailError(false);
                            }
                        }}
                        placeholder="Nhập tài khoản"
                        className={`w-full rounded-md pl-10 pr-4 py-3 focus:outline-none focus:ring-2 ${
                            emailError
                            ? 'border border-red-500 focus:ring-red-500'
                            : 'border border-[#009EE2] focus:ring-[#009EE2]'
                        }`}
                    />
                </div>

                <div className="relative mb-">
                    <span className="absolute top-0 left-3 -translate-y-1/2 bg-white px-1 text-sm text-[#009EE2] font-semibold">
                        Mật khẩu
                    </span>
                    <span className="absolute inset-y-0 left-3 flex items-center text-[#009EE2]">
                        <img src={passwordIcon} alt="パスワード" className="w-5 h-5 object-contain" />
                    </span>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => {
                            setPassword(e.target.value);
                            if (password && e.target.value.trim()) {
                                setPasswordError(false);
                            }
                        }}
                        placeholder="Nhập mật khẩu"
                        className={`w-full rounded-md pl-10 pr-4 py-3 focus:outline-none focus:ring-2 ${
                            passwordError
                            ? 'border border-red-500 focus:ring-red-500'
                            : 'border border-[#009EE2] focus:ring-[#009EE2]'
                        }`}
                    />
                </div>
                <div className="text-right mt-0">
                    <button
                        type="button"
                        onClick={() => navigate('/forgot-password')}
                        className="text-sm text-gray-600 hover:underline font-semibold"
                    >
                        Quên mật khẩu
                    </button>
                </div>
                <button
                    type="submit"
                    disabled={loading}
                    className="bg-[#009EE2] text-white mt-2 px-8 py-3 rounded hover:bg-[#007AC0] transition font-semibold"
                >
                    Đăng nhập
                </button>
            </form>
            

            <p className="text-center text-sm text-gray-600 mt-6">
                Nếu bạn không có tài khoản{' '}
                <button
                    onClick={() => navigate('/sign-up')}
                    className="text-[#009EE2] font-medium hover:underline bg-transparent border-none p-0"
                >
                    Đăng ký mới
                </button>
            </p>
        </div>
    );
};

export default LoginForm;