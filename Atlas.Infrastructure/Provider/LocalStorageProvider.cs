using Atlas.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Atlas.Infrastructure.Repositories
{
    public class LocalStorageProvider : IStorageProvider
    {
        private readonly string _basePath;

        public LocalStorageProvider(IConfiguration configuration)
        {
            // 1. Lấy tên thư mục từ file config
            string folderName = configuration.GetSection("StorageSettings:FolderName").Value ?? "AtlasStorage";

            // 2. Lấy thư mục nơi project đang chạy (thường là Atlas.Web)
            string currentDir = Directory.GetCurrentDirectory();

            // 3. ÉP BUỘC lùi ra ngoài 1 cấp bằng hàm của hệ điều hành (Ra thư mục Solution chứa các project)
            // Dấu ? để phòng hờ trường hợp ứng dụng bị deploy ở ổ đĩa gốc (C:\)
            string parentDir = Directory.GetParent(currentDir)?.FullName ?? currentDir;

            // 4. Kết hợp lại thành đường dẫn tuyệt đối động
            _basePath = Path.Combine(parentDir, folderName);

            // Tạo sẵn thư mục vật lý nếu chưa có
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string folderName, string fileName)
        {
            // 1. Tạo đường dẫn phân vùng theo thời gian (VD: Products/2026/06/18)
            string timePartition = Path.Combine(
                DateTime.Now.Year.ToString(),
                DateTime.Now.Month.ToString("D2"),
                DateTime.Now.Day.ToString("D2")
            );

            string relativeFolder = Path.Combine(folderName, timePartition);
            string absoluteFolder = Path.Combine(_basePath, relativeFolder);

            // 2. Đảm bảo thư mục vật lý đã tồn tại
            if (!Directory.Exists(absoluteFolder))
            {
                Directory.CreateDirectory(absoluteFolder);
            }

            // 3. Tạo tên file duy nhất (tránh ghi đè nếu trùng tên)
            string uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

            // Đường dẫn tương đối lưu vào DB (dùng dấu / cho chuẩn Web)
            string relativeFilePath = Path.Combine(relativeFolder, uniqueFileName).Replace("\\", "/");

            // Đường dẫn tuyệt đối để lưu xuống ổ cứng
            string absoluteFilePath = Path.Combine(absoluteFolder, uniqueFileName);

            // 4. Ghi file xuống ổ cứng
            using (var stream = new FileStream(absoluteFilePath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.CopyToAsync(stream);
            }

            return relativeFilePath;
        }

        public async Task<Stream> GetFileAsync(string relativePath)
        {
            // Chuẩn hóa lại đường dẫn cho hệ điều hành hiện tại
            relativePath = relativePath.TrimStart('/').Replace("/", "\\");
            string absoluteFilePath = Path.Combine(_basePath, relativePath);

            if (!File.Exists(absoluteFilePath))
            {
                throw new FileNotFoundException($"File not found at: {absoluteFilePath}");
            }

            // Mở file dưới dạng đọc (Read) và không khóa file (FileShare.Read)
            return new FileStream(absoluteFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}