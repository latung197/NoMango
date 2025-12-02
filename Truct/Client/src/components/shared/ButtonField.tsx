import React from 'react';

export type ButtonVariant = 
  | 'primary' 
  | 'secondary' 
  | 'success' 
  | 'danger' 
  | 'warning' 
  | 'info'
  | 'outline-primary'
  | 'outline-secondary'
  | 'outline-success'
  | 'outline-danger'
  | 'outline-warning'
  | 'outline-info'
  | 'ghost'
  | 'link';

export type ButtonSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl';
export type ButtonType = 'button' | 'submit' | 'reset';

export interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  /** Nội dung hiển thị trên button */
  children: React.ReactNode;
  /** Kiểu hiển thị (màu sắc) */
  variant?: ButtonVariant;
  /** Kích thước */
  size?: ButtonSize;
  /** Button chiếm toàn bộ chiều rộng */
  fullWidth?: boolean;
  /** Hiển thị loading */
  isLoading?: boolean;
  /** Icon bên trái */
  leftIcon?: React.ReactNode;
  /** Icon bên phải */
  rightIcon?: React.ReactNode;
  /** Chỉ hiển thị icon */
  iconOnly?: boolean;
  /** Hiệu ứng rounded pill */
  rounded?: 'none' | 'sm' | 'md' | 'lg' | 'full';
}

const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(({
  children,
  variant = 'primary',
  size = 'md',
  type = 'button',
  fullWidth = false,
  isLoading = false,
  leftIcon,
  rightIcon,
  iconOnly = false,
  rounded = 'md',
  disabled,
  className = '',
  onClick,
  ...props
}, ref) => {
  // Base classes
  const baseClasses = 'font-medium transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed inline-flex items-center justify-center';
  
  // Rounded classes
  const roundedClasses = {
    none: 'rounded-none',
    sm: 'rounded-sm',
    md: 'rounded',
    lg: 'rounded-lg',
    full: 'rounded-full',
  };
  
  // Size classes
  const sizeClasses = {
    xs: iconOnly ? 'p-1' : 'px-2 py-1 text-xs',
    sm: iconOnly ? 'p-1.5' : 'px-3 py-1.5 text-sm',
    md: iconOnly ? 'p-2' : 'px-4 py-2 text-sm',
    lg: iconOnly ? 'p-2.5' : 'px-5 py-2.5 text-base',
    xl: iconOnly ? 'p-3' : 'px-6 py-3 text-base',
  };
  
  // Variant classes - Tailwind
  const variantClasses: Record<ButtonVariant, string> = {
    // Solid variants
    'primary': 'bg-blue-600 text-white hover:bg-blue-700 focus:ring-blue-500 active:bg-blue-800',
    'secondary': 'bg-gray-600 text-white hover:bg-gray-700 focus:ring-gray-500 active:bg-gray-800',
    'success': 'bg-green-600 text-white hover:bg-green-700 focus:ring-green-500 active:bg-green-800',
    'danger': 'bg-red-600 text-white hover:bg-red-700 focus:ring-red-500 active:bg-red-800',
    'warning': 'bg-yellow-500 text-white hover:bg-yellow-600 focus:ring-yellow-400 active:bg-yellow-700',
    'info': 'bg-cyan-500 text-white hover:bg-cyan-600 focus:ring-cyan-400 active:bg-cyan-700',
    
    // Outline variants
    'outline-primary': 'border border-blue-600 text-blue-600 hover:bg-blue-50 focus:ring-blue-500 active:bg-blue-100',
    'outline-secondary': 'border border-gray-600 text-gray-600 hover:bg-gray-50 focus:ring-gray-500 active:bg-gray-100',
    'outline-success': 'border border-green-600 text-green-600 hover:bg-green-50 focus:ring-green-500 active:bg-green-100',
    'outline-danger': 'border border-red-600 text-red-600 hover:bg-red-50 focus:ring-red-500 active:bg-red-100',
    'outline-warning': 'border border-yellow-500 text-yellow-600 hover:bg-yellow-50 focus:ring-yellow-400 active:bg-yellow-100',
    'outline-info': 'border border-cyan-500 text-cyan-600 hover:bg-cyan-50 focus:ring-cyan-400 active:bg-cyan-100',
    
    // Ghost variants
    'ghost': 'text-gray-700 hover:bg-gray-100 focus:ring-gray-300 active:bg-gray-200',
    'link': 'text-blue-600 hover:text-blue-800 hover:underline focus:ring-blue-300 p-0',
  };

  // Combine all classes
  const buttonClasses = [
    baseClasses,
    roundedClasses[rounded],
    sizeClasses[size],
    variantClasses[variant],
    fullWidth ? 'w-full' : '',
    iconOnly && !isLoading ? 'aspect-square' : '',
    className,
  ]
    .filter(Boolean)
    .join(' ');

  const isDisabled = disabled || isLoading;

  return (
    <button
      ref={ref}
      type={type}
      className={buttonClasses}
      disabled={isDisabled}
      onClick={onClick}
      {...props}
    >
      {isLoading ? (
        <div className="flex items-center">
          <svg 
            className="animate-spin h-4 w-4 mr-2 text-current" 
            xmlns="http://www.w3.org/2000/svg" 
            fill="none" 
            viewBox="0 0 24 24"
          >
            <circle 
              className="opacity-25" 
              cx="12" 
              cy="12" 
              r="10" 
              stroke="currentColor" 
              strokeWidth="4"
            ></circle>
            <path 
              className="opacity-75" 
              fill="currentColor" 
              d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
            ></path>
          </svg>
          {children}
        </div>
      ) : (
        <>
          {leftIcon && !iconOnly && (
            <span className="mr-2">{leftIcon}</span>
          )}
          {iconOnly && leftIcon ? leftIcon : children}
          {rightIcon && !iconOnly && (
            <span className="ml-2">{rightIcon}</span>
          )}
        </>
      )}
    </button>
  );
});

Button.displayName = 'Button';

export default Button;