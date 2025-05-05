using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces.ITockenService;
using Core.Interfaces.IUserRepository;


namespace Core.UseCases.Authentication
{
    public class Login
    {

            private readonly IUserRepository _userRepository;
            private readonly ITokenService _tokenService;

            public Login(IUserRepository userRepository, ITokenService tokenService)
            {
                _userRepository = userRepository;
                _tokenService = tokenService;
            }

            public async Task<string> ExecuteAsync(string username, string password)
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                if (user == null || !VerifyPassword(password, user.PasswordHash))
                    throw new UnauthorizedAccessException("Invalid credentials.");

                return _tokenService.GenerateToken(user);
            }

            private bool VerifyPassword(string password, string passwordHash)
            {
                // Giả sử sử dụng BCrypt hoặc một thư viện mã hóa khác
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
        }
}
