
IF EXISTS(SELECT name FROM sys.databases WHERE name = 'QuanLyRapPhim')
BEGIN
    ALTER DATABASE QuanLyRapPhim SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyRapPhim;
END
GO

CREATE DATABASE QuanLyRapPhim;
GO

USE QuanLyRapPhim;
GO

CREATE TABLE RAP_PHIM (
    IDRap INT IDENTITY(1,1) PRIMARY KEY,
    TenRap NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(200) NOT NULL,
    SoDienThoai VARCHAR(15) NOT NULL,
    Email VARCHAR(100)
);

CREATE TABLE KHACH_HANG (
    IDKhachHang INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    MaKhach VARCHAR(20) UNIQUE NOT NULL,
    NgayDangKy DATE NOT NULL DEFAULT GETDATE()
);

CREATE TABLE TAI_KHOAN (
    IDTaiKhoan INT IDENTITY(1,1) PRIMARY KEY,
    IDKhachHang INT NULL,
    TenDangNhap VARCHAR(50) UNIQUE NOT NULL,
    MatKhau VARCHAR(255) NOT NULL,
    Loai VARCHAR(10) NOT NULL CHECK (Loai IN ('khach', 'admin')),
    TrangThai BIT DEFAULT 1,
    NgayTao DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IDKhachHang) REFERENCES KHACH_HANG(IDKhachHang)
);

CREATE TABLE PHIM (
    IDPhim INT IDENTITY(1,1) PRIMARY KEY,
    TenPhim NVARCHAR(200) NOT NULL,
    DaoDien NVARCHAR(100) NOT NULL,
    TheLoai NVARCHAR(50) NOT NULL,
    ThoiLuong INT NOT NULL CHECK (ThoiLuong > 0),
    TomTat NVARCHAR(MAX) NOT NULL,
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'Đang chiếu',
    AnhBia NVARCHAR(255),
    AnhNen NVARCHAR(255)
);

CREATE TABLE PHONG_CHIEU (
    IDPhong INT IDENTITY(1,1) PRIMARY KEY,
    IDRap INT NOT NULL,
    TenPhong NVARCHAR(50) NOT NULL,
    SoLuongGhe INT NOT NULL CHECK (SoLuongGhe > 0),
    LoaiPhong NVARCHAR(20) DEFAULT N'2D',
    FOREIGN KEY (IDRap) REFERENCES RAP_PHIM(IDRap)
);


CREATE TABLE XUAT_CHIEU (
    IDXuatChieu INT IDENTITY(1,1) PRIMARY KEY,
    IDPhim INT NOT NULL,
    IDPhong INT NOT NULL,
    NgayChieu DATE NOT NULL,
    GioChieu TIME NOT NULL,
    GiaVe DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (IDPhim) REFERENCES PHIM(IDPhim),
    FOREIGN KEY (IDPhong) REFERENCES PHONG_CHIEU(IDPhong)
);


CREATE TABLE DON_DAT_VE (
    IDDonDatVe INT IDENTITY(1,1) PRIMARY KEY,
    IDKhachHang INT NOT NULL,
    IDXuatChieu INT NOT NULL,
    NgayDat DATETIME DEFAULT GETDATE(),
    TrangThaiDatVe NVARCHAR(20) DEFAULT N'Đã đặt',
    TongTien DECIMAL(10,2) DEFAULT 0,
    FOREIGN KEY (IDKhachHang) REFERENCES KHACH_HANG(IDKhachHang),
    FOREIGN KEY (IDXuatChieu) REFERENCES XUAT_CHIEU(IDXuatChieu)
);


CREATE TABLE CT_DATVE (
    IDChiTiet INT IDENTITY(1,1) PRIMARY KEY,
    IDDonDatVe INT NOT NULL,
    ViTriGhe VARCHAR(10) NOT NULL,
    Gia DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (IDDonDatVe) REFERENCES DON_DAT_VE(IDDonDatVe)
);


CREATE TABLE BINH_LUAN (
    IDBinhLuan INT IDENTITY(1,1) PRIMARY KEY,
    IDPhim INT NOT NULL,
    TenNguoiBinhLuan NVARCHAR(100) NOT NULL,
    NgayBinhLuan DATETIME DEFAULT GETDATE(),
    NoiDung NVARCHAR(MAX) NOT NULL,
    SoSao INT NOT NULL CHECK (SoSao >= 1 AND SoSao <= 5),
    FOREIGN KEY (IDPhim) REFERENCES PHIM(IDPhim)
);


CREATE TABLE TIN_TUC (
    IDTinTuc INT IDENTITY(1,1) PRIMARY KEY,
    TieuDe NVARCHAR(255) NOT NULL,
    AnhBia NVARCHAR(255),
    NoiDung NVARCHAR(MAX) NOT NULL,
    NgayDang DATETIME DEFAULT GETDATE()
);

CREATE INDEX IX_KHACH_HANG_Email ON KHACH_HANG(Email);
CREATE INDEX IX_KHACH_HANG_SoDienThoai ON KHACH_HANG(SoDienThoai);
CREATE INDEX IX_TAI_KHOAN_TenDangNhap ON TAI_KHOAN(TenDangNhap);
CREATE INDEX IX_TAI_KHOAN_IDKhachHang ON TAI_KHOAN(IDKhachHang);
CREATE INDEX IX_TAI_KHOAN_Loai ON TAI_KHOAN(Loai);
CREATE INDEX IX_PHIM_TenPhim ON PHIM(TenPhim);
CREATE INDEX IX_XUAT_CHIEU_NgayChieu ON XUAT_CHIEU(NgayChieu);
CREATE INDEX IX_XUAT_CHIEU_GioChieu ON XUAT_CHIEU(GioChieu);
CREATE INDEX IX_DON_DAT_VE_NgayDat ON DON_DAT_VE(NgayDat);
CREATE INDEX IX_DON_DAT_VE_TongTien ON DON_DAT_VE(TongTien);
CREATE INDEX IX_PHONG_CHIEU_IDRap ON PHONG_CHIEU(IDRap);
CREATE INDEX IX_BINH_LUAN_IDPhim ON BINH_LUAN(IDPhim);
CREATE INDEX IX_BINH_LUAN_NgayBinhLuan ON BINH_LUAN(NgayBinhLuan);
CREATE INDEX IX_TIN_TUC_NgayDang ON TIN_TUC(NgayDang);
CREATE INDEX IX_CT_DATVE_IDDonDatVe ON CT_DATVE(IDDonDatVe);
GO

INSERT INTO RAP_PHIM (TenRap, DiaChi, SoDienThoai, Email) VALUES
(N'Rạp 1', N'191 Bà Triệu, Quận Hai Bà Trưng, Hà Nội', '024 3974 9999', 'cgv.vincom@cgv.vn'),
(N'Rạp 2', N'54 Liễu Giai, Quận Ba Đình, Hà Nội', '024 3733 8555', 'lotte.hanoi@lotte.vn'),
(N'Rạp 3', N'Tầng 5, Vincom Mega Mall, 159 Phạm Văn Đồng, Hà Nội', '024 6253 9999', 'bhd.star@bhd.vn');


INSERT INTO KHACH_HANG (HoTen, SoDienThoai, Email, MaKhach, NgayDangKy) VALUES
(N'Nguyễn Văn An', '0912345678', 'nguyenvanan@email.com', 'KH001', '2024-01-15'),
(N'Trần Thị Bình', '0923456789', 'tranthibinh@email.com', 'KH002', '2024-01-16'),
(N'Lê Văn Cường', '0934567890', 'levancuong@email.com', 'KH003', '2024-01-17'),
(N'Phạm Thị Dung', '0945678901', 'phamthidung@email.com', 'KH004', '2024-01-18'),
(N'Hoàng Văn Em', '0956789012', 'hoangvanem@email.com', 'KH005', '2024-01-19');


INSERT INTO TAI_KHOAN (IDKhachHang, TenDangNhap, MatKhau, Loai, TrangThai) VALUES
(1, 'nguyenvanan', 'hashed_password_1', 'khach', 1),
(2, 'tranthibinh', 'hashed_password_2', 'khach', 1),
(3, 'levancuong', 'hashed_password_3', 'khach', 1),
(4, 'phamthidung', 'hashed_password_4', 'khach', 1),
(5, 'hoangvanem', 'hashed_password_5', 'khach', 1),
(NULL, 'admin', 'hashed_admin_password', 'admin', 1),
(NULL, 'quantri', 'hashed_quantri_password', 'admin', 1);


INSERT INTO PHIM (TenPhim, DaoDien, TheLoai, ThoiLuong, TomTat, TrangThai, AnhBia, AnhNen) VALUES
(N'Avengers: Endgame', N'Anthony Russo', N'Hành động', 181, N'Sau sự kiện tàn khốc trong Infinity War, các Avengers còn sót lại phải tìm cách đảo ngược hành động hủy diệt của Thanos trong trận chiến cuối cùng để cứu vũ trụ.', N'Đang chiếu', 'AnhBia1.jpg', 'AnhNen1.jpg'),
(N'Spider-Man: No Way Home', N'Jon Watts', N'Phiêu lưu', 148, N'Spider-Man phải đối mặt với hậu quả khi danh tính bị lộ và nhờ Doctor Strange giúp đỡ, dẫn đến việc mở ra đa vũ trụ và các kẻ thù từ các thực tại khác.', N'Đang chiếu', 'AnhBia2.jpg', 'AnhNen2.jpg'),
(N'The Batman', N'Matt Reeves', N'Hành động', 176, N'Batman trong năm thứ hai chiến đấu chống tội phạm ở Gotham City phải đối mặt với Riddler - một kẻ sát nhân bí ẩn nhắm vào giới thượng lưu thành phố.', N'Đang chiếu', 'AnhBia3.jpg', 'AnhNen3.jpg'),
(N'Black Panther', N'Ryan Coogler', N'Phiêu lưu', 134, N'TChalla trở về quê hương Wakanda để lên ngôi vua, nhưng phải đối mặt với một kẻ thù từ quá khứ đe dọa số phận đất nước và cả thế giới.', N'Sắp chiếu', 'AnhBia4.jpg', 'AnhNen4.jpg'),
(N'Avatar: Dòng chảy của nước', N'James Cameron', N'Khoa học viễn tưởng', 192, N'Jake Sully và gia đình bảo vệ Pandora khỏi mối đe dọa mới từ loài người, đồng thời khám phá thế giới đại dương của các bộ tộc Na vi.', N'Đang chiếu', 'AnhBia5.jpg', 'AnhNen5.jpg');

INSERT INTO PHONG_CHIEU (IDRap, TenPhong, SoLuongGhe, LoaiPhong) VALUES
(1, N'Phòng 1', 100, N'2D'),
(1, N'Phòng 2', 80, N'3D'),
(1, N'Phòng 3', 120, N'IMAX'),
(2, N'Phòng 1', 90, N'2D'),
(2, N'Phòng 2', 110, N'3D'),
(3, N'Phòng 1', 100, N'2D'),
(3, N'Phòng 2', 85, N'3D');


INSERT INTO XUAT_CHIEU (IDPhim, IDPhong, NgayChieu, GioChieu, GiaVe) VALUES
(1, 1, '2024-02-01', '19:00:00', 85000.00),
(1, 2, '2024-02-01', '21:30:00', 120000.00),
(2, 1, '2024-02-01', '18:30:00', 85000.00),
(2, 3, '2024-02-01', '20:00:00', 150000.00),
(3, 4, '2024-02-01', '19:15:00', 90000.00),
(3, 5, '2024-02-01', '22:00:00', 110000.00),
(5, 6, '2024-02-01', '17:45:00', 85000.00),
(5, 7, '2024-02-01', '20:45:00', 120000.00);

CREATE TABLE THANH_TOAN (
    IDThanhToan INT IDENTITY(1,1) PRIMARY KEY,
    IDDonDatVe INT NOT NULL,
    PhuongThuc NVARCHAR(50) NOT NULL,
    TrangThai NVARCHAR(20) DEFAULT N'Thành công',
    NgayThanhToan DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IDDonDatVe) REFERENCES DON_DAT_VE(IDDonDatVe)
);

DECLARE @TongTien1 DECIMAL(10,2), @TongTien2 DECIMAL(10,2), @TongTien3 DECIMAL(10,2), 
        @TongTien4 DECIMAL(10,2), @TongTien5 DECIMAL(10,2), @TongTien6 DECIMAL(10,2), 
        @TongTien7 DECIMAL(10,2);


SET @TongTien1 = 85000.00;
INSERT INTO DON_DAT_VE (IDKhachHang, IDXuatChieu, NgayDat, TrangThaiDatVe, TongTien) 
VALUES (1, 1, '2024-01-25 10:30:00', N'Đã đặt', @TongTien1);
INSERT INTO CT_DATVE (IDDonDatVe, ViTriGhe, Gia) VALUES (SCOPE_IDENTITY(), 'A10', 85000.00);

SET @TongTien2 = 85000.00;
INSERT INTO DON_DAT_VE (IDKhachHang, IDXuatChieu, NgayDat, TrangThaiDatVe, TongTien) 
VALUES (2, 1, '2024-01-25 11:15:00', N'Đã đặt', @TongTien2);
INSERT INTO CT_DATVE (IDDonDatVe, ViTriGhe, Gia) VALUES (SCOPE_IDENTITY(), 'B05', 85000.00);

SET @TongTien3 = 170000.00;
INSERT INTO DON_DAT_VE (IDKhachHang, IDXuatChieu, NgayDat, TrangThaiDatVe, TongTien) 
VALUES (3, 3, '2024-01-25 12:00:00', N'Đã đặt', @TongTien3);
DECLARE @DonVe3 INT = SCOPE_IDENTITY();
INSERT INTO CT_DATVE (IDDonDatVe, ViTriGhe, Gia) VALUES 
(@DonVe3, 'C12', 85000.00),
(@DonVe3, 'C13', 85000.00);

SET @TongTien4 = 270000.00;
INSERT INTO DON_DAT_VE (IDKhachHang, IDXuatChieu, NgayDat, TrangThaiDatVe, TongTien) 
VALUES (4, 5, '2024-01-25 13:45:00', N'Đã đặt', @TongTien4);
DECLARE @DonVe4 INT = SCOPE_IDENTITY();
INSERT INTO CT_DATVE (IDDonDatVe, ViTriGhe, Gia) VALUES 
(@DonVe4, 'D08', 90000.00),
(@DonVe4, 'D09', 90000.00),
(@DonVe4, 'D10', 90000.00);

SET @TongTien5 = 85000.00;
INSERT INTO DON_DAT_VE (IDKhachHang, IDXuatChieu, NgayDat, TrangThaiDatVe, TongTien) 
VALUES (5, 7, '2024-01-25 14:20:00', N'Đã đặt', @TongTien5);
INSERT INTO CT_DATVE (IDDonDatVe, ViTriGhe, Gia) VALUES (SCOPE_IDENTITY(), 'E15', 85000.00);

SET @TongTien6 = 300000.00;
INSERT INTO DON_DAT_VE (IDKhachHang, IDXuatChieu, NgayDat, TrangThaiDatVe, TongTien) 
VALUES (1, 4, '2024-01-26 09:30:00', N'Đã thanh toán', @TongTien6);
DECLARE @DonVe6 INT = SCOPE_IDENTITY();
INSERT INTO CT_DATVE (IDDonDatVe, ViTriGhe, Gia) VALUES 
(@DonVe6, 'F03', 150000.00),
(@DonVe6, 'F04', 150000.00);


SET @TongTien7 = 110000.00;
INSERT INTO DON_DAT_VE (IDKhachHang, IDXuatChieu, NgayDat, TrangThaiDatVe, TongTien) 
VALUES (2, 6, '2024-01-26 10:15:00', N'Đã hủy', @TongTien7);
INSERT INTO CT_DATVE (IDDonDatVe, ViTriGhe, Gia) VALUES (SCOPE_IDENTITY(), 'G11', 110000.00);


INSERT INTO BINH_LUAN (IDPhim, TenNguoiBinhLuan, NgayBinhLuan, NoiDung, SoSao) VALUES
(1, N'Nguyễn Văn An', '2024-01-20 09:00:00', N'Phim hay tuyệt vời! Cảm xúc lẫn hành động đều đỉnh cao. Kết thúc rất ý nghĩa cho chuỗi phim Avengers.', 5),
(1, N'Trần Thị Bình', '2024-01-20 10:30:00', N'Cốt truyện logic, diễn xuất xuất sắc. Đáng xem cho mọi fan của Marvel!', 4),
(2, N'Lê Văn Cường', '2024-01-21 14:15:00', N'Spider-Man lần này thực sự ấn tượng. Sự xuất hiện của các Spider-Man cũ làm mình rất bất ngờ!', 5),
(2, N'Phạm Thị Dung', '2024-01-21 16:45:00', N'Phim hay nhưng hơi dài, một số phân cảnh hơi rối.', 3),
(3, N'Hoàng Văn Em', '2024-01-22 11:20:00', N'Batman phiên bản mới rất khác biệt, tập trung vào phá án. Âm nhạc và hình ảnh xuất sắc.', 4),
(5, N'Nguyễn Văn An', '2024-01-23 19:30:00', N'Kỹ xảo đỉnh cao, thế giới Pandora lần này còn đẹp hơn phần 1. Cốt truyện cảm động về gia đình.', 5);


INSERT INTO TIN_TUC (TieuDe, AnhBia, NoiDung, NgayDang) VALUES
(N'Khuyến mãi đặc biệt tháng 2', 'khuyenmai.jpg', N'Nội dung chi tiết về chương trình khuyến mãi tháng 2 với nhiều ưu đãi hấp dẫn cho khách hàng.', '2024-01-20 08:00:00'),
(N'Phim mới tháng 2: Black Panther', 'blackpanther.jpg', N'Giới thiệu về phim Black Panther - một siêu phẩm hành động mới sẽ ra mắt vào cuối tháng 2.', '2024-01-21 10:30:00'),
(N'Cải tạo hệ thống âm thanh', 'amthanh.jpg', N'Rạp chiếu phim đã nâng cấp toàn bộ hệ thống âm thanh Dolby Atmos mang đến trải nghiệm xem phim tuyệt vời nhất.', '2024-01-22 14:15:00');
GO

CREATE OR ALTER PROCEDURE sp_DatVe
    @IDKhachHang INT,
    @IDXuatChieu INT,
    @DanhSachGhe NVARCHAR(MAX)
AS
BEGIN
    DECLARE @IDDonDatVe INT;
    DECLARE @TongTien DECIMAL(10,2) = 0;
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        DECLARE @GheTable TABLE (
            ViTriGhe VARCHAR(10),
            Gia DECIMAL(10,2)
        );
        
        INSERT INTO @GheTable (ViTriGhe, Gia)
        SELECT 
            LTRIM(RTRIM(LEFT(value, CHARINDEX(':', value) - 1))),
            CAST(SUBSTRING(value, CHARINDEX(':', value) + 1, LEN(value)) AS DECIMAL(10,2))
        FROM STRING_SPLIT(@DanhSachGhe, ',')
        WHERE value LIKE '%:%';
        SELECT @TongTien = ISNULL(SUM(Gia), 0) FROM @GheTable;
        INSERT INTO DON_DAT_VE (IDKhachHang, IDXuatChieu, NgayDat, TrangThaiDatVe, TongTien)
        VALUES (@IDKhachHang, @IDXuatChieu, GETDATE(), N'Đã đặt', @TongTien);
        
        SET @IDDonDatVe = SCOPE_IDENTITY();
        INSERT INTO CT_DATVE (IDDonDatVe, ViTriGhe, Gia)
        SELECT @IDDonDatVe, ViTriGhe, Gia
        FROM @GheTable;
        
        COMMIT TRANSACTION;
        
        SELECT @IDDonDatVe AS IDDonDatVe, @TongTien AS TongTien;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE sp_TimKhachHang
    @Email VARCHAR(100) = NULL,
    @SoDienThoai VARCHAR(15) = NULL
AS
BEGIN
    SELECT * FROM KHACH_HANG 
    WHERE Email = @Email OR SoDienThoai = @SoDienThoai;
END;
GO

CREATE PROCEDURE sp_GetPhongChieuByRap
    @IDRap INT
AS
BEGIN
    SELECT * FROM PHONG_CHIEU 
    WHERE IDRap = @IDRap
    ORDER BY TenPhong;
END;
GO

CREATE PROCEDURE sp_ThemBinhLuan
    @IDPhim INT,
    @TenNguoiBinhLuan NVARCHAR(100),
    @NoiDung NVARCHAR(MAX),
    @SoSao INT
AS
BEGIN
    INSERT INTO BINH_LUAN (IDPhim, TenNguoiBinhLuan, NoiDung, SoSao)
    VALUES (@IDPhim, @TenNguoiBinhLuan, @NoiDung, @SoSao);
    
    RETURN SCOPE_IDENTITY();
END;
GO

CREATE PROCEDURE sp_GetBinhLuanTheoPhim
    @IDPhim INT
AS
BEGIN
    SELECT * FROM BINH_LUAN 
    WHERE IDPhim = @IDPhim
    ORDER BY NgayBinhLuan DESC;
END;
GO

CREATE PROCEDURE sp_GetTinTucMoiNhat
    @SoLuong INT = 10
AS
BEGIN
    SELECT TOP (@SoLuong) * 
    FROM TIN_TUC 
    ORDER BY NgayDang DESC;
END;
GO

CREATE PROCEDURE sp_DangKyTaiKhoan
    @IDKhachHang INT = NULL,
    @TenDangNhap VARCHAR(50),
    @MatKhau VARCHAR(255),
    @Loai VARCHAR(10) = 'khach'
AS
BEGIN
    IF EXISTS (SELECT 1 FROM TAI_KHOAN WHERE TenDangNhap = @TenDangNhap)
    BEGIN
        RETURN -1;
    END
    
    INSERT INTO TAI_KHOAN (IDKhachHang, TenDangNhap, MatKhau, Loai)
    VALUES (@IDKhachHang, @TenDangNhap, @MatKhau, @Loai);
    
    RETURN SCOPE_IDENTITY();
END;
GO

CREATE PROCEDURE sp_DangNhap
    @TenDangNhap VARCHAR(50),
    @MatKhau VARCHAR(255)
AS
BEGIN
    SELECT 
        tk.IDTaiKhoan,
        tk.TenDangNhap,
        tk.Loai,
        tk.TrangThai,
        tk.NgayTao,
        kh.IDKhachHang,
        kh.HoTen,
        kh.Email,
        kh.SoDienThoai
    FROM TAI_KHOAN tk
    LEFT JOIN KHACH_HANG kh ON tk.IDKhachHang = kh.IDKhachHang
    WHERE tk.TenDangNhap = @TenDangNhap 
    AND tk.MatKhau = @MatKhau
    AND tk.TrangThai = 1;
END;
GO

CREATE PROCEDURE sp_DoiMatKhau
    @IDTaiKhoan INT,
    @MatKhauMoi VARCHAR(255)
AS
BEGIN
    UPDATE TAI_KHOAN 
    SET MatKhau = @MatKhauMoi 
    WHERE IDTaiKhoan = @IDTaiKhoan;
    
    RETURN @@ROWCOUNT;
END;
GO

CREATE PROCEDURE sp_KhoaTaiKhoan
    @IDTaiKhoan INT
AS
BEGIN
    UPDATE TAI_KHOAN 
    SET TrangThai = 0 
    WHERE IDTaiKhoan = @IDTaiKhoan;
    
    RETURN @@ROWCOUNT;
END;
GO

CREATE PROCEDURE sp_MoKhoaTaiKhoan
    @IDTaiKhoan INT
AS
BEGIN
    UPDATE TAI_KHOAN 
    SET TrangThai = 1 
    WHERE IDTaiKhoan = @IDTaiKhoan;
    
    RETURN @@ROWCOUNT;
END;
GO

CREATE PROCEDURE sp_GetChiTietDonVe
    @IDDonDatVe INT
AS
BEGIN
    SELECT 
        ddv.IDDonDatVe,
        ddv.IDKhachHang,
        ddv.IDXuatChieu,
        ddv.NgayDat,
        ddv.TrangThaiDatVe,
        ddv.TongTien,
        ctdg.ViTriGhe,
        ctdg.Gia,
        p.TenPhim,
        xc.NgayChieu,
        xc.GioChieu,
        ph.TenPhong,
        r.TenRap
    FROM DON_DAT_VE ddv
    INNER JOIN CT_DATVE ctdg ON ddv.IDDonDatVe = ctdg.IDDonDatVe
    INNER JOIN XUAT_CHIEU xc ON ddv.IDXuatChieu = xc.IDXuatChieu
    INNER JOIN PHIM p ON xc.IDPhim = p.IDPhim
    INNER JOIN PHONG_CHIEU ph ON xc.IDPhong = ph.IDPhong
    INNER JOIN RAP_PHIM r ON ph.IDRap = r.IDRap
    WHERE ddv.IDDonDatVe = @IDDonDatVe;
END;
GO

CREATE PROCEDURE sp_KiemTraGheDaDat
    @IDXuatChieu INT
AS
BEGIN
    SELECT DISTINCT ctdg.ViTriGhe
    FROM DON_DAT_VE ddv
    INNER JOIN CT_DATVE ctdg ON ddv.IDDonDatVe = ctdg.IDDonDatVe
    WHERE ddv.IDXuatChieu = @IDXuatChieu
    AND ddv.TrangThaiDatVe IN (N'Đã đặt', N'Đã thanh toán');
END;
GO

CREATE PROCEDURE sp_HuyDonVe
    @IDDonDatVe INT
AS
BEGIN
    UPDATE DON_DAT_VE
    SET TrangThaiDatVe = N'Đã hủy'
    WHERE IDDonDatVe = @IDDonDatVe
    AND TrangThaiDatVe = N'Đã đặt';
    
    RETURN @@ROWCOUNT;
END;
GO

CREATE PROCEDURE sp_ThanhToanDonVe
    @IDDonDatVe INT
AS
BEGIN
    UPDATE DON_DAT_VE
    SET TrangThaiDatVe = N'Đã thanh toán'
    WHERE IDDonDatVe = @IDDonDatVe
    AND TrangThaiDatVe = N'Đã đặt';
    
    RETURN @@ROWCOUNT;
END;
GO

CREATE PROCEDURE sp_ThongKeDoanhThu
    @TuNgay DATE = NULL,
    @DenNgay DATE = NULL,
    @IDRap INT = NULL
AS
BEGIN
    IF @TuNgay IS NULL SET @TuNgay = DATEADD(DAY, -30, GETDATE());
    IF @DenNgay IS NULL SET @DenNgay = GETDATE();
    
    SELECT 
        CONVERT(DATE, ddv.NgayDat) AS Ngay,
        COUNT(DISTINCT ddv.IDDonDatVe) AS SoDon,
        SUM(ctdg.Gia) AS TongDoanhThu,
        COUNT(ctdg.IDChiTiet) AS SoVeDaBan
    FROM DON_DAT_VE ddv
    INNER JOIN CT_DATVE ctdg ON ddv.IDDonDatVe = ctdg.IDDonDatVe
    INNER JOIN XUAT_CHIEU xc ON ddv.IDXuatChieu = xc.IDXuatChieu
    INNER JOIN PHONG_CHIEU pc ON xc.IDPhong = pc.IDPhong
    WHERE ddv.TrangThaiDatVe = N'Đã thanh toán'
    AND ddv.NgayDat BETWEEN @TuNgay AND DATEADD(DAY, 1, @DenNgay)
    AND (@IDRap IS NULL OR pc.IDRap = @IDRap)
    GROUP BY CONVERT(DATE, ddv.NgayDat)
    ORDER BY Ngay;
END;
GO

CREATE TRIGGER trg_TaoMaKhachHang
ON KHACH_HANG
AFTER INSERT
AS
BEGIN
    UPDATE kh
    SET MaKhach = 'KH' + RIGHT('000' + CAST(kh.IDKhachHang AS VARCHAR), 3)
    FROM KHACH_HANG kh
    INNER JOIN inserted i ON kh.IDKhachHang = i.IDKhachHang
    WHERE i.MaKhach = '' OR i.MaKhach IS NULL;
END;
GO

CREATE TRIGGER trg_CapNhatTrangThaiPhim
ON XUAT_CHIEU
AFTER INSERT, UPDATE
AS
BEGIN
    UPDATE PHIM
    SET TrangThai = N'Ngừng chiếu'
    WHERE IDPhim IN (
        SELECT p.IDPhim
        FROM PHIM p
        LEFT JOIN XUAT_CHIEU xc ON p.IDPhim = xc.IDPhim
        WHERE xc.NgayChieu < GETDATE() - 30
        AND p.TrangThai = N'Đang chiếu'
    );
END;
GO

CREATE TRIGGER trg_CapNhatTongTien
ON CT_DATVE
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    DECLARE @DonIDs TABLE (IDDonDatVe INT);
    
    INSERT INTO @DonIDs (IDDonDatVe)
    SELECT IDDonDatVe FROM inserted
    UNION
    SELECT IDDonDatVe FROM deleted;
    
    UPDATE DON_DAT_VE
    SET TongTien = (
        SELECT ISNULL(SUM(Gia), 0)
        FROM CT_DATVE
        WHERE CT_DATVE.IDDonDatVe = DON_DAT_VE.IDDonDatVe
    )
    WHERE IDDonDatVe IN (SELECT IDDonDatVe FROM @DonIDs);
END;
GO

select * from XUAT_CHIEU

INSERT INTO XUAT_CHIEU(IDPhim,IDPhong,NgayChieu,GioChieu,GiaVe) VALUES (1,1,GETDATE(),'19:00:00',80000)

CREATE OR ALTER PROCEDURE sp_GetSuatChieuByRapPhimNgay
    @IDRap INT,
    @IDPhim INT,
    @NgayChieu DATE
AS
BEGIN
    SELECT 
        xc.IDXuatChieu,
        xc.IDPhim,
        xc.IDPhong,
        xc.NgayChieu,
        CONVERT(VARCHAR(5), xc.GioChieu, 108) AS GioChieu, -- Format thành HH:mm
        xc.GiaVe,
        pc.TenPhong,
        pc.SoLuongGhe,
        pc.LoaiPhong,
        r.IDRap,
        r.TenRap,
        r.DiaChi,
        p.TenPhim,
        p.ThoiLuong
    FROM XUAT_CHIEU xc
    INNER JOIN PHONG_CHIEU pc ON xc.IDPhong = pc.IDPhong
    INNER JOIN RAP_PHIM r ON pc.IDRap = r.IDRap
    INNER JOIN PHIM p ON xc.IDPhim = p.IDPhim
    WHERE r.IDRap = @IDRap
        AND xc.IDPhim = @IDPhim
        AND xc.NgayChieu = @NgayChieu
    ORDER BY xc.GioChieu;
END;
GO


EXEC sp_GetSuatChieuByRapPhimNgay 
    @IDRap = 1,
    @IDPhim = 1,
    @NgayChieu = '2025-12-10';