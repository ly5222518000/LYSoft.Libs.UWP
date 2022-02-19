namespace LYSoft.Libs.UWP;

internal class ObjectEditorPage<T> : Page where T : new() {
    public T Instance { get; }
    public ObjectEditorPage() : this(new T()) { }
    public ObjectEditorPage(T instance) {
        Instance = instance;
        BuildPage();
    }

    private void BuildPage() {
        var panel = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Stretch, Padding = new(10) };
        var scrollviewer = new ScrollViewer() { Content = panel, MaxHeight = 700, MinWidth = 500 };
        Content = scrollviewer;

        var items = typeof(T)
            .GetProperties()
            .Select(prop => (prop, attr: prop.GetCustomAttribute<ObjectEditorPropertyAttribute>()))
            .Where(i => i.prop != null)
            .Where(i => i.prop.CanRead && i.prop.CanWrite)
            .OrderBy(i => i.attr.Index);

        foreach (var (prop, attr) in items) {
            var type = prop.PropertyType;
            if (type.IsEnum) {
                HandleEnumComboBox(panel, type, prop, attr);
            } else if (type == typeof(string)) {
                if (attr.Type == ObjectEditorType.Default || attr.Type == ObjectEditorType.Text) {
                    HandleTextBox(panel, prop, attr);
                } else if (attr.Type == ObjectEditorType.Password) {
                    HandlePasswordBox(panel, prop, attr);
                } else if (attr.Type == ObjectEditorType.Range) {
                    HandleTextComboBox(panel, prop, attr);
                } else if (attr.Type == ObjectEditorType.OpenFile) {
                    HandleTextOpenFile(panel, prop, attr);
                } else if (attr.Type == ObjectEditorType.SaveFile) {
                    HandleTextSaveFile(panel, prop, attr);
                } else if (attr.Type == ObjectEditorType.Folder) {
                    HandleTextFolder(panel, prop, attr);
                }
            } else if (type == typeof(DateTime)) {
                if (attr.Type == ObjectEditorType.Default || attr.Type == ObjectEditorType.Date) {
                    HandleDatePicker(panel, prop, attr);
                } else if (attr.Type == ObjectEditorType.Time) {
                    HandleTimePicker(panel, prop, attr);
                }
            } else if (type == typeof(StorageFile)) {
                if (attr.Type == ObjectEditorType.Default || attr.Type == ObjectEditorType.OpenFile) {
                    HandleStorageFileOpenFile(panel, prop, attr);
                } else if (attr.Type == ObjectEditorType.SaveFile) {
                    HandleStorageFileSaveFile(panel, prop, attr);
                }
            } else if (type == typeof(StorageFolder)) {
                HandleStorageFolder(panel, prop, attr);
            } else if (type == typeof(IStorageItem)) {
                if (attr.Type == ObjectEditorType.Default || attr.Type == ObjectEditorType.OpenFile) {
                    HandleStorageFileOpenFile(panel, prop, attr);
                } else if (attr.Type == ObjectEditorType.SaveFile) {
                    HandleStorageFileSaveFile(panel, prop, attr);
                } else if (attr.Type == ObjectEditorType.Folder) {
                    HandleStorageFolder(panel, prop, attr);
                }
            } else if (type == typeof(double) || type == typeof(int) || type == typeof(short) || type == typeof(long) || type == typeof(byte) || type == typeof(float) || type == typeof(decimal)) {
                HandleNumberBox(panel, prop, attr);
            }
        }
    }

    private void HandleNumberBox(StackPanel panel, PropertyInfo prop, ObjectEditorPropertyAttribute attr) {
        var numberbox = new NumberBox() { HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new(0, 10, 0, 0), Header = attr.Header ?? prop.Name, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact, AcceptsExpression = true };
        var binding = new Binding() { Source = Instance, Path = new(prop.Name), Mode = BindingMode.TwoWay };
        numberbox.SetBinding(NumberBox.ValueProperty, binding);
        numberbox.LostFocus += (_, _) => NumberBox_LostFocus(numberbox, attr);
        panel.Children.Add(numberbox);
    }

    private void NumberBox_LostFocus(NumberBox numberbox, ObjectEditorPropertyAttribute attr) {
        var isfail = (attr.MaxValue != double.NaN && attr.MaxValue < numberbox.Value) || (attr.MinValue != double.NaN && attr.MinValue > numberbox.Value);
        if (isfail) {
            var tip = numberbox.Tip(attr.FailTip, FlyoutPlacementMode.Bottom);
            if (tip == null) {
                numberbox.Focus(FocusState.Pointer);
            } else {
                tip.Closed += (_, _) => numberbox.Focus(FocusState.Pointer);
            }
        }
    }

    private void HandleStorageFolder(StackPanel panel, PropertyInfo prop, ObjectEditorPropertyAttribute attr) {
        var (button, textbox) = HandleStorageItemPicker(panel, prop, attr);
        button.Click += async (_, _) => await button.GetFolderAsync(f => { prop.SetValue(Instance, f); textbox.Text = f.Path; });
    }

    private void HandleStorageFileSaveFile(StackPanel panel, PropertyInfo prop, ObjectEditorPropertyAttribute attr) {
        var (button, textbox) = HandleStorageItemPicker(panel, prop, attr);
        button.Click += async (_, _) => await button.GetSaveFileAsync(attr.OptionsArray, f => { prop.SetValue(Instance, f); textbox.Text = f.Path; });
    }

    private void HandleStorageFileOpenFile(StackPanel panel, PropertyInfo prop, ObjectEditorPropertyAttribute attr) {
        var (button, textbox) = HandleStorageItemPicker(panel, prop, attr);
        button.Click += async (_, _) => await button.GetOpenFileAsync(attr.OptionsArray, f => { prop.SetValue(Instance, f); textbox.Text = f.Path; });
    }

    private (Button, TextBox) HandleStorageItemPicker(StackPanel panel, PropertyInfo prop, ObjectEditorPropertyAttribute attr) {
        var textbox = new TextBox() { Header = attr.Header ?? prop.Name };
        var binding = new Binding() { Source = Instance, Path = new($"{prop.Name}.Path"), Mode = BindingMode.OneWay };
        textbox.SetBinding(TextBox.TextProperty, binding);
        var button = new Button() { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Height = 28, Margin = new(0, 0, 3, 2), Content = new SymbolIcon(Symbol.More) };
        var grid = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new(0, 10, 0, 0) };
        grid.Children.Add(textbox);
        grid.Children.Add(button);
        panel.Children.Add(grid);
        return (button, textbox);
    }

    private void HandleTextFolder(StackPanel panel, PropertyInfo prop, ObjectEditorPropertyAttribute attr) {
        var (button, textbox) = HandleTextPicker(panel, prop, attr);
        button.Click += async (_, _) => { await button.GetFolderAsync(f => textbox.Text = f.Path); };
    }

    private void HandleTextSaveFile(StackPanel panel, PropertyInfo prop, ObjectEditorPropertyAttribute attr) {
        var (button, textbox) = HandleTextPicker(panel, prop, attr);
        button.Click += async (_, _) => { await button.GetSaveFileAsync(attr.OptionsArray, f => textbox.Text = f.Path); };
    }

    private void HandleTextOpenFile(StackPanel panel, PropertyInfo prop, ObjectEditorPropertyAttribute attr) {
        var (button, textbox) = HandleTextPicker(panel, prop, attr);
        button.Click += async (_, _) => { await button.GetOpenFileAsync(attr.OptionsArray, f => textbox.Text = f.Path); };
    }

    private (Button, TextBox) HandleTextPicker(StackPanel panel, PropertyInfo prop, ObjectEditorPropertyAttribute attr) {
        var textbox = new TextBox() { Header = attr.Header ?? prop.Name };
        var binding = new Binding() { Source = Instance, Path = new(prop.Name), Mode = BindingMode.TwoWay };
        textbox.SetBinding(TextBox.TextProperty, binding);
        var button = new Button() { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Height = 28, Margin = new(0, 0, 3, 2), Content = new SymbolIcon(Symbol.More) };
        var grid = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new(0, 10, 0, 0) };
        grid.Children.Add(textbox);
        grid.Children.Add(button);
        panel.Children.Add(grid);
        return (button, textbox);
    }

    private void HandleTimePicker(StackPanel panel, PropertyInfo prop, ObjectEditorPropertyAttribute attr) {
        var picker = new TimePicker() { HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new(0, 10, 0, 0), Header = attr.Header ?? prop.Name, ClockIdentifier = "24HourClock" };
        var binding = new Binding() { Source = Instance, Path = new(prop.Name), Mode = BindingMode.TwoWay };
        picker.SetBinding(TimePicker.TimeProperty, binding);
        picker.LostFocus += (_, _) => TimePicker_LostFocus(picker, attr);
        panel.Children.Add(picker);
    }

    private void TimePicker_LostFocus(TimePicker picker, ObjectEditorPropertyAttribute attr) {
        if (!Regex.IsMatch(picker.Time.ToString("HHmm"), attr.VerifyRegex)) {
            var tip = picker.Tip(attr.FailTip, FlyoutPlacementMode.Bottom);
            if (tip == null) {
                picker.Focus(FocusState.Pointer);
            } else {
                tip.Closed += (_, _) => picker.Focus(FocusState.Pointer);
            }
        }
    }

    private void HandleDatePicker(StackPanel panel, PropertyInfo prop, ObjectEditorPropertyAttribute attr) {
        var picker = new DatePicker() { HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new(0, 10, 0, 0), Header = attr.Header ?? prop.Name };
        var binding = new Binding() { Source = Instance, Path = new(prop.Name), Mode = BindingMode.TwoWay };
        picker.SetBinding(DatePicker.DateProperty, binding);
        picker.LostFocus += (_, _) => DatePicker_LostFocus(picker, attr);
        panel.Children.Add(picker);
    }

    private void DatePicker_LostFocus(DatePicker picker, ObjectEditorPropertyAttribute attr) {
        if (!Regex.IsMatch(picker.Date.ToString("yyyyMMdd"), attr.VerifyRegex)) {
            var tip = picker.Tip(attr.FailTip, FlyoutPlacementMode.Bottom);
            if (tip == null) {
                picker.Focus(FocusState.Pointer);
            } else {
                tip.Closed += (_, _) => picker.Focus(FocusState.Pointer);
            }
        }
    }

    private void HandlePasswordBox(StackPanel panel, PropertyInfo prop, ObjectEditorPropertyAttribute attr) {
        var passwordbox = new PasswordBox() { HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new(0, 10, 0, 0), Header = attr.Header ?? prop.Name };
        var binding = new Binding() { Source = Instance, Path = new(prop.Name), Mode = BindingMode.TwoWay };
        passwordbox.SetBinding(PasswordBox.PasswordProperty, binding);
        PasswordVerifyService.SetRegex(passwordbox, attr.VerifyRegex);
        PasswordVerifyService.SetAllowEmpty(passwordbox, attr.AllowEmpty);
        PasswordVerifyService.SetFailTip(passwordbox, attr.FailTip);
        panel.Children.Add(passwordbox);
    }

    private void HandleTextBox(StackPanel panel, PropertyInfo prop, ObjectEditorPropertyAttribute attr) {
        var textbox = new TextBox() { HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new(0, 10, 0, 0), Header = attr.Header ?? prop.Name };
        var binding = new Binding() { Source = Instance, Path = new(prop.Name), Mode = BindingMode.TwoWay };
        textbox.SetBinding(TextBox.TextProperty, binding);
        TextVerifyService.SetRegex(textbox, attr.VerifyRegex);
        TextVerifyService.SetAllowEmpty(textbox, attr.AllowEmpty);
        TextVerifyService.SetFailTip(textbox, attr.FailTip);
        panel.Children.Add(textbox);
    }

    private void HandleTextComboBox(StackPanel panel, PropertyInfo prop, ObjectEditorPropertyAttribute attr) {
        var combo = new ComboBox() { HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new(0, 10, 0, 0), Header = attr.Header ?? prop.Name };
        var binding = new Binding() { Source = Instance, Path = new(prop.Name), Mode = BindingMode.TwoWay };
        combo.SetBinding(Selector.SelectedItemProperty, binding);
        foreach (var v in attr.OptionsArray) { combo.Items.Add(v); }
        combo.SelectedItem = combo.Items.FirstOrDefault(i => i == prop.GetValue(Instance));
        panel.Children.Add(combo);
    }

    private void HandleEnumComboBox(StackPanel panel, Type type, PropertyInfo prop, ObjectEditorPropertyAttribute attr) {
        var combo = new ComboBox() { HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new(0, 10, 0, 0), Header = attr.Header ?? prop.Name };
        var binding = new Binding() { Source = Instance, Path = new(prop.Name), Mode = BindingMode.TwoWay };
        combo.SetBinding(Selector.SelectedItemProperty, binding);
        foreach (var v in Enum.GetValues(type)) { combo.Items.Add(v); }
        combo.SelectedItem = combo.Items.FirstOrDefault(i => (int)i == (int)prop.GetValue(Instance));
        panel.Children.Add(combo);
    }
}

/// <summary>对象编辑器属性特性</summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ObjectEditorPropertyAttribute : Attribute {
    /// <summary>显示的顺序</summary>
    public int Index { get; set; } = 0;
    /// <summary>显示的标题</summary>
    public string Header { get; set; }
    /// <summary>输入控件的类型</summary>
    public ObjectEditorType Type { get; set; } = ObjectEditorType.Default;
    /// <summary>选项列表，是一个以竖线分割的文本，当将<see cref="string"/>类型设置为组合框时会作为组合框的选项，文件类型时会作为筛选器</summary>
    public string Options { get; set; }
    /// <summary>验证文本的正则表达式，所有非文本类型会调用ToString()方法后进行比较，<see cref="DateTime"/>类型会转换为8位年月日日期/4位24小时制时间，文件类型不检查</summary>
    public string VerifyRegex { get; set; } = ".*";
    /// <summary>验证文本是否可为空</summary>
    public bool AllowEmpty { get; set; } = true;
    /// <summary>验证文本失败提示</summary>
    public string FailTip { get; set; } = "输入的内容不正确";
    /// <summary>验证数值类型最大值，只对数值类型有效</summary>
    public double MaxValue { get; set; } = double.NaN;
    /// <summary>验证数值类型最小值，只对数值类型有效</summary>
    public double MinValue { get; set; } = double.NaN;

    internal string[] OptionsArray { get => Options?.Split('|', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>(); }
}

/// <summary>对象编辑器类型特性</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ObjectEditorTypeAttribute : Attribute {

    /// <summary>对象编辑器标题</summary>
    public ObjectEditorTypeAttribute(string header) {
        Header = header;
    }
    /// <summary>显示的标题</summary>
    public string Header { get; }
}

/// <summary>对象编辑器属性显示类型</summary>
public enum ObjectEditorType {
    /// <summary>默认控件，支持所有类型</summary>
    Default,
    /// <summary>文本框，支持<see cref="string"/>类型，<see cref="string"/>默认</summary>
    Text,
    /// <summary>密码框，支持<see cref="string"/>类型</summary>
    Password,
    /// <summary>文件打开选择，支持<see cref="string"/>、<see cref="StorageFile"/>、<see cref="IStorageItem"/>类型，<see cref="StorageFile"/>、<see cref="IStorageItem"/>默认</summary>
    OpenFile,
    /// <summary>文件保存选择，支持<see cref="string"/>、<see cref="StorageFile"/>、<see cref="IStorageItem"/>类型</summary>
    SaveFile,
    /// <summary>文件夹选择，支持<see cref="string"/>、<see cref="StorageFolder"/>、<see cref="IStorageItem"/>类型，<see cref="StorageFolder"/>默认</summary>
    Folder,
    /// <summary>日期选择器，支持<see cref="DateTime"/>类型，默认</summary>
    Date,
    /// <summary>时间选择器，支持<see cref="DateTime"/>类型</summary>
    Time,
    /// <summary>数值输入框，支持数值类型，默认</summary>
    Number,
    /// <summary>组合框，支持<see cref="Enum"/>和<see cref="string"/>类型，<see cref="Enum"/>默认</summary>
    Range,
}

/// <summary>公共静态类，包含静态方法和扩展方法</summary>
public static partial class Common {

    /// <summary>使用<see cref="ContentDialog"/>创建并编辑一个指定类型的对象</summary>
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