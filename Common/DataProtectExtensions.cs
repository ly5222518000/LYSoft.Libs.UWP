namespace LYSoft.Libs.UWP;

public static partial class Common {

    private static readonly BinaryStringEncoding encoding = BinaryStringEncoding.Utf8;

    /// <summary>加密<see cref="IBuffer"/>数据</summary>
    public static async Task<IBuffer> ProtectAsync(this IBuffer buffer) => await new DataProtectionProvider("LOCAL=user").ProtectAsync(buffer);
    /// <summary>加密<see cref="string"/>数据</summary>
    public static async Task<IBuffer> ProtectAsync(this string message) => await ProtectAsync(CryptographicBuffer.ConvertStringToBinary(message, encoding));

    /// <summary>解密数据到<see cref="IBuffer"/></summary>
    public static async Task<IBuffer> UnprotectAsync(this IBuffer buffer) => await new DataProtectionProvider().UnprotectAsync(buffer);
    /// <summary>解密数据到<see cref="string"/></summary>
    public static async Task<string> UnprotectToStringAsync(this IBuffer buffer) => CryptographicBuffer.ConvertBinaryToString(encoding, await UnprotectAsync(buffer));
}