namespace LYSoft.Libs.UWP;

internal class ProtectedJsonStorageFileConfiguration<T> : IStorageFileConfiguration<T> where T : new() {
    public StorageFile File { get; private set; }

    public ProtectedJsonStorageFileConfiguration(StorageFile file) {
        ReplaceWith(file);
    }

    public T Instance { get; private set; }

    public IConfiguration<T> Load() => Common.RunSync(LoadAsync);
    public IConfiguration<T> Save() => Common.RunSync(SaveAsync);
    public async Task<IConfiguration<T>> LoadAsync() => await LoadAsync(File);
    public async Task<IConfiguration<T>> SaveAsync() => await SaveAsync(File);
        
    public IConfiguration<T> ReplaceWith(T value) {
        Instance = value;
        return this;
    }

    public IStorageFileConfiguration<T> ReplaceWith(StorageFile file) {
        File = file;
        Load();
        return this;
    }

    public async Task<IStorageFileConfiguration<T>> LoadAsync(StorageFile file) {
        try {
            var buffer = await FileIO.ReadBufferAsync(file);
            var text = await buffer.UnprotectToStringAsync();
            Instance = JsonSerializer.Deserialize<T>(text);
        } catch (Exception) {
            Instance = new();
        }
        return this;
    }

    public async Task<IStorageFileConfiguration<T>> SaveAsync(StorageFile file) {
        var text = JsonSerializer.Serialize(Instance);
        var buffer = await text.ProtectAsync();
        await FileIO.WriteBufferAsync(file, buffer);
        return this;
    }
}

public static partial class Common {
    /// <summary>将指定文件作为受保护的Json配置文件</summary>
    public static IStorageFileConfiguration<T> AsProtectedJsonConfigurationFile<T>(this StorageFile file) where T : new() {
        return new ProtectedJsonStorageFileConfiguration<T>(file);
    }
}