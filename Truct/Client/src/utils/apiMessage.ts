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
    status !== 9999;

  return isValidStatus ? message ?? 'エラーメッセージがありません' : '予期しないエラーが発生しました。';
};
