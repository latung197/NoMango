
interface LoginResponse {
  name: string;
  accessToken: string;
  refreshToken: string;
  message: String; 
  tokenType: string;
  accessTokenExpiresIn: string;
  refreshTokenExpiresIn: string
}