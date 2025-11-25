import { useState } from 'react';
import { forgotPassword } from '@/services/auth.service';
import { toast } from 'react-toastify';
import { useTranslation } from 'react-i18next';
import { useNavigate } from "react-router-dom";
import { getToken } from '@/utils/authUtils';

export default function ForgotPassword() {
  const token = getToken();
  if (token) {
      window.location.href = "/";
  }

  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [loading, setLoading] = useState(false);
  const {t, i18n} = useTranslation();
  const [errors, setErrors] = useState<{ [key: string]: string }>({});
    

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!email) {
        setErrors({ email: t('message.validate.required_input') });
        return;
    }
    try {
        setLoading(true);
        const data = await forgotPassword({ email });
        toast.success('パスワード再設定メールを送信しました');
        navigate('/forgot-password/notice', { state: { email: email, minutes: data.data.expireMinutes } });
    } finally {
        setLoading(false);
    }
  };

  return (
    <div className="max-w-md mx-auto mt-10 p-6 border rounded shadow">
      <h2 className="text-xl font-bold text-center mb-4">パスワードを忘れた場合</h2>
      <p className="text-sm text-gray-600 text-center mb-6">
        登録済みのメールアドレスを入力してください。再設定リンクを送信します。
      </p>

        <form onSubmit={handleSubmit}>
            <div className="mb-6">
                <input
                    type="email"
                    value={email}
                    onChange={(e) => {
                    setEmail(e.target.value);
                    if (errors.email) {
                        setErrors({});
                    }
                    }}
                    placeholder="メールアドレス"
                    className={`w-full border px-4 py-2 rounded ${
                    errors.email ? 'border-red-500' : 'border-gray-300'
                    } mt-2 focus:outline-none focus:ring-1 ${
                    errors.email ? 'focus:ring-red-500' : 'focus:ring-[#007AC0]'
                    } transition`}
                />
                {errors.email && (
                    <p className="text-red-500 text-[10px] mt-1 text-left">{errors.email}</p>
                )}
            </div>
            <button
                type="submit"
                disabled={loading}
                className="w-full bg-[#009EE2] text-white py-2 rounded hover:bg-[#007AC0]"
                >
                {loading ? '送信中…' : '送信する'}
            </button>
        </form>
    </div>
  );
}