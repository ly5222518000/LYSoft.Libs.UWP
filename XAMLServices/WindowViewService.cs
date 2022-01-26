namespace LYSoft.Libs.UWP;

public static class WindowViewService {

    public static readonly DependencyProperty TitleBarModeProperty = DependencyProperty.Register("TitleBarMode", typeof(WindowStyleTitleBarMode), typeof(WindowViewService), new(WindowStyleTitleBarMode.Normal));
    public static void SetTitleBarMode(Page page, WindowStyleTitleBarMode value) { page.SetValue(TitleBarModeProperty, value); page.SetTitleBarStyle(); }
    public static WindowStyleTitleBarMode GetTitleBarMode(Page page) => (WindowStyleTitleBarMode)page.GetValue(TitleBarModeProperty);

    public static readonly DependencyProperty TitleBarElementProperty = DependencyProperty.Register("TitleBarElement", typeof(UIElement), typeof(WindowViewService), new(null));
    public static void SetTitleBarElement(Page page, UIElement value) { page.SetValue(TitleBarElementProperty, value); page.SetTitleBarStyle(); }
    public static UIElement GetTitleBarElement(Page page) => (UIElement)page.GetValue(TitleBarElementProperty);

    private static void SetTitleBarStyle(this Page page) {
        var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
        var titleBar = ApplicationView.GetForCurrentView().TitleBar;
        var titleBarElement = GetTitleBarElement(page);
        switch (GetTitleBarMode(page)) {
            case WindowStyleTitleBarMode.Normal:
                Common.SetTitleBar();
                break;
            case WindowStyleTitleBarMode.NoTitleBar:
                Common.SetNoTitleBar(titleBarElement);
                break;
        }
    }

    public static readonly DependencyProperty ViewModeProperty = DependencyProperty.Register("ViewMode", typeof(WindowStyleViewMode), typeof(WindowViewService), new(WindowStyleViewMode.Normal));
    public static void SetViewMode(Page page, WindowStyleViewMode value) { page.SetValue(TitleBarModeProperty, value); page.SetViewMode(); }
    public static WindowStyleViewMode GetViewMode(Page page) => (WindowStyleViewMode)page.GetValue(TitleBarModeProperty);

    private static async void SetViewMode(this Page page) {
        switch (GetViewMode(page)) {
            case WindowStyleViewMode.Normal:
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
                break;
            case WindowStyleViewMode.CompactOverlay:
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
                break;
            case WindowStyleViewMode.FullScreen:
                Common.SetFullScreen();
                break;
            default:
                break;
        }
    }
}

public enum WindowStyleTitleBarMode {
    Normal,
    NoTitleBar,
}

public enum WindowStyleViewMode {
    Normal,
    FullScreen,
    CompactOverlay,
}