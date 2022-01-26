namespace LYSoft.Libs.UWP;

public static partial class Common {

    /// <summary>显示消息对话框</summary>
    public static async Task MessageAsync(this UIElement element, string message) {
        var dialog = new ContentDialog() { Title = "系统提示", Content = message, CloseButtonText = "确定" };
        await dialog.ShowAsync();
    }

    /// <summary>显示询问对话框并返回结果</summary>
    public static async Task<bool> ConfirmAsync(this UIElement element, string message) {
        var dialog = new ContentDialog() { Title = "系统提示", Content = message, PrimaryButtonText = "是", CloseButtonText = "否" };
        return await dialog.ShowAsync() == ContentDialogResult.Primary;
    }

    /// <summary>在控件指定方位显示消息提示</summary>
    public static Flyout Tip(this UIElement element, string message, FlyoutPlacementMode placement) {
        try {
            var flyout = new Flyout() { Placement = placement, Content = new TextBlock() { Text = message } };
            element.ContextFlyout = flyout;
            flyout.ShowAt((FrameworkElement)element);
            return flyout;
        } catch {
            return null;
        }
    }
}