
export const validatePassword = (password: string): string | null => {
  if (!password) return '値を入力してください'; // Required

  if (password.length < 6 || password.length > 20) {
    return '6〜20文字で入力してください'; // Length
  }

  if (!/[A-Z]/.test(password)) {
    return '英大文字を1文字以上含めてください'; // At least one uppercase
  }

  if (!/[!@#$%^&*(),.?":{}|<>]/.test(password)) {
    return '記号を1文字以上含めてください'; // At least one special character
  }

  return null;
};