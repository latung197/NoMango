import { useLocation, useSearchParams, useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { toast } from 'react-toastify';

import emailImg from '@/assets/images/logo_email.png';
import { resendOtp, verifyAccount } from '@/services/auth.service';
import { getToken } from '@/utils/authUtils';

export default function VerifyAccount() {

  const token = getToken();
  if (token) {
      window.location.href = "/";
  }

  const location = useLocation();
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();

  // Lấy email từ state hoặc query
  const [email, setEmail] = useState('');

  useEffect(() => {
    const stateEmail = location.state?.email;
    const queryEmail = searchParams.get('email');

    if (stateEmail) {
      setEmail(stateEmail);
    } else if (queryEmail) {
      setEmail(queryEmail);
    } else {
      toast.error('メールアドレスが見つかりません');
      navigate('/sign-up');
    }
  }, [location.state, searchParams]);

  const [code, setCode] = useState(Array(6).fill(''));

    const handleChange = (value: string, index: number) => {
        if (!/^\d+$/.test(value)) return; // chỉ cho phép số

        const digit = value.slice(-1); // 👈 luôn lấy ký tự cuối cùng
        const newCode = [...code];
        newCode[index] = digit;
        setCode(newCode);

        // Tự động focus sang ô tiếp theo nếu có
        const nextInput = document.getElementById(`otp-${index + 1}`);
        if (digit && nextInput) nextInput.focus();
    };

  const handleSubmit = async () => {
    const otpCode = code.join('');
    if (otpCode.length < 6) {
      toast.error('6桁の認証コードを入力してください');
      return;
    }
    const result = await verifyAccount({ email, otpCode });
    toast.success('認証が完了しました！');
    navigate('/login');
  };

    const handleResend = async () => {
        const result = await resendOtp({ email });
        toast.success('認証コードを再送信しました');
    };

    const handlePaste = (e: React.ClipboardEvent<HTMLInputElement>) => {
        e.preventDefault();
        const pastedData = e.clipboardData.getData("Text").trim();

        if (!/^\d+$/.test(pastedData)) return;

        const digits = pastedData.slice(0, code.length).split("");

        setCode((prev) =>
            prev.map((_, i) => digits[i] ?? "")
        );
    };

  return (
    <div className="max-w-xl mx-auto mt-10 p-6 border rounded shadow">
      <div className="flex justify-center mb-4 mt-16">
        <img src={emailImg} alt="email" className="w-25 h-" />
      </div>

      <h2 className="text-xl font-bold text-center mb-2">アカウント認証</h2>
      <p className="text-sm text-gray-600 text-center mb-6">
        <span className="font-semibold">{email}</span> に送信された認証コードを入力してください。
      </p>

      <div className="flex justify-center gap-2 mb-4">
        {code.map((digit, index) => (
          <input
            key={index}
            id={`otp-${index}`}
            type="text"
            maxLength={1}
            value={digit}
            onChange={(e) => handleChange(e.target.value, index)}
            onPaste={handlePaste}
            onFocus={(e) => e.target.select()}
            className="w-10 h-10 text-center border rounded text-lg"
          />
        ))}
      </div>

      <p className="text-sm text-center text-gray-600 mb-4">
        コードが届いていませんか？{' '}
            <button
                onClick={handleResend}
                className="text-blue-600 hover:underline"
            >
                再送信
            </button>

      </p>

      <button
        onClick={handleSubmit}
        className="w-full bg-[#009EE2] text-white py-2 rounded hover:bg-[#007AC0] font-semibold"
      >
        確認する
      </button>

      <p className="text-sm text-center text-gray-500 mt-6">
        すでにアカウントをお持ちですか？{' '}
        <a href="/login" className="text-blue-600 hover:underline">
          ログイン
        </a>
      </p>
    </div>
  );
}

