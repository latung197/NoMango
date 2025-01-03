using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.ITockenService
{
    public interface ITokenService
    {
        /// <summary>
        /// Tạo token cho người dùng.
        /// </summary>
        /// <param name="user">Thông tin người dùng.</param>
        /// <returns>Chuỗi token đã được tạo.</returns>
        string GenerateToken(User user);

        /// <summary>
        /// Xác thực token và trả về kết quả.
        /// </summary>
        /// <param name="token">Chuỗi token cần xác thực.</param>
        /// <returns>True nếu token hợp lệ, ngược lại là false.</returns>
        bool ValidateToken(string token);

        /// <summary>
        /// Lấy thông tin người dùng từ token.
        /// </summary>
        /// <param name="token">Chuỗi token cần phân tích.</param>
        /// <returns>Đối tượng User chứa thông tin từ token, hoặc null nếu token không hợp lệ.</returns>
        User GetUserFromToken(string token);
    }
}
