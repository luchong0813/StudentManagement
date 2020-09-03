# 视频教程
[从零开始学ASP.NET Core与Entity Framework Core ]("https://www.bilibili.com/video/BV1wb411W7aB?p=1")


# 托管设置
设置项目文件的`AspNetCoreHostingModel`属性
```
<PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <!--<AspNetCoreHostingModel>InProcess</AspNetCoreHostingModel>-->
    <AspNetCoreHostingModel>OutOfProcess</AspNetCoreHostingModel>
  </PropertyGroup>
  ```
+ InProcess：使用IIS服务器托管
+ OutOfProcess：使用自带Kestrel服务器托管（自宿主）

# ASP.NET Core中的配置源
+ `appsettings.json`，`appsettings.{xxxxx}.json` 不同环境下对应不同的托管环境
+ User secrets (用户机密)
+ Environment variables (环境变量)
+ Command-line argument (命令行参数)
    `donnet run MyKey="xxxxxx"`
  Tips：它们的关系是依次加载，逐层覆盖
  
# ASP.NET Core中间件
+ 可同时被访问和请求
+ 可以处理请求后，然后将请求传递给下一个中间件
+ 可以处理请求后，并使管道短路
+ 可以处理传出响应
+ 中间件是按照添加的顺序执行的

###### 通过在`Configure`中添加参数`ILogger<Startup> logger`引入ASP.Net Core自带的日志组件。
```
public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                context.Response.ContentType = "text/plain;charset=utf-8";

                logger.LogInformation("M1：传入请求");
                await next();
                logger.LogInformation("M1：传出响应");
            });

            app.Use(async (context, next) =>
            {
                context.Response.ContentType = "text/plain;charset=utf-8";

                logger.LogInformation("M2：传入请求");
                await next();
                logger.LogInformation("M2：传出响应");
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("M3：处理请求，并生成响应");

                logger.LogInformation("M3：处理请求，并生成响应");
            });
        }
```
###### 输出日志：（可以看到三个中间件的执行过程）
```
Microsoft.AspNetCore.Hosting.Internal.WebHost:Information: Request starting HTTP/2.0 GET https://localhost:44383/  
StudyManagement.Startup:Debug: M1: 传入请求
StudyManagement.Startup:Debug: M2: 传入请求
StudyManagement.Startup:Debug: M3: 处理请求，生成响应
StudyManagement.Startup:Debug: M2: 传出响应
StudyManagement.Startup:Debug: M1: 传出响应
Microsoft.AspNetCore.Hosting.Internal.WebHost:Information: Request finished in 52.8954ms 200 text/plain;charset=utf-8
StudyManagement.Startup:Debug: M1: 传入请求
StudyManagement.Startup:Debug: M2: 传入请求
StudyManagement.Startup:Debug: M3: 处理请求，生成响应
StudyManagement.Startup:Debug: M2: 传出响应
StudyManagement.Startup:Debug: M1: 传出响应
Microsoft.AspNetCore.Hosting.Internal.WebHost:Information: Request finished in 34.3387ms 200 text/plain;charset=utf-8
```
###### 请求处理管道的中 3 个非常重要的知识点：
+ 所有的请求都会在每个中间件组件调用next()方法之前触发。
+ 当中间件处理请求并产生响应时，请求处理流程在管道中开始反向传递。
+ 所有的响应都会在每个中间件组件调用next()方法之前触发。

###### 乱码问题：(设置字符类型)

`context.Response.ContentType = "text/plain; charset=utf-8";`

# 静态文件支持
所有静态文件都在目录`wwwroot`下
###### 首先
```
// 设置默认文件
// 不设置的话，默认就是index.html/default.html这几个
var defaultFileOpinions = new DefaultFilesOptions();
defaultFileOpinions.DefaultFileNames.Clear();
defaultFileOpinions.DefaultFileNames.Add("test.html");

// 添加默认文件中间件，必须在UseStaticFiles之前注册
app.UseDefaultFiles(defaultFileOpinions);

// 添加静态文件中间件
app.UseStaticFiles();
```
###### DirectoryBrowser 中间件
  可以在浏览器浏览`wwwroot`下的内容。不推荐在生产环境中使用。
  ```
  app.UseDirectoryBrowser();
  ```
  ###### FileServer 中间件
集成`UseDefaultFiles`, `UseStaticFiles`, `UseDirectoryBrowser`三个中间件的功能。同样不推荐在生产环境中使用。
```
var fileServerOpinions = new FileServerOptions();
fileServerOpinions.DefaultFilesOptions.DefaultFileNames.Clear();
fileServerOpinions.DefaultFilesOptions.DefaultFileNames.Add("test.html");

app.UseFileServer(fileServerOpinions);
```
# 开发者异常页面
###### 示例代码
```
if (env.IsDevelopment())
{
    var developerExceptionPageOptions = new DeveloperExceptionPageOptions();
    // 显示代码行数
    developerExceptionPageOptions.SourceCodeLineCount = 10;
    app.UseDeveloperExceptionPage();
}

app.Run(async (context) =>
{
    throw new Exception("自己抛出的异常");
});
```
 `UseDeveloperExceptionPage`中间件的位置尽量放置在前面，因为如果管道中的后面的中间件组件引发异常，它可以处理异常并显示Developer Exception页面。
 
# 开发环境变量
+ Development：开发环境
+ Staging：演示（模式，临时）环境
+ Production：正式（生产）环境

Ops：
+ 使用`ASPNETCORE_ENVIRONMENT`环境变量设置开发环境。
+ 在开发机上，在`launchSettings.json`文件中设置环境变量。
+ 在Staging和Production环境时，尽量在操作系统设置环境变量。
+ 使用`IHostEnvironment`服务访问运行时环境
+ 除了标准环境之外还支持自定义环境（UAT、QA等）
 ```
 //如果环境是Development serve Developer Exception Page
if（env.IsDevelopment()）
{
    app.UseDeveloperExceptionPage();
}
//else提供具有应用程序支持的用户友好错误页面联系信息
   else if (env.IsStaging() || env.IsProduction() || env.IsEnvironment("UAT")){
                app.UseExceptionHandler("/Error");
            }
```

# 引入MVC
1. 将MVC服务依赖注入到容器中

```
public void ConfigureServices(IServiceCollection services)
{
    // 单纯引入核心MVC服务，只有核心功能
    services.AddMvcCore();
    // 一般用这个，功能多
    services.AddMvc();
}
```
2. 添加MVC中间件到请求处理中

```
//添加路由中间件
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{Controller=Home}/{action=Index}/{id?}");
            });
  ```
  Tips:MVC路由规则：/控制器名称/方法名称，（不区分大小写）
  
#  初步了解模型和依赖注入
##### 定义模型

```
public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public string Email { get; set; }
    }
```
##### 定义接口

```
public interface IStudentRepository
    {
        Student GetStudent(int id);
    }
```
##### 实现接口（目前还没接入数据库，使用硬编码Mock）

```
public class MockStudentRepository : IStudentRepository
    {
        private List<Student> _students;

        public MockStudentRepository()
        {
            _students = new List<Student>()
            {
                new Student{ Id=1,Name="张三",ClassName="17计应4班",Email="84512211@outlook.com"},
                new Student{ Id=2,Name="李四",ClassName="17计应5班",Email="451515jshjd@outlook.com"},
                new Student{ Id=3,Name="王五",ClassName="17计应4班",Email="sghdha52@qq.com"},
                new Student{ Id=4,Name="赵六",ClassName="17计应6班",Email="45xshxdjn22@outlook.com"},
                new Student{ Id=5,Name="鲁班",ClassName="17计应5班",Email="dxshjc1515251@163.com"}
            };
        }


        public Student GetStudent(int id)
        {
            return _students.FirstOrDefault(s => s.Id == id);
        }
    }
```
##### 注册依赖注入
ASP.NET Core依赖注入容器注册服有三种
+ AddSingleton：全局单例
+ AddTransient：每次使用都创建新对象
+ AddScoped：每个http请求中创建和使用同一个对象

依赖注入的优点
+ 低耦合
+ 高测试性，更加方便进行单元测试

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    // 注册依赖注入，将实现类与接口绑定
    services.AddSingleton<IStudentRepository, MockStudentRepository>();
}
```
##### 控制器入门
+  返回json类型

```
public JsonResult Details()
        {
            Student model = _studentRepository.GetStudent(1);
            return Json(model);
        }
 ```
 + 内容格式协商

在控制器方法中使用`ObjectResult`返回类型，支持内容协商，根据请求头参数返回数据
```
// 支持内容格式协商
public ObjectResult Details(int id)
{
    return new ObjectResult(_studentRepository.GetById(id));
}
```

如：
`Accept: application/xml`
将返回xml格式。注：还要添加xml序列化器。
```
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc()
        // 注册XML序列化器
        .AddXmlSerializerFormatters();
}
```
#视图入门
###将数据从控制器传输到视图的方法
前两种都是弱类型
+ ViewData
+ ViewBag
+ 强类型视图

###### ViewData
+ 弱类型字典对象
+ 使用string类型的键值，存储和查询
+ 运行时动态解析
+ 没有智能感知，编译时也没有类型检查

使用方法：
```
ViewData["Title"] = "学生视图";
ViewData["Model"] = model;
```
cshtml代码：
```
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
</head>
<body>
    <h1>@ViewData["Title"]</h1>
    @{
        var student = ViewData["model"] as StudyManagement.Models.Student;
    }
    <div>姓名：@student.Name</div>
    <div>班级：@student.ClassName</div>
</body>
</html>
```
###### ViewBag
```
// 直接给动态属性赋值
ViewBag.PageTitle = "ViewBag标题";
ViewBag.Student = model;
```
cshtml使用：
```
<h1>@ViewBag.PageTitle</h1>
<div>姓名：@ViewBag.Student.Name</div>
<div>班级：@ViewBag.Student.ClassName</div>
```
###### 强类型视图
在控制器中传给View()模型
```
public IActionResult GetView()
{
    var model = _studentRepository.GetById(1);
    return View(model);
}
```
在cshtml中指定模型类型
```
@model StudyManagement.Models.Student
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
</head>
<body>
    <h1>强类型模型</h1>
    <ul>
        <li>@Model.Id</li>
        <li>@Model.Name</li>
        <li>@Model.ClassName</li>
        <li>@Model.Email</li>
    </ul>

</body>
</html>
```
### ViewModel 视图模型
类似于DTO（数据传输对象）
###### 定义ViewModel
```
public class StudentDetailsViewModel
{
    public Student Student { get; set; }
    public string PageTitle { get; set; }
}
```
###### 改造控制器
```
public IActionResult Details()
{
    var model = _studentRepository.GetById(1);
    var viewModel = new StudentDetailsViewModel
    {
        Student = model,
        PageTitle = "viewmodel里的页面标题"
    };
    return View(viewModel);
}
```
###### 在View中使用
```
<!-- 这里注册的模型改成了ViewModel了 -->
@model StudyManagement.ViewModels.StudentDetailsViewModel
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
</head>
<body>
    <h1>强类型模型</h1>
    <h2>@Model.PageTitle</h2>
    <ul>
        <li>@Model.Student.Id</li>
        <li>@Model.Student.Name</li>
        <li>@Model.Student.ClassName</li>
        <li>@Model.Student.Email</li>
    </ul>
</body>
</html>
```
### View中使用循环
```
@model IEnumerable<StudyManagement.Models.Student>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
</head>
<body>
    <table border="1">
        <tr>
            <td>Id</td>
            <td>姓名</td>
            <td>班级</td>
            <td>邮箱</td>
        </tr>
        @foreach (var student in Model)
        {
            <tr>
                <td>@student.Id</td>
                <td>@student.Name</td>
                <td>@student.ClassName</td>
                <td>@student.Email</td>
            </tr>
        }
    </table>
</body>
</html>
```
#  布局视图 LayoutView
### 创建布局视图
```
<!DOCTYPE html>

<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <title>@ViewBag.Title</title>
</head>
<body>
    <div>
        @RenderBody()
    </div>

    @RenderSection("Scripts", required: false)
</body>
</html>
```
### 渲染视图
```
@model IEnumerable<StudyManagement.Models.Student>
@{
    Layout = "~/Views/Shared/_Layout.cshtml";
    ViewBag.Title = "首页 学生列表";
}
<div></div>
```
### 视图节点 Section
+ 在布局视图里渲染节点
`@RenderSection("Scripts", required: false)`
+ 在普通视图里定义节点
```
@section Scripts{ 
    <script>
        document.write("hello");
    </script>
}
```
# 视图开始 ViewStart
我的理解就是`_ViewStart.cshtml`文件所在目录下的每个视图文件开始渲染先执行这个文件的内容。一般直接放在`Views`目录下，全局生效，可以放在各个子文件夹下，这样可以覆盖全局的`_ViewStart.cshtml`。
```
@{
    Layout = "_Layout";
}
```
# 视图导入 ViewImports
用来导入命名空间、注册模型等等n多种操作。
生效机制和ViewStart差不多。
# 路由
+ 常规路由

```
//添加路由中间件
app.UseRouting();
app.UseEndpoints(endpoints =>
{
        endpoints.MapControllerRoute(
             name: "default",
             pattern: "{Controller=Home}/{action=Index}/{id?}");
 });
```
+ 属性路由


比传统路由更加灵活，可以搭配传统路由使用。

即在控制器方法上添加路由注解，一个方法可以同时映射多个路由。
```
[Route("Home/Index")]
public IActionResult Index()
{
    return View(_studentRepository.GetAll());
}
```
路由中也可以指定参数
```
[Route("test/{id?}")]
public IActionResult Details(int id = 1)
{
    var model = _studentRepository.GetById(id);
    var viewModel = new StudentDetailsViewModel
    {
        Student = model,
        PageTitle = "viewmodel里的页面标题"
    };
    return View(viewModel);
}
```
可以直接在控制器类上加注解，[controller]/[action]。
# LibMan包管理工具
+ 使用方法：右键项目--添加--客户端库
+ libman.json 文件：libman.json 是库管理器清单文件。
请注意，在清单文件中，我们有一个刚刚安装的 Bootstrap 客户端库的列表。我们也可以直接编辑清单文件来安装客户端软件包，而不是使用 LibMan 提供的图形界面。
