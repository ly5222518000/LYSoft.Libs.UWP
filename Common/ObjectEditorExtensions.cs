namespace LYSoft.Libs.UWP;

public static partial class Common {

    /// <summary>使用<see cref="ContentDialog"/>创建一个指定类型的对象</summary>
    public static async Task<T> EditAsync<T>() where T : new() => await EditAsync(new T());

    /// <summary>使用<see cref="ContentDialog"/>编辑一个指定类型的对象</summary>
    public static async Task<T> EditAsync<T>(this T obj) where T : new() {
        var typeattr = typeof(T).GetCustomAttribute<ObjectEditorTypeAttribute>();
        var page = new ObjectEditorPage<T>(obj);
        var dialog = new ContentDialog() { Title = typeattr?.Header ?? "编辑", Content = page, CloseButtonText = "确定" };
        await dialog.ShowAsync();
        return obj;
    }

    /// <summary>使用<see cref="AppWindow"/>创建一个指定类型的对象</summary>
    public static async Task<T> EditInNewWindowAsync<T>() where T : new() => await EditInNewWindowAsync(new T());

    /// <summary>使用<see cref="AppWindow"/>编辑一个指定类型的对象</summary>
    public static async Task<T> EditInNewWindowAsync<T>(this T obj) where T : new() {
        var typeattr = typeof(T).GetCustomAttribute<ObjectEditorTypeAttribute>();
        var page = new ObjectEditorPage<T>(obj);
        var window = await page.ShowInNewWindowAsync(typeattr?.Header ?? "编辑");
        var closed = false;
        window.Closed += (_, _) => closed = true;
        while (!closed) { await Task.Delay(10); }
        return obj;
    }

}