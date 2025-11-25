import InputField from '@/components/shared/InputField';
import { useUserInfo } from '@/hooks/useUserInfo';
import { FormDataRequest } from '@/types/Quotation';
import React from 'react';

interface InfoModalProps {
  visible: boolean;
  loading: boolean;
  onClose: () => void;
  onSubmit: (data: any) => void;
  form?: FormDataRequest;
  errors: Record<string, string>;
  handleChange: (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => void;
}

const InfoModal: React.FC<InfoModalProps> = ({
  visible,
  loading,
  onClose,
  onSubmit,
  form,
  errors,
  handleChange,
}) => {
    const { userInfo, setUserInfo } = useUserInfo();
    if (!visible) return null;
    return (
        <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg p-6 w-[50%] space-y-4">
                <h2 className="text-lg font-medium">連絡先情報を入力してください</h2>
                {!userInfo && (
                    <form className="space-y-6">
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <InputField
                                label="氏名"
                                name="name"
                                required
                                value={form?.name}
                                onChange={handleChange}
                                error={errors.name}
                                maxLength={50}
                            />
                            <InputField
                                label="メールアドレス"
                                name="email"
                                required
                                value={form?.email}
                                onChange={handleChange}
                                error={errors.email}
                                maxLength={100}
                            />
                        </div>
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <InputField
                                label="携帯番号"
                                name="phoneNumber"
                                required
                                value={form?.phoneNumber}
                                onChange={handleChange}
                                error={errors.phoneNumber}
                                maxLength={20}
                            />
                            <InputField
                                label="会社名"
                                name="company"
                                required
                                value={form?.company}
                                onChange={handleChange}
                                error={errors.company}
                                maxLength={255}
                            />
                        </div>
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <InputField
                                label="役職"
                                name="position"
                                value={form?.position}
                                onChange={handleChange}
                                error={errors.position}
                                maxLength={100}
                            />
                            <InputField
                                label="住所"
                                name="address"
                                required
                                value={form?.address}
                                onChange={handleChange}
                                error={errors.address}
                                maxLength={255}
                            />
                        </div>
                    </form>
                )}
                <div>
                    <label className="block text-sm font-medium text-gray-700 text-left">メッセージ</label>
                    <textarea
                        name="message"
                        value={form?.message}
                        onChange={handleChange}
                        className={`mt-1 block w-full border rounded px-3 py-2 text-sm resize-none h-24 focus:outline-none focus:ring-1 ${
                        errors.message ? 'border-red-500 focus:ring-red-500' : 'border-gray-300 focus:ring-blue-500'
                        }`}
                        maxLength={500}
                    />
                        {errors.message && <p className="text-red-500 text-xs mt-1">{errors.message}</p>}
                </div>

                <div className="flex justify-end space-x-2 pt-2">
                    <button onClick={onClose} className="px-4 py-2 border rounded">キャンセル</button>
                    <button
                        onClick={() => onSubmit(form)}
                        disabled={loading}
                        className={`px-4 py-2 rounded text-center ${loading ? 'bg-gray-400 cursor-not-allowed' : 'bg-blue-600 text-white'}`}
                        >
                        {loading ? (
                            <span className="flex items-center gap-2 text-center">
                                <svg className="animate-spin h-4 w-4  text-white" viewBox="0 0 24 24">...</svg>
                                送信中...
                            </span>
                            ) : (
                            '送信'
                            )}
                    </button>

                </div>        
            </div>
        </div>
    );
};

export default InfoModal;