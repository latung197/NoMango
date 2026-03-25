
interface LoginResponse {
  name: string;
  accessToken: string;
  refreshToken: string;
  message: string; 
  tokenType: string;
  accessTokenExpiresIn: string;
  refreshTokenExpiresIn: string
}