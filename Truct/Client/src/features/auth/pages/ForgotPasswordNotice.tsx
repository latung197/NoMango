import { useLocation, useSearchParams, useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { toast } from 'react-toastify';

import emailImg from '@/assets/images/logo_email.png';
import { forgotPassword } from '@/services/auth.service';
import { getToken } from '@/utils/authUtils';

export default function ForgotPasswordNotice() {
  const token = getToken();
  if (token) {
      window.location.href = "/";
  }

  const location = useLocation();
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [minutes, setMinutes] = useState('');

  useEffect(() => {
    const stateEmail = location.state?.email;
    const queryEmail = searchParams.get('email');
    const expireMinutes = location.state?.minutes;
    if (expireMinutes) {
      setMinutes(expireMinutes);
    }
    if (stateEmail) {
      setEmail(stateEmail);
    } else if (queryEmail) {
      setEmail(queryEmail);
    } else {
      toast.error('メールアドレスが見つかりません');
      navigate('/login');
    }
  }, [location.state, searchParams]);

  const handleResend = async () => {
      const data = await forgotPassword({ email });
      toast.success('パスワード再設定のリクエストを送信しました');
      setMinutes(data.data.expireMinutes);
  };

  return (
    <div className="max-w-xl mx-auto mt-10 p-6 border rounded shadow">
        <div className="flex justify-center mb-4 mt-16">
            <img src={emailImg} alt="email" className="w-25 h-" />
        </div>

        <h2 className="text-xl font-bold text-center mb-2">メールをご確認ください</h2>
        <p className="text-sm text-gray-600 text-center mb-6">
             パスワードをリセットするための手順が記載されたメールを送信しました。
        </p>

        <p className="text-sm text-gray-600 text-center mb-6">
            <span className="font-bold">{minutes}</span> 分以内にメールが届かない場合は、迷惑メールフォルダをご確認の上、{' '}
            <span
              onClick={handleResend}
              className="text-[#009EE2] hover:underline cursor-pointer font-medium"
            >
              再送信
            </span>
            を行うか、{' '}
            <span
               onClick={() => navigate('/forgot-password')}
              className="text-[#009EE2] hover:underline cursor-pointer font-medium"
            >
              別のメールアドレスを試してください
            </span>.
        </p>
        <p className="text-sm text-center text-gray-600 mb-4">
            <span
                onClick={() => navigate('/login')}
              className="text-[#009EE2] hover:underline cursor-pointer font-medium"
            >
                サインイン画面に戻る
            </span>
        </p>
    </div>
  );
}

