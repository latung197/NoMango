import React from 'react';

type InputFieldProps = {
  label: string;
  name: string;
  type?: string;
  required?: boolean;
  value?: string;
  onChange?: (e: React.ChangeEvent<HTMLInputElement>) => void;
  error?: string;
  maxLength?: number;
};

const InputField: React.FC<InputFieldProps> = ({
  label,
  name,
  type = 'text',
  required = false,
  value,
  onChange,
  error,
  maxLength
}) => {
  return (
    <div className="relative">
      <label className="block text-sm font-medium text-gray-700 text-left">
        {label}
        {required && <span className="text-red-500"> *</span>}
      </label>

      <input
        type={type}
        name={name}
        value={value}
        onChange={onChange}
        maxLength={maxLength}
        className={`w-full border ${
          error ? 'border-red-500' : 'border-gray-300'
        } rounded px-4 py-2 mt-2 focus:outline-none focus:ring-1 ${
          error ? 'focus:ring-red-500' : 'focus:ring-[#007AC0]'
        } transition`}
        required={required}
      />

        {error && (
            <p className="absolute text-red-500 text-[10px] mt-1 top-full left-0">
                {error}
            </p>
        )}
    </div>
  );
};

export default InputField;