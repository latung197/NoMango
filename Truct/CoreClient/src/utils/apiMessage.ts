export const getApiMessage = (error: {
  response?: {
    data?: {
      status?: number;
      message?: string;
    };
  };
}): string => {
  const status = error.response?.data?.status;
  const message = error.response?.data?.message;

  const isValidStatus =
    typeof status === 'number' &&
    status.toString().length === 4 &&
    status !== 5000;

  return isValidStatus ? message ?? 'Không có thông báo lỗi' : 'Đã xảy ra lỗi không mong muốn.';
};