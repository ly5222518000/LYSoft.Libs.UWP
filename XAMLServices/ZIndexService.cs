namespace LYSoft.Libs.UWP;

public static class ZIndexService {

    public static readonly DependencyProperty ZIndexProperty = DependencyProperty.Register("ZIndex", typeof(int), typeof(ZIndexService), new(0));
    public static void SetZIndex(UIElement element, int value) { element.SetElementZIndex(value); element.SetValue(ZIndexProperty, value); }
    public static int GetZIndex(UIElement element) => (int)element.GetValue(ZIndexProperty);

    private static void SetElementZIndex(this UIElement element, int value) {
        if (element.GetValue(UIElement.ShadowProperty) != null) { element.SetValue(UIElement.ShadowProperty, new ThemeShadow()); }
        value = value > 32 ? 32 : value < 0 ? 0 : value;
        var z = GetZIndex(element);
        z = z > 32 ? 32 : z < 0 ? 0 : z;
        var offset = value - z;
        //var a = 123;
        element.Translation += new Vector3(0, 0, offset);
    }
}