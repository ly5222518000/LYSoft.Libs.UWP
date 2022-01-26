namespace LYSoft.Libs.UWP;

/// <summary>配置泛型接口</summary>
public interface IConfiguration<T> where T : new() {
    /// <summary>配置实例</summary>
    public T Instance { get; }
    /// <summary>持久化保存配置</summary>
    public IConfiguration<T> Save();
    /// <summary>异步持久化保存配置</summary>
    public Task<IConfiguration<T>> SaveAsync();
    /// <summary>使用新配置替换现有配置</summary>
    public IConfiguration<T> ReplaceWith(T value);
    /// <summary>从持久化项目中加载配置</summary>
    public IConfiguration<T> Load();
    /// <summary>从持久化项目中异步加载配置</summary>
    public Task<IConfiguration<T>> LoadAsync();
}