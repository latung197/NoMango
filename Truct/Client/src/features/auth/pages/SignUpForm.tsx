import React, { useState } from 'react';
import { genderOptions, getDefaultGender } from '@/constants/genderOptions';
import InputField from '@/components/shared/InputField';
import { useTranslation } from 'react-i18next';
import { validatePassword } from '@/utils/validators/passwordValidator';
import { signUp } from '@/services/auth.service';
import { useNavigate } from "react-router-dom";
import { getToken } from '@/utils/authUtils';

const SignUpForm = () => {
    const token = getToken();
    if (token) {
        window.location.href = "/";
    }

    const navigate = useNavigate();
    const {t, i18n} = useTranslation();
    const [form, setForm] = useState({
        name: '',
        password: '',
        gender: getDefaultGender(),
        company: '',
        address: '',
        email: '',
        confirmPassword: '',
        phoneNumber: '',
        position: '',
        postalCode: '',
    });

    const [errors, setErrors] = useState<{ [key: string]: string }>({});

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
        setErrors((prevErrors) => {
            const newErrors = { ...prevErrors };
            if (newErrors[name] && value.trim() !== '') {
                delete newErrors[name];
            }
            if (name === 'password') {
                const passwordError = validatePassword(value);
                if (passwordError) {
                    newErrors.password = passwordError;
                } else {
                    delete newErrors.password;
                }
                if (form.confirmPassword && value !== form.confirmPassword) {
                    newErrors.confirmPassword = t('message.validate.required_input');
                } else {
                    delete newErrors.confirmPassword;
                }
            }
            if (name === 'confirmPassword' && form.password && value !== form.password) {
                newErrors.confirmPassword = t('message.validate.required_input');
            } else if (name === 'confirmPassword') {
                delete newErrors.confirmPassword;
            }
            return newErrors;
        });
    };

    const validate = () => {
        const newErrors: { [key: string]: string } = {};
        if (!form.name) newErrors.name = t('message.validate.required_input');
        if (!form.email) newErrors.email = t('message.validate.required_input');
        if (!form.confirmPassword) newErrors.confirmPassword = t('message.validate.required_input');
        // if (!form.gender) newErrors.gender = t('message.validate.required_select');
        if (!form.phoneNumber) newErrors.phoneNumber = t('message.validate.required_input');
        if (!form.company) newErrors.company = t('message.validate.required_input');
        if (!form.address) newErrors.address = t('message.validate.required_input');
        // if (!form.postalCode) newErrors.postalCode = t('message.validate.required_input');

        const passwordError = validatePassword(form.password);
        if (passwordError) newErrors.password = passwordError;

        if (
            form.password &&
            form.confirmPassword &&
            form.password !== form.confirmPassword
        ) {
            newErrors.confirmPassword = t('message.validate.password_mismatch');
        }
        return newErrors;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const newErrors = validate();
        if (Object.keys(newErrors).length > 0) {
            setErrors(newErrors);
            return;
        }
        const result = await signUp(form);
        // ✅ Chuyển sang màn khác
        navigate('/sign-up/verify', { state: { email: form.email } });
    };

    return (
        <div className="max-w-screen-xl mx-auto p-8">
            <h2 className="text-2xl font-bold text-[#009EE2] mb-6">アカウントを作成</h2>
            <form className="space-y-6">
            {/* Hàng 1 */}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <InputField
                        label="氏名"
                        name="name"
                        required
                        value={form.name}
                        onChange={handleChange}
                        error={errors.name}
                        maxLength={50}
                    />
                    <InputField
                        label="メールアドレス"
                        name="email"
                        required
                        value={form.email}
                        onChange={handleChange}
                        error={errors.email}
                        maxLength={100}
                    />
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <InputField
                        label="パスワード"
                        name="password"
                        type='password'
                        required
                        value={form.password}
                        onChange={handleChange}
                        error={errors.password}
                        maxLength={20}
                    />
                    <InputField
                        label="パスワード（確認)"
                        name="confirmPassword"
                        type='password'
                        required
                        value={form.confirmPassword}
                        onChange={handleChange}
                        error={errors.confirmPassword}
                        maxLength={20}
                    />
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <div>
                        <div className="relative">
                            <label className="block text-sm font-medium text-gray-700 text-left">
                                性別 <span className="text-red-500">*</span>
                            </label>

                            <select
                                name="gender"
                                value={form.gender}
                                onChange={handleChange}
                                className={`w-full border rounded px-4 py-2 mt-2 appearance-none focus:ring-[#007AC0] transition`}
                                >
                                {genderOptions.map((option) => (
                                    <option key={option.value} value={option.value}>
                                    {option.label}
                                    </option>
                                ))}
                            </select>
                        </div>
                    </div>
                    <InputField
                        label="携帯番号"
                        name="phoneNumber"
                        required
                        value={form.phoneNumber}
                        onChange={handleChange}
                        error={errors.phoneNumber}
                        maxLength={20}
                    />
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <InputField
                        label="会社名"
                        name="company"
                        required
                        value={form.company}
                        onChange={handleChange}
                        error={errors.company}
                        maxLength={255}
                    />
                    <InputField
                        label="役職"
                        name="position"
                        value={form.position}
                        onChange={handleChange}
                        error={errors.position}
                        maxLength={100}
                    />
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <InputField
                        label="郵便番号"
                        name="postalCode"
                        value={form.postalCode}
                        onChange={handleChange}
                        error={errors.postalCode}
                        maxLength={20}
                    />
                    <InputField
                        label="住所"
                        name="address"
                        required
                        value={form.address}
                        onChange={handleChange}
                        error={errors.address}
                        maxLength={255}
                    />
                </div>
            </form>
            <form onSubmit={handleSubmit}>
                <div className="mt-8">
                    <button
                    type="submit"
                    className="bg-[#009EE2] text-white px-6 py-3 rounded hover:bg-[#007AC0] font-semibold"
                    >
                    アカウントを作成
                    </button>
                </div>
            </form>


            <p className="text-center text-sm text-gray-600 mt-10">
                すでにアカウントをお持ちですか？{' '}
                <a href="/login" className="text-[#009EE2] font-medium hover:underline">
                ログイン
                </a>
            </p>
        </div>
    );
};

export default SignUpForm;