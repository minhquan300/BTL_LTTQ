using System.Globalization;
using System.Text;

namespace QuanLyNhaHang.Helpers
{
    internal static class MenuImageStorage
    {
        private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".jfif", ".png", ".bmp", ".gif"
        };

        public static bool IsSupportedImage(string path) =>
            SupportedExtensions.Contains(Path.GetExtension(path));

        public static string? Resolve(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath)) return null;
            if (Path.IsPathRooted(imagePath))
                return File.Exists(imagePath) ? imagePath : null;

            string path = Path.Combine(AppContext.BaseDirectory, imagePath);
            return File.Exists(path) ? path : null;
        }

        public static string CopyIntoApplicationImages(string sourcePath, int menuItemId)
        {
            if (!File.Exists(sourcePath))
                throw new FileNotFoundException("Không tìm thấy tệp ảnh đã chọn.", sourcePath);
            if (!IsSupportedImage(sourcePath))
                throw new InvalidDataException("Chỉ hỗ trợ ảnh JPG, JPEG, PNG, BMP hoặc GIF.");

            string imagesDirectory = Path.Combine(AppContext.BaseDirectory, "Images");
            Directory.CreateDirectory(imagesDirectory);

            string extension = Path.GetExtension(sourcePath).ToLowerInvariant();
            string originalName = Path.GetFileNameWithoutExtension(sourcePath);
            string safeName = NormalizeFileName(originalName);
            string uniquePart = menuItemId > 0 ? menuItemId.ToString("D5", CultureInfo.InvariantCulture) : Guid.NewGuid().ToString("N");
            string destinationName = $"{uniquePart}_{safeName}_{Guid.NewGuid():N}{extension}";
            string destinationPath = Path.Combine(imagesDirectory, destinationName);
            File.Copy(sourcePath, destinationPath, overwrite: false);
            return Path.Combine("Images", destinationName);
        }

        public static string NormalizeForMatching(string value)
        {
            string normalized = value.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);
            foreach (char character in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
                    continue;
                builder.Append(char.IsLetterOrDigit(character) ? char.ToLowerInvariant(character) : ' ');
            }

            return string.Join(' ', builder.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        private static string NormalizeFileName(string value)
        {
            string normalized = NormalizeForMatching(value);
            string safe = string.Join('-', normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            return string.IsNullOrWhiteSpace(safe) ? "mon-an" : safe;
        }
    }
}
