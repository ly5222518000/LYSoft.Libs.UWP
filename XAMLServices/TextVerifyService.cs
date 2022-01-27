namespace LYSoft.Libs.UWP;

/// <summary>文本验证服务，可用于<see cref="TextBox"/></summary>
public static class TextVerifyService {

    private static List<TextBox> list = new();

    /// <summary>验证失败提示依赖属性</summary>
    public static readonly DependencyProperty FailTipProperty = DependencyProperty.Register("FailTip", typeof(string), typeof(TextVerifyService), new("输入的内容不正确"));
    /// <summary>设置验证失败提示</summary>
    public static void SetFailTip(TextBox element, string value) => element.SetValue(FailTipProperty, value);
    /// <summary>获取验证失败提示</summary>
    public static string GetFailTip(TextBox element) => (string)element.GetValue(FailTipProperty);

    /// <summary>验证预设类型依赖属性</summary>
    public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(TextVerifyType), typeof(TextVerifyService), new(TextVerifyType.None));
    /// <summary>设置验证预设类型</summary>
    public static void SetType(TextBox element, TextVerifyType value) { element.SetValue(TypeProperty, value); if (!list.Contains(element)) { element.LostFocus += Element_LostFocus; list.Add(element); } }
    /// <summary>获取验证预设类型</summary>
    public static TextVerifyType GetType(TextBox element) => (TextVerifyType)element.GetValue(TypeProperty);

    /// <summary>验证正则表达式依赖属性</summary>
    public static readonly DependencyProperty RegexProperty = DependencyProperty.Register("Regex", typeof(string), typeof(TextVerifyService), new(null));
    /// <summary>设置验证正则表达式</summary>
    public static void SetRegex(TextBox element, string value) { element.SetValue(RegexProperty, value); if (!list.Contains(element)) { element.LostFocus += Element_LostFocus; list.Add(element); } }
    /// <summary>获取验证正则表达式</summary>
    public static string GetRegex(TextBox element) => (string)element.GetValue(RegexProperty);

    /// <summary>验证允许空值依赖属性</summary>
    public static readonly DependencyProperty AllowEmptyProperty = DependencyProperty.Register("AllowEmpty", typeof(bool), typeof(TextVerifyService), new(true));
    /// <summary>设置验证允许空值</summary>
    public static void SetAllowEmpty(TextBox element, bool value) => element.SetValue(AllowEmptyProperty, value);
    /// <summary>获取验证允许空值</summary>
    public static bool GetAllowEmpty(TextBox element) => (bool)element.GetValue(AllowEmptyProperty);

    private static string GetTypeRegexText(this TextVerifyType type) => typeof(TextVerifyType).GetField(type.ToString()).GetCustomAttributes().OfType<TextVerifyTypeAttribute>().Single().Regex;

    private static void Element_LostFocus(object sender, RoutedEventArgs e) {
        var element = sender as TextBox;
        if (GetAllowEmpty(element) && string.IsNullOrEmpty(element.Text)) { return; }
        var type = GetType(element);

        var isfail = Enum
            .GetValues(typeof(TextVerifyType))
            .Cast<TextVerifyType>()
            .Where(t => type.HasFlag(t))
            .Select(t => t.GetTypeRegexText())
            .Append(GetRegex(element))
            .Where(t => !string.IsNullOrEmpty(t))
            .All(t => !Regex.IsMatch(element.Text, t));
        if (isfail) {
            var tip = element.Tip(GetFailTip(element), FlyoutPlacementMode.Bottom);
            if (tip == null) {
                element.Focus(FocusState.Pointer);
            } else {
                tip.Closed += (_, _) => element.Focus(FocusState.Pointer);
            }
        }
    }

}

public static class PasswordVerifyService {

    private static List<PasswordBox> list = new();

    public static readonly DependencyProperty FailTipProperty = DependencyProperty.Register("FailTip", typeof(string), typeof(PasswordVerifyService), new("输入的内容不正确"));
    public static void SetFailTip(PasswordBox element, string value) => element.SetValue(FailTipProperty, value);
    public static string GetFailTip(PasswordBox element) => (string)element.GetValue(FailTipProperty);

    public static readonly DependencyProperty RegexProperty = DependencyProperty.Register("Regex", typeof(string), typeof(PasswordVerifyService), new(null));
    public static void SetRegex(PasswordBox element, string value) { element.SetValue(RegexProperty, value); if (!list.Contains(element)) { element.LostFocus += Element_LostFocus; list.Add(element); } }
    public static string GetRegex(PasswordBox element) => (string)element.GetValue(RegexProperty);

    public static readonly DependencyProperty AllowEmptyProperty = DependencyProperty.Register("AllowEmpty", typeof(bool), typeof(PasswordVerifyService), new(true));
    public static void SetAllowEmpty(PasswordBox element, bool value) => element.SetValue(AllowEmptyProperty, value);
    public static bool GetAllowEmpty(PasswordBox element) => (bool)element.GetValue(AllowEmptyProperty);

    private static void Element_LostFocus(object sender, RoutedEventArgs e) {
        var element = sender as PasswordBox;
        if (GetAllowEmpty(element) && string.IsNullOrEmpty(element.Password)) { return; }
        if (!Regex.IsMatch(element.Password, GetRegex(element))) {
            var tip = element.Tip(GetFailTip(element), FlyoutPlacementMode.Bottom);
            if (tip == null) {
                element.Focus(FocusState.Pointer);
            } else {
                tip.Closed += (_, _) => element.Focus(FocusState.Pointer);
            }
        }

    }

}

[Flags]
public enum TextVerifyType {

    [TextVerifyType(null)]
    None = 1 << 0,

    [TextVerifyType(@"^[A-Za-z0-9\u4e00-\u9fa5]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$")]
    Email = 1 << 1,

    [TextVerifyType(@"^\d+$")]
    Integer = 1 << 2,

    [TextVerifyType(@"^\d+(\.\d+)?$")]
    Number = 1 << 3,

    [TextVerifyType(@"^https?://[^\s]*$")]
    WebURL = 1 << 4,

    [TextVerifyType(@"^[1-9]\d{5}(19|20)\d{2}((0[1-9])|(1[0-2]))((0[1-9])|([1-2][1-9])|(3[0-1]))\d{3}[0-9xX]$")]
    IDCard = 1 << 5,

    [TextVerifyType(@"^1\d{10}$")]
    Phone = 1 << 6,

}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
internal class TextVerifyTypeAttribute : Attribute {
    public TextVerifyTypeAttribute(string regex) {
        Regex = regex;
    }
    public string Regex { get; }
}