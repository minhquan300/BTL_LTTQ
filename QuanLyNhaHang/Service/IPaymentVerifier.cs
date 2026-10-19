using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace QuanLyNhaHang.Service;

/// <summary>
/// Server-side extension point for future provider callbacks. Implementations must verify the
/// provider signature/token against the raw request body before returning a verified result.
/// Never implement this by accepting a success flag posted by the cashier/client application.
/// </summary>
public interface IPaymentVerifier
{
    Task<PaymentVerificationResult> VerifyAsync(
        PaymentVerificationRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record PaymentVerificationRequest(
    string Provider,
    string RawBody,
    string Signature,
    string? AccessToken,
    string ExpectedReference,
    string ExpectedTransferInformation,
    decimal ExpectedAmount);

public sealed record PaymentVerificationResult(
    bool SignatureValid,
    bool AmountMatches,
    bool ReferenceMatches,
    bool ProviderConfirmsSettlement,
    string? TransactionId = null)
{
    public bool IsVerifiedSettlement => SignatureValid && AmountMatches &&
        ReferenceMatches && ProviderConfirmsSettlement;
}

/// <summary>Reads webhook secrets from environment variables, never from source code.</summary>
public sealed class EnvironmentPaymentSecretProvider
{
    public string? GetWebhookSecret(string provider)
    {
        string normalized = Regex.Replace(provider ?? string.Empty, "[^A-Za-z0-9]", "").ToUpperInvariant();
        if (normalized.Length == 0) return null;
        string? secret = Environment.GetEnvironmentVariable($"PAYMENT_{normalized}_WEBHOOK_SECRET");
        return string.IsNullOrWhiteSpace(secret) ? null : secret;
    }
}

/// <summary>Constant-time HMAC-SHA256 helper for providers using a hex body signature.</summary>
public static class PaymentSignatureValidation
{
    public static bool VerifyToken(string suppliedToken, string configuredSecret)
    {
        if (string.IsNullOrEmpty(suppliedToken) || string.IsNullOrEmpty(configuredSecret)) return false;
        byte[] supplied = Encoding.UTF8.GetBytes(suppliedToken);
        byte[] expected = Encoding.UTF8.GetBytes(configuredSecret);
        try
        {
            return supplied.Length == expected.Length &&
                CryptographicOperations.FixedTimeEquals(supplied, expected);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(supplied);
            CryptographicOperations.ZeroMemory(expected);
        }
    }

    public static bool VerifyHmacSha256Hex(string rawBody, string signatureHex, string secret)
    {
        if (string.IsNullOrEmpty(rawBody) || string.IsNullOrWhiteSpace(signatureHex) ||
            string.IsNullOrEmpty(secret)) return false;
        byte[] supplied;
        try { supplied = Convert.FromHexString(signatureHex.Trim()); }
        catch (FormatException) { return false; }
        byte[] key = Encoding.UTF8.GetBytes(secret);
        byte[] body = Encoding.UTF8.GetBytes(rawBody);
        byte[] expected = HMACSHA256.HashData(key, body);
        try
        {
            return supplied.Length == expected.Length &&
                CryptographicOperations.FixedTimeEquals(supplied, expected);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(key);
            CryptographicOperations.ZeroMemory(body);
            CryptographicOperations.ZeroMemory(expected);
            CryptographicOperations.ZeroMemory(supplied);
        }
    }
}
