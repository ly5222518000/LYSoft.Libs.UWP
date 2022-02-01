namespace LYSoft.Libs.UWP;

public static partial class Common {

    /// <summary>在新<see cref="AppWindow"/>窗口里显示指定内容</summary>
    public static async Task<AppWindow> ShowInNewWindowAsync(this UIElement element, string title) {
        AppWindow appWindow = await AppWindow.TryCreateAsync();
        var scrollviewer = new ScrollViewer() { Padding = new(50), };
        scrollviewer.Content = element;
        ElementCompositionPreview.SetAppWindowContent(appWindow, scrollviewer);
        appWindow.Title = title;
        await appWindow.TryShowAsync();
        return appWindow;
    }

    /// <summary>设置为没有标题栏的窗口，保留默认的标题栏位置</summary>
    public static void SetNoTitleBar() => SetNoTitleBar(null);

    /// <summary>设置为没有标题栏的窗口，使用指定的元素作为标题栏</summary>
    public static void SetNoTitleBar(this UIElement title) {
        CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
        Window.Current.SetTitleBar(title);
        var titleBar = ApplicationView.GetForCurrentView().TitleBar;
        titleBar.ButtonBackgroundColor = Colors.Transparent;
        titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
    }

    /// <summary>设置为有标题栏的窗口</summary>
    public static void SetTitleBar() {
        CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
        Window.Current.SetTitleBar(null);
        var titleBar = ApplicationView.GetForCurrentView().TitleBar;
        titleBar.ButtonBackgroundColor = null;
        titleBar.ButtonInactiveBackgroundColor = null;
    }

    /// <summary>设置为全屏模式</summary>
    public static void SetFullScreen() {
        ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
    }

    /// <summary>退出全屏模式</summary>
    public static void ExitFullScreen() {
        ApplicationView.GetForCurrentView().ExitFullScreenMode();
    }
}