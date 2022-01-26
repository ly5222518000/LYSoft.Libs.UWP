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
包括根据指定类型对象自动生成对象编辑器的功能
```c
   await obj.EditAsync();
   await Common.EditAsync<T>();
   //or
   await obj.EditInNewWindowAsync();
   await Common.EditInNewWindowAsync<T>();
```
