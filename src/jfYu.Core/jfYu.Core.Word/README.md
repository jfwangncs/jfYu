

### word工具

```
Install-Package jfYu.Core.Word
```

可自由生成word文件，也可以根据模板生成word文件，可加入图片文件。模板标记为${name}
```
var ContainerBuilder = new ContainerBuilder();
ContainerBuilder.AddJfYuWord();
var c = ContainerBuilder.Build();
var ms = c.Resolve<jfYuWord>();   
var x = new System.Collections.Generic.Dictionary<string, object>
{
    { "x", "测试哦" }
};

FileStream fs = new FileStream("d:/1.jpg", FileMode.Open);
x.Add("y", fs);
ms.GenerateWordByTemplate("d:/1.docx", x, "d:/2.docx");

```

