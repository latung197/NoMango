import { useLocation, useSearchParams, useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { toast } from 'react-toastify';
import { resetPassword } from '@/services/auth.service';
import InputField from '@/components/shared/InputField';
import { validatePassword } from '@/utils/validators/passwordValidator';
import { t } from 'i18next';
import { getToken } from '@/utils/authUtils';

export default function ResetPassword() {
    if (getToken()) {
        window.location.href = "/";
    }

    const location = useLocation();
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [token, setToken] = useState('');
    const [form, setForm] = useState({
        password: '',
        confirmPassword: '',
    });
    const [errors, setErrors] = useState<{ [key: string]: string | undefined }>({});

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
         if (errors[name as keyof typeof errors]) {
            setErrors((prev) => ({ ...prev, [name]: undefined }));
        }
    };

    const validate = () => {
        const newErrors: { [key: string]: string } = {};
        const passwordError = validatePassword(form.password);
        if (passwordError) newErrors.password = passwordError;
        if (!form.confirmPassword) newErrors.confirmPassword = t('message.validate.required_input');
        if (
            form.password &&
            form.confirmPassword &&
            form.password !== form.confirmPassword
        ) {
            newErrors.confirmPassword = t('message.validate.password_mismatch');
        }
        
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if (!validate()) return;

        try {
            setLoading(true);
            const data = await resetPassword({ token, password:  form.password});
            toast.success('パスワードが正常にリセットされました');
            navigate('/login');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        const queryToken = searchParams.get('token');
        if (queryToken) {
            setToken(queryToken);
        } else {
            navigate('/login');
        }
    }, [location.state, searchParams]);

  return (
    <div className="max-w-xl mx-auto mt-10 p-6 border rounded shadow">
        <h2 className="text-xl font-bold text-center mb-4">パスワードをリセットする</h2>

        <form onSubmit={handleSubmit} className="space-y-6">
            <InputField
                label="新しいパスワード"
                name="password"
                type="password"
                required
                value={form.password}
                onChange={handleChange}
                error={errors.password}
                maxLength={20}
            />

            <InputField
            label="パスワードの確認"
            name="confirmPassword"
            type="password"
            required
            value={form.confirmPassword}
            onChange={handleChange}
            error={errors.confirmPassword}
            maxLength={20}
            
            />

            <div className="mt-8 text-center">
                <button
                    type="submit"
                    disabled={loading}
                    className="bg-[#009EE2] text-white px-6 py-3 rounded hover:bg-[#007AC0] font-semibold"
                >
                    確認する
                </button>
            </div>
        </form>
    </div>
  );
}

