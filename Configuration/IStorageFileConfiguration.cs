namespace LYSoft.Libs.UWP;

/// <summary>文件配置泛型接口</summary>
public interface IStorageFileConfiguration<T>: IConfiguration<T> where T : new() {
    /// <summary>配置文件</summary>
    public StorageFile File { get; }
    /// <summary>使用新配置文件中的配置替换现有配置，会替换文件路径，不会自动保存</summary>
    public IStorageFileConfiguration<T> ReplaceWith(StorageFile file);
    /// <summary>加载指定文件中的配置，但不替换文件路径，也不会自动保存</summary>
    public Task<IStorageFileConfiguration<T>> LoadAsync(StorageFile file);
    /// <summary>将配置保存到指定文件中，但不替换文件路径</summary>
    public Task<IStorageFileConfiguration<T>> SaveAsync(StorageFile file);
}