namespace LYSoft.Libs.UWP;

public static partial class Common {

    /// <summary>在指定文件夹中创建或获取指定名称的文件</summary>
    public static StorageFile EnsureFile(this StorageFolder folder, string name) => RunSync(() => folder.EnsureFileAsync(name));

    /// <summary>在指定文件夹中创建或获取指定名称的文件</summary>
    public static StorageFolder EnsureFolder(this StorageFolder folder, string name) => RunSync(() => folder.EnsureFolderAsync(name));

    /// <summary>在指定文件夹中异步创建或获取指定名称的文件</summary>
    public static async Task<StorageFile> EnsureFileAsync(this StorageFolder folder, string name) {
        return await folder.CreateFileAsync(name, CreationCollisionOption.OpenIfExists);
    }

    /// <summary>在指定文件夹中异步创建或获取指定名称的文件夹</summary>
    public static async Task<StorageFolder> EnsureFolderAsync(this StorageFolder folder, string name) {
        return await folder.CreateFolderAsync(name, CreationCollisionOption.OpenIfExists);
    }

}