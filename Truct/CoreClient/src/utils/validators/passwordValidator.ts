 
 export const validatePassword = (password: string): string | null => {
    if (!password) return 'Vui lòng nhập mật khẩu'; // Required

    if (password.length < 6 || password.length > 20) {
      return 'Vui lòng nhập từ 6 đến 20 ký tự'; // Length
    }

    if (!/[A-Z]/.test(password)) {
      return 'Vui lòng chứa ít nhất 1 chữ cái in hoa'; // At least one uppercase
    }

    if (!/[!@#$%^&*(),.?":{}|<>]/.test(password)) {
      return 'Vui lòng chứa ít nhất 1 ký tự đặc biệt'; // At least one special character
    }

    return null;
  };