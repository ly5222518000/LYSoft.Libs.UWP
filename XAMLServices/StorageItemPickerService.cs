namespace LYSoft.Libs.UWP;

public static class StorageItemPickerService {
    public static readonly DependencyProperty PickerTypeProperty = DependencyProperty.Register("PickerType", typeof(StorageItemPickerType), typeof(StorageItemPickerService), new(StorageItemPickerType.None));
    public static void SetPickerType(Button button, StorageItemPickerType value) { button.SetValue(PickerTypeProperty, value); button.Click += (_, _) => Click(button); }
    public static StorageItemPickerType GetPickerType(Button button) => (StorageItemPickerType)button.GetValue(PickerTypeProperty);

    public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(StorageItemPickerService), new(null));
    public static void SetFilter(Button button, string value) => button.SetValue(FilterProperty, value);
    public static string GetFilter(Button button) => (string)button.GetValue(FilterProperty);

    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable<IStorageItem>), typeof(StorageItemPickerService), new(null));
    public static void SetItems(Button button, IEnumerable<IStorageItem> value) => button.SetValue(ItemsProperty, value);
    public static IEnumerable<IStorageItem> GetItems(Button button) => (IEnumerable<IStorageItem>)button.GetValue(ItemsProperty);

    public static readonly DependencyProperty FileProperty = DependencyProperty.Register("File", typeof(StorageFile), typeof(StorageItemPickerService), new(null));
    public static void SetFile(Button button, StorageFile value) => button.SetValue(FileProperty, value);
    public static StorageFile GetFile(Button button) => (StorageFile)button.GetValue(FileProperty);

    public static readonly DependencyProperty FilesProperty = DependencyProperty.Register("Files", typeof(IEnumerable<StorageFile>), typeof(StorageItemPickerService), new(null));
    public static void SetFiles(Button button, IEnumerable<StorageFile> value) => button.SetValue(FilesProperty, value);
    public static IEnumerable<StorageFile> GetFiles(Button button) => (IEnumerable<StorageFile>)button.GetValue(FilesProperty);

    public static readonly DependencyProperty FolderProperty = DependencyProperty.Register("Folder", typeof(StorageFolder), typeof(StorageItemPickerService), new(null));
    public static void SetFolder(Button button, StorageFolder value) => button.SetValue(FolderProperty, value);
    public static StorageFolder GetFolder(Button button) => (StorageFolder)button.GetValue(FolderProperty);

    public static readonly DependencyProperty CallBackProperty = DependencyProperty.Register("CallBack", typeof(Action<IEnumerable<IStorageItem>>), typeof(StorageItemPickerService), new(null));
    public static void SetCallBack(Button button, Action<IEnumerable<IStorageItem>> value) => button.SetValue(CallBackProperty, value);
    public static Action<IEnumerable<IStorageItem>> GetCallBack(Button button) => (Action<IEnumerable<IStorageItem>>)button.GetValue(CallBackProperty);

    private static async void Click(Button button) {
        var filters = GetFilter(button)?.Split(";").Select(f => f.Trim()).ToArray();
        var type = GetPickerType(button);
        GetCallBack(button)?.Invoke(type switch {
            StorageItemPickerType.OpenFile => Enumerable.Repeat((IStorageItem)await button.GetOpenFileAsync(), 1),
            StorageItemPickerType.OpenFiles => (await button.GetOpenFilesAsync()).Cast<IStorageItem>(),
            StorageItemPickerType.SaveFile => Enumerable.Repeat((IStorageItem)await button.GetSaveFileAsync(), 1),
            StorageItemPickerType.Folder => Enumerable.Repeat((IStorageItem)await button.GetFolderAsync(), 1),
            _ => null,
        });

        switch (type) {
            case StorageItemPickerType.OpenFile:
                var openfile = await button.GetOpenFileAsync(filters);
                if (openfile != null) { SetFile(button, openfile); }
                break;
            case StorageItemPickerType.OpenFiles:
                var openfiles = await button.GetOpenFilesAsync(filters);
                if (openfiles != null && openfiles.Any()) { SetFiles(button, openfiles); }
                break;
            case StorageItemPickerType.SaveFile:
                var savefile = await button.GetSaveFileAsync(filters);
                if (savefile != null) { SetFile(button, savefile); }
                break;
            case StorageItemPickerType.Folder:
                var folder = await button.GetFolderAsync();
                if (folder != null) { SetFolder(button, folder); }
                break;
            default: break;
        }
    }
}

public enum StorageItemPickerType {
    None,
    OpenFile,
    OpenFiles,
    SaveFile,
    Folder,
}