namespace LYSoft.Libs.UWP;

public static partial class Common {

    private static readonly BinaryStringEncoding encoding = BinaryStringEncoding.Utf8;

    public static async Task<IBuffer> ProtectAsync(this IBuffer buffer) => await new DataProtectionProvider("LOCAL=user").ProtectAsync(buffer);
    public static async Task<IBuffer> ProtectAsync(this string message) => await ProtectAsync(CryptographicBuffer.ConvertStringToBinary(message, encoding));

    public static async Task<IBuffer> UnprotectAsync(this IBuffer buffer) => await new DataProtectionProvider().UnprotectAsync(buffer);
    public static async Task<string> UnprotectToStringAsync(this IBuffer buffer) => CryptographicBuffer.ConvertBinaryToString(encoding, await UnprotectAsync(buffer));
}