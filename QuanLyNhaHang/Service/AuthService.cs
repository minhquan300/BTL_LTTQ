using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyNhaHang.Service
{
    public enum ManagerApprovalStatus
    {
        InvalidCredentials,
        NotManager,
        Approved
    }

    public sealed record ManagerApprovalResult(ManagerApprovalStatus Status, int? EmployeeID = null, string? FullName = null);

    public class AuthService
    {
        private const int Pbkdf2Iterations = 600_000;
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const string Pbkdf2Prefix = "PBKDF2-SHA256";

        private static AuthService? _instance;
        public static AuthService Instance => _instance ??= new AuthService();

        private readonly UserRepository _repo = new();
        public User? CurrentUser { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;

        public bool IsAdmin => CurrentUser != null && (CurrentUser.RoleID == 1 ||
            CurrentUser.RoleName.Equals("Quan ly", StringComparison.OrdinalIgnoreCase) ||
            CurrentUser.RoleName.Equals("QuanLy", StringComparison.OrdinalIgnoreCase));

        public bool IsCashier => CurrentUser != null && (CurrentUser.RoleID == 2 ||
            CurrentUser.RoleName.Equals("Thu ngan", StringComparison.OrdinalIgnoreCase));

        public bool IsWaiter => CurrentUser != null && (CurrentUser.RoleID == 3 ||
            CurrentUser.RoleName.Equals("Phuc vu", StringComparison.OrdinalIgnoreCase));

        public bool IsChef => CurrentUser != null && (CurrentUser.RoleID == 4 ||
            CurrentUser.RoleName.Equals("Dau bep", StringComparison.OrdinalIgnoreCase));

        public async Task<(bool Success, string Message)> LoginAsync(string username, string password)
        {
            string attemptedUsername = (username ?? string.Empty).Trim();
            string suppliedPassword = password ?? string.Empty;
            int? attemptedEmployeeId = null;
            bool success = false;

            try
            {
                if (attemptedUsername.Length == 0 || suppliedPassword.Length == 0)
                    return (false, "Vui lòng nhập đầy đủ thông tin!");

                var user = await _repo.GetByUsernameAsync(attemptedUsername);
                if (user == null)
                    return (false, "Tên đăng nhập hoặc mật khẩu không chính xác.");
                attemptedEmployeeId = user.EmployeeID;

                bool valid = await VerifyPasswordAsync(suppliedPassword, user.PasswordHash, user.Username);
                if (!valid)
                    return (false, "Tên đăng nhập hoặc mật khẩu không chính xác.");

                if (NeedsRehash(user.PasswordHash))
                {
                    string upgradedHash = await HashPasswordAsync(suppliedPassword);
                    await _repo.UpdatePasswordAsync(user.EmployeeID, upgradedHash);
                    user.PasswordHash = upgradedHash;
                }

                CurrentUser = user;
                success = true;
                return (true, $"Chào mừng {user.FullName}!");
            }
            finally
            {
                // Strings are immutable in .NET; drop references immediately. The UI also clears its textbox.
                suppliedPassword = string.Empty;
                password = string.Empty;

                // Audit failures must never block authentication and only the account identifier is recorded.
                try
                {
                    await _repo.LogLoginAttemptAsync(
                        attemptedUsername,
                        attemptedEmployeeId,
                        success);
                }
                catch
                {
                    // The migration script creates the table. Until it is applied (or if the DB is unavailable),
                    // authentication continues without writing a credential or exception to logs.
                }
            }
        }

        public async Task<bool> ValidateManagerCredentialsAsync(string username, string password)
            => (await AuthenticateManagerForApprovalAsync(username, password)).Status == ManagerApprovalStatus.Approved;

        public async Task<int?> AuthenticateManagerAsync(string username, string password)
        {
            var result = await AuthenticateManagerForApprovalAsync(username, password);
            return result.Status == ManagerApprovalStatus.Approved ? result.EmployeeID : null;
        }

        public async Task<ManagerApprovalResult> AuthenticateManagerForApprovalAsync(string username, string password)
        {
            string suppliedPassword = password ?? string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(username) || suppliedPassword.Length == 0)
                    return new ManagerApprovalResult(ManagerApprovalStatus.InvalidCredentials);
                var user = await _repo.GetByUsernameAsync(username.Trim());
                if (user == null)
                    return new ManagerApprovalResult(ManagerApprovalStatus.InvalidCredentials);

                if (!await VerifyPasswordAsync(suppliedPassword, user.PasswordHash, user.Username))
                    return new ManagerApprovalResult(ManagerApprovalStatus.InvalidCredentials);

                bool isManager = user.RoleID == 1 ||
                    user.RoleName.Equals("Quan ly", StringComparison.OrdinalIgnoreCase) ||
                    user.RoleName.Equals("QuanLy", StringComparison.OrdinalIgnoreCase);

                bool isCurrentNonManager = CurrentUser != null &&
                    CurrentUser.EmployeeID == user.EmployeeID && !IsAdmin;
                if (isCurrentNonManager || !isManager)
                    return new ManagerApprovalResult(ManagerApprovalStatus.NotManager);

                if (NeedsRehash(user.PasswordHash))
                {
                    string upgradedHash = await HashPasswordAsync(suppliedPassword);
                    await _repo.UpdatePasswordAsync(user.EmployeeID, upgradedHash);
                    user.PasswordHash = upgradedHash;
                }
                return new ManagerApprovalResult(ManagerApprovalStatus.Approved, user.EmployeeID, user.FullName);
            }
            finally
            {
                suppliedPassword = string.Empty;
                password = string.Empty;
            }
        }

        public async Task LogManagerApprovalLockoutAsync(string username)
        {
            try
            {
                await _repo.LogSecurityWarningAsync(username,
                    "ManagerApprovalLockout", "Ba lần xác thực quản lý liên tiếp không thành công; khóa xác nhận 30 giây.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceWarning("Unable to persist manager approval lockout audit: {0}", ex.GetType().Name);
            }
        }

        public Task LogManagerApprovalAsync(int managerEmployeeId, string actionDescription)
        {
            User? requester = CurrentUser;
            return _repo.LogApprovalAsync(
                managerEmployeeId,
                requester?.EmployeeID,
                requester?.Username,
                requester?.FullName,
                actionDescription);
        }

        public void Logout() => CurrentUser = null;

        public static string HashPassword(string password)
        {
            ArgumentNullException.ThrowIfNull(password);
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] derived = Array.Empty<byte>();
            try
            {
                derived = Rfc2898DeriveBytes.Pbkdf2(
                    passwordBytes, salt, Pbkdf2Iterations, HashAlgorithmName.SHA256, HashSize);
                return $"{Pbkdf2Prefix}${Pbkdf2Iterations.ToString(CultureInfo.InvariantCulture)}$" +
                       $"{Convert.ToBase64String(salt)}${Convert.ToBase64String(derived)}";
            }
            finally
            {
                CryptographicOperations.ZeroMemory(passwordBytes);
                CryptographicOperations.ZeroMemory(salt);
                if (derived.Length > 0) CryptographicOperations.ZeroMemory(derived);
            }
        }

        public static Task<string> HashPasswordAsync(string password) =>
            Task.Run(() => HashPassword(password));

        private static Task<bool> VerifyPasswordAsync(string password, string storedValue, string username) =>
            Task.Run(() => VerifyPassword(password, storedValue, username));

        private static bool VerifyPassword(string password, string storedValue, string username)
        {
            if (storedValue.StartsWith(Pbkdf2Prefix + "$", StringComparison.Ordinal))
                return VerifyPbkdf2(password, storedValue);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[]? expected = null;
            byte[]? actual = null;
            try
            {
                if (string.Equals(storedValue, "CHUA_CO_HASH", StringComparison.Ordinal))
                {
                    return FixedTimeTextEquals(password, "123456") ||
                           FixedTimeTextEquals(password, "admin") ||
                           FixedTimeTextEquals(password, username);
                }

                if (IsHex(storedValue, 64))
                    expected = Convert.FromHexString(storedValue);
                else if (IsHex(storedValue, 40))
                    expected = Convert.FromHexString(storedValue);
                else if (IsHex(storedValue, 32))
                    expected = Convert.FromHexString(storedValue);
                else
                    return FixedTimeTextEquals(password, storedValue); // Legacy plaintext, upgraded after success.

                if (IsHex(storedValue, 64))
                    actual = SHA256.HashData(passwordBytes);
                else if (IsHex(storedValue, 40))
                    actual = SHA1.HashData(passwordBytes);
                else
                    actual = MD5.HashData(passwordBytes);

                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            catch (FormatException)
            {
                return false;
            }
            finally
            {
                CryptographicOperations.ZeroMemory(passwordBytes);
                if (expected != null) CryptographicOperations.ZeroMemory(expected);
                if (actual != null) CryptographicOperations.ZeroMemory(actual);
            }
        }

        private static bool VerifyPbkdf2(string password, string encoded)
        {
            string[] parts = encoded.Split('$');
            if (parts.Length != 4 || parts[0] != Pbkdf2Prefix ||
                !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out int iterations) ||
                iterations < 100_000 || iterations > 2_000_000)
                return false;

            byte[]? salt = null;
            byte[]? expected = null;
            byte[]? passwordBytes = null;
            byte[]? actual = null;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expected = Convert.FromBase64String(parts[3]);
                if (salt.Length < 16 || salt.Length > 64 || expected.Length != HashSize) return false;
                passwordBytes = Encoding.UTF8.GetBytes(password);
                actual = Rfc2898DeriveBytes.Pbkdf2(
                    passwordBytes, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            catch (FormatException)
            {
                return false;
            }
            finally
            {
                if (salt != null) CryptographicOperations.ZeroMemory(salt);
                if (expected != null) CryptographicOperations.ZeroMemory(expected);
                if (passwordBytes != null) CryptographicOperations.ZeroMemory(passwordBytes);
                if (actual != null) CryptographicOperations.ZeroMemory(actual);
            }
        }

        private static bool NeedsRehash(string storedValue)
        {
            string[] parts = storedValue.Split('$');
            return parts.Length != 4 || parts[0] != Pbkdf2Prefix ||
                   !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out int iterations) ||
                   iterations != Pbkdf2Iterations;
        }

        private static bool IsHex(string value, int length)
        {
            if (value.Length != length) return false;
            foreach (char c in value)
                if (!Uri.IsHexDigit(c)) return false;
            return true;
        }

        private static bool FixedTimeTextEquals(string left, string right)
        {
            byte[] leftBytes = Encoding.UTF8.GetBytes(left);
            byte[] rightBytes = Encoding.UTF8.GetBytes(right);
            try
            {
                return leftBytes.Length == rightBytes.Length &&
                       CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(leftBytes);
                CryptographicOperations.ZeroMemory(rightBytes);
            }
        }
    }
}
