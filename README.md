# LYSoft.Libs.UWP
LYSoft.Libs.UWP 扩展库

包括文件选择器扩展，对话框扩展，异步方法扩展，窗口扩展等

包括配置类和配置接口

包括根据指定类型对象自动生成对象编辑器的功能
```c
   await obj.EditAsync();
   await Common.EditAsync<T>();
   //or
   await obj.EditInNewWindowAsync();
   await Common.EditInNewWindowAsync<T>();
```
