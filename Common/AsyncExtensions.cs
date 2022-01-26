namespace LYSoft.Libs.UWP;

public static partial class Common {
    private static TaskFactory factory { get; } = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

    /// <summary>同步执行异步方法，并返回结果
    /// <br />用法1: var result = Extensions.RunSync(AsyncMethod);
    /// <br />用法2: var result = Extensions.RunSync(() => AsyncMethod());
    /// <br />用法3: var m = AsyncMethod; var result = AsyncMethod.RunSync();</summary>
    public static TResult RunSync<TResult>(this Func<Task<TResult>> func) {
        var cultureUi = CultureInfo.CurrentUICulture;
        var culture = CultureInfo.CurrentCulture;
        return factory.StartNew(() => {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = cultureUi;
            return func();
        }).Unwrap().GetAwaiter().GetResult();
    }

    /// <summary>同步执行异步方法
    /// <br />用法1: Extensions.RunSync(AsyncMethod);
    /// <br />用法2: Extensions.RunSync(() => AsyncMethod());
    /// <br />用法3: var m = AsyncMethod; AsyncMethod.RunSync();</summary>
    public static void RunSync(this Func<Task> func) {
        var cultureUi = CultureInfo.CurrentUICulture;
        var culture = CultureInfo.CurrentCulture;
        factory.StartNew(() => {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = cultureUi;
            return func();
        }).Unwrap().GetAwaiter().GetResult();
    }
}