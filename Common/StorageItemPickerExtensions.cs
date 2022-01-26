using Windows.Storage.Pickers;

namespace LYSoft.Libs.UWP;

public static partial class Common {

    /// <summary>从各种控件生成默认 StorageItem 处理函数</summary>
    public static Action<IStorageItem> GetStorageItemSetter(this UIElement element) {
        return element switch {
            TextBox t => f => t.Text = f.Path,
            TextBlock t => f => t.Text = f.Path,
            ContentControl t => f => t.Content = f.Path,
            _ => throw new ArgumentException(),
        };
    }

    /// <summary>打开一个StorageFile</summary>
    public static async Task<StorageFile> GetOpenFileAsync(this UIElement _) => await GetOpenFileAsync(_, null, null);

    /// <summary>使用指定的筛选器打开一个StorageFile</summary>
    public static async Task<StorageFile> GetOpenFileAsync(this UIElement element, IEnumerable<string> filters) => await GetOpenFileAsync(element, filters, null);

    /// <summary>打开一个StorageFile，并使用指定的回调函数处理该文件</summary>
    public static async Task<StorageFile> GetOpenFileAsync(this UIElement element, Action<StorageFile> setter) => await GetOpenFileAsync(element, null, null);

    /// <summary>使用指定的筛选器打开一个StorageFile，并使用指定的回调函数处理该文件</summary>
    public static async Task<StorageFile> GetOpenFileAsync(this UIElement element, IEnumerable<string> filters, Action<StorageFile> setter) {
        var filters2 = filters ?? new[] { "*" };
        if (filters2.Count() == 0) { filters2 = new[] { "*" }; }
        var picker = new FileOpenPicker();
        foreach (var filter in filters2) {
            picker.FileTypeFilter.Add(filter);
        }
        var file = await picker.PickSingleFileAsync();
        if (file == null) { return null; }
        setter?.Invoke(file);
        return file;
    }

    /// <summary>打开一组StorageFile</summary>
    public static async Task<IEnumerable<StorageFile>> GetOpenFilesAsync(this UIElement _) => await GetOpenFilesAsync(_, null, null);

    /// <summary>使用指定的筛选器打开一组StorageFile</summary>
    public static async Task<IEnumerable<StorageFile>> GetOpenFilesAsync(this UIElement element, IEnumerable<string> filters) => await GetOpenFilesAsync(element, filters, null);

    /// <summary>打开一组StorageFile，并使用指定的回调函数处理该组文件</summary>
    public static async Task<IEnumerable<StorageFile>> GetOpenFilesAsync(this UIElement element, Action<StorageFile> setter) => await GetOpenFilesAsync(element, null, null);

    /// <summary>使用指定的筛选器打开一组StorageFile，并使用指定的回调函数处理该组文件</summary>
    public static async Task<IEnumerable<StorageFile>> GetOpenFilesAsync(this UIElement element, IEnumerable<string> filters, Action<IEnumerable<StorageFile>> setter) {
        var filters2 = filters ?? new[] { "*" };
        if (filters2.Count() == 0) { filters2 = new[] { "*" }; }
        var picker = new FileOpenPicker();
        foreach (var filter in filters2) {
            picker.FileTypeFilter.Add(filter);
        }
        var files = await picker.PickMultipleFilesAsync();
        if (files == null || files.Count == 0) { return null; }
        setter?.Invoke(files);
        return files;
    }

    /// <summary>保存一个StorageFile</summary>
    public static async Task<StorageFile> GetSaveFileAsync(this UIElement element) => await GetSaveFileAsync(element, Enumerable.Empty<string>(), null);

    /// <summary>使用指定的筛选器保存一个StorageFile</summary>
    public static async Task<StorageFile> GetSaveFileAsync(this UIElement element, IEnumerable<string> filters) => await GetSaveFileAsync(element, filters, null);

    /// <summary>保存一个StorageFile，并使用指定的回调函数处理该文件</summary>
    public static async Task<StorageFile> GetSaveFileAsync(this UIElement element, Action<StorageFile> setter) => await GetSaveFileAsync(element, Enumerable.Empty<string>(), null);

    /// <summary>使用指定的筛选器保存一个StorageFile，并使用指定的回调函数处理该文件</summary>
    public static async Task<StorageFile> GetSaveFileAsync(this UIElement element, IEnumerable<string> filters, Action<StorageFile> setter) {
        var picker = new FileSavePicker();
        if (filters != null && filters.Any()) {
            picker.FileTypeChoices.Add("文件", filters.ToList());
        }
        var file = await picker.PickSaveFileAsync();
        if (file == null) { return null; }
        setter?.Invoke(file);
        return file;
    }

    /// <summary>选择一个StorageFolder</summary>
    public static async Task<StorageFolder> GetFolderAsync(this UIElement element) => await GetFolderAsync(element, null);

    /// <summary>选择一个StorageFolder，并使用指定的回调函数处理该文件夹</summary>
    public static async Task<StorageFolder> GetFolderAsync(this UIElement element, Action<StorageFolder> setter) {
        var picker = new FolderPicker();
        var folder = await picker.PickSingleFolderAsync();
        if (folder == null) { return null; }
        setter?.Invoke(folder);
        return folder;
    }
}