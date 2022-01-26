# LYSoft.Libs.UWP
LYSoft.Libs.UWP 扩展库

包括文件选择器扩展，对话框扩展，异步方法扩展，窗口扩展等
```c
   //异步扩展方法
   var result = Extensions.RunSync(AsyncMethod);
   //or
   var result = Extensions.RunSync(() => AsyncMethod());
   //or
   var m = AsyncMethod; 
   var result = m.RunSync();
```
包括配置类和配置接口，目前只有json文件配置类，可以使用加密的配置文件防止篡改
```c
   var file = ApplicatrionData.Current.LocalFolder.EnsureFile("config.json");
   var config = file.AsProtectedJsonConfigurationFile<T>();
   //or
   var config = file.AsJsonConfigurationFile<T>();
```
包括根据指定类型对象自动生成对象编辑器的功能，仅支持常见基本类型、文件(夹)类型和枚举类型
```c
//类型声明，需要编辑的属性需要有ObjectEditorPropertyAttribute特性，更多用法详见XML注释
[ObjectEditorType("编辑个人信息")]
public class Class1 : INotifyPropertyChanged {
   private StorageFile file;

   [ObjectEditorProperty(Index = 0, Header = "方向")]
   public BindingDirection BindingDirection { get; set; } = BindingDirection.TwoWay;

   [ObjectEditorProperty(Index = -10, Header = "姓名")]
   public string Name { get; set; } = "张三";

   [ObjectEditorProperty(Index = -10, Header = "密码", VerifyRegex = @"^.{6,16}$", FailTip = "密码应为6-16位", Type = ObjectEditorType.Password)]
   public string Password { get; set; } = "password";

   [ObjectEditorProperty(Index = 1, Header = "文件", Type = ObjectEditorType.SaveFile, Options = ".jpg|.png")]
   public StorageFile File { get => file; set { file = value; PropertyChanged?.Invoke(this, new(nameof(File))); }  }

   [ObjectEditorProperty(Index = 2, Header = "年龄", Type = ObjectEditorType.SaveFile, Options = ".jpg|.png")]
   public int Age { get; set; } = 80;

   public event PropertyChangedEventHandler PropertyChanged;

   public override string ToString() {
      return $"枚举: {BindingDirection}\n字符串: {Name}\n密码: {Password}\n文件: {File?.Path}\n数值: {Age}";
   }
}

//调用方式
var obj = new Class1();

await obj.EditAsync();
await Common.EditAsync<T>();
//or
await obj.EditInNewWindowAsync();
await Common.EditInNewWindowAsync<T>();
```
包括大量使用DependencyProperty的Service
```c
   <Page 
      ...
      xmlns:ly="using:LYSoft.Libs.UWP"
      ly:WindowStyleService.TitleBarMode="NoTitleBar"
      ly:WindowStyleService.ViewMode="Normal">
      
      <Grid>
         <StackPanel Margin="50" MaxWidth="700" HorizontalAlignment="Stretch">
            <TextBlock Margin="0,0,0,30" x:Name="titlebar" Text="用户登录" Style="{StaticResource TitleTextBlockStyle}"/>
            <TextBox
                x:Name="text"
                ly:TextVerifyService.Type="Email,IDCard,Phone"
                ly:TextVerifyService.FailTip="{x:Bind text.PlaceholderText}"
                PlaceholderText="请输入邮箱、身份证或手机号码"
                Text="{x:Bind UserName, Mode=TwoWay}"
                Margin="0,10,0,0" Header="用户名" AcceptsReturn="True" TextWrapping="Wrap"/>
            <PasswordBox
                x:Name="password"
                Margin="0,10,0,0"
                Header="密码"
                Password="{x:Bind Password, Mode=TwoWay}"
                ly:PasswordVerifyService.Regex="^.{6,16}$"
                ly:PasswordVerifyService.FailTip="{x:Bind password.PlaceholderText}"
                PlaceholderText="请输入6-16位任意字符"/>
            <Button
                Margin="0,10,0,0"
                ly:ZIndexService.ZIndex="32"
                Content="登录"
                Click="Login"/>
        </StackPanel>
      </Grid>
      
   </Page>
```
