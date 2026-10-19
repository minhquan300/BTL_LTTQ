using System.Text.Json;

namespace QuanLyNhaHang.Properties
{
    /// <summary>Small user-scoped settings store used by the WinForms client.</summary>
    internal sealed class Settings
    {
        private static readonly Lazy<Settings> Instance = new(() => new Settings());
        public static Settings Default => Instance.Value;

        private static string SettingsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "QuanLyNhaHang",
            "user-settings.json");

        public string RememberedUsername { get; set; } = string.Empty;
        public bool AutoPrintAfterPayment { get; set; }
        public string InvoicePrinterName { get; set; } = string.Empty;

        private Settings()
        {
            try
            {
                if (!File.Exists(SettingsPath)) return;
                var stored = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(SettingsPath));
                if (stored != null && stored.TryGetValue(nameof(RememberedUsername), out var username))
                    RememberedUsername = username;
                if (stored != null && stored.TryGetValue(nameof(AutoPrintAfterPayment), out var autoPrint) &&
                    bool.TryParse(autoPrint, out bool enabled))
                    AutoPrintAfterPayment = enabled;
                if (stored != null && stored.TryGetValue(nameof(InvoicePrinterName), out var printerName))
                    InvoicePrinterName = printerName;
            }
            catch
            {
                RememberedUsername = string.Empty;
            }
        }

        public void Save()
        {
            var path = SettingsPath;
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, JsonSerializer.Serialize(new Dictionary<string, string>
            {
                [nameof(RememberedUsername)] = RememberedUsername,
                [nameof(AutoPrintAfterPayment)] = AutoPrintAfterPayment.ToString(),
                [nameof(InvoicePrinterName)] = InvoicePrinterName
            }));
        }
    }
}
