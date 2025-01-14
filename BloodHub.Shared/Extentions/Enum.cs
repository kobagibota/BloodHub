namespace BloodHub.Shared.Extentions
{
    public enum ChangeType
    {
        Update,
        Delete
    }

    public enum TokenType
    {
        AccessToken,
        RefreshToken,
        SessionToken
    }

    public enum Activity
    {
        Login,              // Đăng nhập
        Logout,             // Đăng xuất
        Generate,           // Sinh token
        Rotate,             // Cập nhật token (Refresh token)
        Revoke,             // Thu hồi token
        ChangePassword,     // Đổi mật khẩu
        InvalidToken        // Sử dụng token không hợp lệ
    }

    public enum TransactionType
    {
        Import,    // Nhập  
        Export,    // Xuất  
        Refund,    // Hoàn trả  
        Break      // Xuất huỷ 
    }

    public enum TransactionStatus
    {
        Pending,    // Đang chờ xử lý
        Completed,  // Hoàn tất
        Cancelled   // Đã hủy
    }

    public enum ShiftStatus
    {
        Pending,        // Đang nhận bàn giao ca trực
        InProgress,     // Đang trực  
        Transferred,    // Đã bàn giao
        Completed       // Hoàn thành ca trực
    }

    public enum Gender      // Giới tính
    {
        Female,
        Male,
        Other
    }

    public enum BloodGroup  // Nhóm máu hệ ABO
    {
        A,
        B,
        AB,
        O
    }

    public enum Rhesus      // Nhóm máu hệ Rhesus
    {
        Positive, 
        Negative
    }

    public enum InventoryStatus      // Trạng thái tồn kho của sản phẩm 
    {
        Available,      // Máu sẵn sàng để sử dụng (đơn vị máu có thể xuất kho).
        InTest,         // Đang trong quá trình xét nghiệm (đánh giá thuận hợp).
        Issued,         // Đã được xuất kho và giao cho bệnh nhân.
        Returned,       // Đã xuất nhưng hoàn trả lại kho.
        Discarded       // Bị hủy bỏ (do hết hạn, hỏng hoặc không sử dụng được).
    }

    public enum TestResult      // Kết quả xét nghiệm (các ống)
    {
        Positive,
        Negative
    }

    public enum CrossmatchResult      // Nhóm máu hệ Rhesus
    {
        Compatible,     // tương thích
        Incompatible,   // không tương thích
        Indeterminate   // Không xác định
    }
}