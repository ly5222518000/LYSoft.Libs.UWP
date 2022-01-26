namespace LYSoft.Libs.UWP;

internal class JsonStorageFileConfiguration<T> : IStorageFileConfiguration<T> where T : new() {

    public JsonStorageFileConfiguration(StorageFile file) {
        File = file;
        Load();
    }

    public StorageFile File { get; private set; }
    public T Instance { get; private set; }
    public IConfiguration<T> Load() => Common.RunSync(LoadAsync);
    public IConfiguration<T> Save() => Common.RunSync(SaveAsync);
    public async Task<IConfiguration<T>> LoadAsync() => await LoadAsync(File);
    public async Task<IConfiguration<T>> SaveAsync() => await SaveAsync(File);

    public async Task<IStorageFileConfiguration<T>> LoadAsync(StorageFile file) {
        try {
            var text = await FileIO.ReadTextAsync(file);
            Instance = JsonSerializer.Deserialize<T>(text);
        } catch (Exception) {
            Instance = new T();
        }
        return this;
    }

    public async Task<IStorageFileConfiguration<T>> SaveAsync(StorageFile file) {
        var text = JsonSerializer.Serialize(Instance);
        await FileIO.WriteTextAsync(file, text);
        return this;
    }

    public IConfiguration<T> ReplaceWith(T value) {
        Instance = value;
        return this;
    }

    public IStorageFileConfiguration<T> ReplaceWith(StorageFile file) {
        File = file;
        Load();
        return this;
    }
}

public static partial class Common {
    /// <summary>将指定文件作为Json配置文件</summary>
    public static IStorageFileConfiguration<T> AsJsonConfigurationFile<T>(this StorageFile file) where T : new() {
        return new JsonStorageFileConfiguration<T>(file);
    }
}