namespace LYSoft.Libs.UWP;

public static class StorageItemDropService {

    private static Dictionary<UIElement, ElementDropInfo> dic = new();

    public static readonly DependencyProperty DropTypeProperty = DependencyProperty.Register("DropType", typeof(StorageItemDropType), typeof(StorageItemDropService), new(StorageItemDropType.None));
    public static void SetDropType(UIElement element, StorageItemDropType value) { element.SetValue(DropTypeProperty, value); element.SetInfoEvents(); }
    public static StorageItemDropType GetDropType(UIElement element) => (StorageItemDropType)element.GetValue(DropTypeProperty);

    public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(StorageItemDropService), new(null));
    public static void SetFilter(UIElement element, string value) => element.SetValue(FilterProperty, value);
    public static string GetFilter(UIElement element) => (string)element.GetValue(FilterProperty);

    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable<IStorageItem>), typeof(StorageItemDropService), new(null));
    public static void SetItems(UIElement element, IEnumerable<IStorageItem> value) => element.SetValue(ItemsProperty, value);
    public static IEnumerable<IStorageItem> GetItems(UIElement element) => (IEnumerable<IStorageItem>)element.GetValue(ItemsProperty);

    public static readonly DependencyProperty FileProperty = DependencyProperty.Register("File", typeof(StorageFile), typeof(StorageItemDropService), new(null));
    public static void SetFile(UIElement element, StorageFile value) => element.SetValue(FileProperty, value);
    public static StorageFile GetFile(UIElement element) => (StorageFile)element.GetValue(FileProperty);

    public static readonly DependencyProperty FilesProperty = DependencyProperty.Register("Files", typeof(IEnumerable<StorageFile>), typeof(StorageItemDropService), new(null));
    public static void SetFiles(UIElement element, IEnumerable<StorageFile> value) => element.SetValue(FilesProperty, value);
    public static IEnumerable<StorageFile> GetFiles(UIElement element) => (IEnumerable<StorageFile>)element.GetValue(FilesProperty);

    public static readonly DependencyProperty FolderProperty = DependencyProperty.Register("Folder", typeof(StorageFolder), typeof(StorageItemDropService), new(null));
    public static void SetFolder(UIElement element, StorageFolder value) => element.SetValue(FolderProperty, value);
    public static StorageFolder GetFolder(UIElement element) => (StorageFolder)element.GetValue(FolderProperty);

    public static readonly DependencyProperty FoldersProperty = DependencyProperty.Register("Folders", typeof(IEnumerable<StorageFolder>), typeof(StorageItemDropService), new(null));
    public static void SetFolders(UIElement element, IEnumerable<StorageFolder> value) => element.SetValue(FoldersProperty, value);
    public static IEnumerable<StorageFolder> GetFolders(UIElement element) => (IEnumerable<StorageFolder>)element.GetValue(FoldersProperty);

    public static readonly DependencyProperty CallBackProperty = DependencyProperty.Register("CallBack", typeof(Action<IEnumerable<IStorageItem>>), typeof(StorageItemDropService), new(null));
    public static void SetCallBack(UIElement element, Action<IEnumerable<IStorageItem>> value) => element.SetValue(CallBackProperty, value);
    public static Action<IEnumerable<IStorageItem>> GetCallBack(UIElement element) => (Action<IEnumerable<IStorageItem>>)element.GetValue(CallBackProperty);

    private static void Drop(object sender, DragEventArgs args) {
        var deferral = args.GetDeferral();
        var element = sender as UIElement;
        var info = dic[element];
        SetItems(element, info.Items);
        switch (GetDropType(element)) {
            case StorageItemDropType.File: SetFile(element, (StorageFile)info.Items.Single()); break;
            case StorageItemDropType.Files: SetFiles(element, info.Items.Cast<StorageFile>()); break;
            case StorageItemDropType.Folder: SetFolder(element, (StorageFolder)info.Items.Single()); break;
            case StorageItemDropType.Folders: SetFolders(element, info.Items.Cast<StorageFolder>()); break;
            default: break;
        }
        var callback = (Action<IEnumerable<IStorageItem>>)element.GetValue(CallBackProperty);
        callback?.Invoke(info.Items);
        deferral.Complete();
    }

    private static async void DragEnter(object sender, DragEventArgs args) {
        var deferral = args.GetDeferral();
        var element = sender as UIElement;
        var info = dic[element];
        try {
            if (args.DataView.Contains(StandardDataFormats.StorageItems)) {
                var items = await args.DataView.GetStorageItemsAsync();
                var filters = GetFilter(element)?.Split(";").Select(f => f.Trim().Trim('.').ToLower()) ?? new string[] { };
                filters = filters.Any(f => f == "*") ? new string[] { } : filters.Select(f => $".{f}");
                switch (GetDropType(element)) {
                    case StorageItemDropType.File:
                        var file = filters.Count() switch {
                            0 => items.FirstOrDefault(i => i.IsOfType(StorageItemTypes.File)),
                            _ => items.FirstOrDefault(i => i.IsOfType(StorageItemTypes.File) && filters.Any(f => i.Path.ToLower().EndsWith(f))),
                        };
                        info.Items = new[] { file };
                        if (file == null) {
                            info.CanDrop = false;
                        } else {
                            info.CanDrop = true;
                            info.Caption = file.Name;
                        }
                        break;
                    case StorageItemDropType.Files:
                        var files = filters.Count() switch {
                            0 => items.Where(i => i.IsOfType(StorageItemTypes.File)),
                            _ => items.Where(i => i.IsOfType(StorageItemTypes.File) && filters.Any(f => i.Path.ToLower().EndsWith(f))),
                        };
                        switch (files.Count()) {
                            case 0: info.CanDrop = false; break;
                            case 1: info.CanDrop = true; info.Caption = files.First().Name; break;
                            default: info.CanDrop = true; info.Caption = $"{files.First().Name} 等{files.Count()}个文件"; break;
                        }
                        info.Items = files;
                        break;
                    case StorageItemDropType.Folder:
                        var folder = items.FirstOrDefault(i => i.IsOfType(StorageItemTypes.Folder));
                        if (folder == null) {
                            info.CanDrop = false;
                        } else {
                            info.CanDrop = true;
                            info.Caption = folder.Name;
                            info.Items = new[] { folder };
                        }
                        break;
                    case StorageItemDropType.Folders:
                        var folders = items.Where(i => i.IsOfType(StorageItemTypes.Folder));
                        switch (folders.Count()) {
                            case 0: info.CanDrop = false; break;
                            case 1: info.CanDrop = true; info.Caption = folders.First().Name; break;
                            default: info.CanDrop = true; info.Caption = $"{folders.First().Name} 等{folders.Count()}个文件夹"; break;
                        }
                        info.Items = folders;
                        break;
                    case StorageItemDropType.None:
                        info.CanDrop = false;
                        break;
                }
            } else {
                info.CanDrop = false;
            }
        } catch (Exception ex) {
            var a = ex;
        }
        deferral.Complete();
    }

    private static void DragOver(object sender, DragEventArgs args) {
        var info = dic[sender as UIElement];
        if (info.CanDrop) {
            args.AcceptedOperation = DataPackageOperation.Copy;
            args.DragUIOverride.Caption = info.Caption;
            args.DragUIOverride.IsCaptionVisible = true;
            args.DragUIOverride.IsContentVisible = true;
            args.DragUIOverride.IsGlyphVisible = false;
        } else {
            args.AcceptedOperation = DataPackageOperation.None;
        }
        args.Handled = true;
    }

    private static void SetInfoEvents(this UIElement element) {
        var info = dic.ContainsKey(element) ? dic[element] : dic[element] = new();
        element.Drop += Drop;
        element.DragOver += DragOver;
        element.DragEnter += DragEnter;
    }

    private class ElementDropInfo {
        public IEnumerable<IStorageItem> Items { get; set; }
        public bool CanDrop { get; set; } = false;
        public string Caption { get; set; } = "";
    }
}

public enum StorageItemDropType {
    None,
    File,
    Files,
    Folder,
    Folders,
}