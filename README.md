ASP.NET Core 学习笔记

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
# TagHelper
优点：根据参数自动生成，不需要手写超链接，类似Django模板里面的url命令。

在`ViewImport`中添加`TagHelper`
`@addTagHelper *,Microsoft.AspNetCore.Mvc.TagHelpers`
比如，链接TagHelper使用
```
<a asp-controller="Home" asp-action="details" asp-route-id="@student.Id" class="btn btn-primary">查看</a>
@*<a href="#" class="btn btn-primary">查看</a>*@
 ```
 ###### ImageTaghelper(缓存破坏)
 `<img class="card-img-top" src="~/images/46243214.jpg" width="50" asp-append-version="true" />`
 Image TagHelper 增强了< img >标签，为静态图像文件提供缓存破坏行为。将图像的内容，生成唯一的散列值并将其附加到图片的 URL。此唯一字符串会提示浏览器从服务器重新加载图片，而不是从浏览器缓存重新加载。
`<img class="card-img-top" src="/images/46243214.jpg?v=tL9eCId0npmRFYCfDFpRO9NRNK8EpIg3x20zfOQtVwM" width="50">`
###### 环境 TagHelper
在开发环境中使用本地css文件，在非开发环境下使用的是CDN的css文件。

注：`integrity`是用来做完整性检查的，保证CDN提供文件的完整和安全。
```
<environment include="Development">
        <link href="~/lib/twitter-bootstrap/css/bootstrap.css" rel="stylesheet" />
    </environment>

    <environment exclude="Development">
        <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css"
              integrity="sha384-JcKb8q3iqJ61gNV9KGb8thSsNjpSL0n8PARn9HuZOnIxN0hoP+VmmDGMN5t9UJ0Z" crossorigin="anonymous"
              asp-fallback-href="~/lib/twitter-bootstrap/css/bootstrap.css"
              asp-suppress-fallback-integrity="true">
    </environment>
 ```
 为了防止CDN加载失败页面无法显示，可以加上fallback相关属性，第一个是失败时加载的文件，第二个是不检查这个文件的完整性
```
asp-fallback-href="~/lib/twitter-bootstrap/css/bootstrap.css"
asp-suppress-fallback-integrity="true"
```
###### 表单 Tag Helper
直接贴上一个布局的代码，把class样式都去掉了，保留最基本代码。

确实是很方便的，和Django、jinja2之类的模板比完全不输。
```
@model Student

<form asp-controller="student" asp-action="create">
    <label asp-for="Name"></label>
    <input asp-for="Name" />

    <label asp-for="Email"></label>
    <input asp-for="Email" />

    <label asp-for="ClassName"></label>
    <select asp-for="ClassName" asp-items="Html.GetEnumSelectList<ClassNameEnum>()"></select>

    <button type="submit">提交</button>
</form>
```
# 模型绑定
将Http请求中的数据绑定到控制器方法上对应参数的顺序：
+ Form Values （Post表单数据）
+ Route Values （路由中的值）
+ Query String （Get的查询字符串）

# 模型验证
###### 1、设置模型
首先在Model中加入验证属性，如：
```
public int Id { get; set; }
        [Display(Name = "姓名")]
        [Required(ErrorMessage = "请输入姓名"), MaxLength(10, ErrorMessage = "姓名不能超过10位字符")]
        public string Name { get; set; }
        [Display(Name = "邮箱")]
        public ClassName ClassName { get; set; }
        [Display(Name = "邮箱")]
        [Required(ErrorMessage = "请输入邮箱"), RegularExpression(@"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$", ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; }
 ```
###### 常用的模型验证方法
+ `Required`：必填
+ `Range`：指定允许的最小值和最大值
+ `MinLength`：最小长度
+ `MaxLength`：最大长度
+ `Compare`：比较两个属性，比如密码和确认密码
+ `RegularExpression`：正则

###### 2、在控制器中加入验证代码
使用`ModelState.IsValid`来验证模型属性是否正确
```
[HttpPost]
        public IActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {
                var stu = _studentRepository.Add(student);
                return RedirectToAction("Details", new { id = stu.Id });
            }
            return View();
        }
```

###### 3、使用TagHelper在网页上显示错误信息
```
<div asp-validation-summary="All" class="text-danger"></div>
    <div class="form-group row">
        <label asp-for="Name" class="col-sm-2 col-form-label"></label>
        <div class="col-sm-10">
            <input asp-for="Name" class="form-control" placeholder="请输入姓名" />
            <span asp-validation-for="Name" class="text-danger"></span>
        </div>

    </div>
```
#  依赖注入
一表搞懂`Scoped`、`Transient`、`Singleton`三个依赖注入服务
|  服务类型   | 同一个HTTP请求的范围内  | 横跨多个不同的HTTP请求  |
|  ----  | ----  | ----  |
| Scoped（作用域）  | 同一个实例 | 新实例 |
| Transient（瞬时）  | 新实例 | 新实例 |
| Singleton（单例）  | 同一个实例 | 同一个实例 |

#  EF Core入门
###### 首先实现DbContext
```
public class AppDbContext:DbContext
{
    // 将应用程序的配置传递给DbContext
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // 对要使用到的每个实体都添加 DbSet<TEntity> 属性
    // 通过DbSet属性来进行增删改查操作
    // 对DbSet采用Linq查询的时候，EFCore自动将其转换为SQL语句
    public DbSet<Student> Students { get; set; }
}
```
###### 注册DbContext连接池
```
services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(_configuration.GetConnectionString("StudentDBConnection"))
                );
```
+ 其中，本地SqlServer数据库的配置，在`appserttings.json`中：

```
"ConnectionStrings": {
    "StudentDBConnection": "server=localhost;database=StudentDB;Trusted_Connection=true"
  }
```
+ vs自带的SQL资源管理器：

```
"ConnectionStrings": {
    "StudentDBConnection": "server=(localdb)\\MSSQLLocalDB;database=StudentDB;Trusted_Connection=true"
  }

```
###### 实现仓储
```
public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _DbContext;

        public StudentRepository(AppDbContext appDbContext)
        {
            _DbContext = appDbContext;
        }

        public Student Add(Student student)
        {
            _DbContext.Add(student);
            _DbContext.SaveChanges();
            return student;
        }

        public Student Delete(int id)
        {
            var student = _DbContext.students.Find(id);
            if (student != null)
            {
                _DbContext.students.Remove(student);
                _DbContext.SaveChanges();
            }
            return student;
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return _DbContext.students;
        }

        public Student GetStudent(int id)
        {
            return _DbContext.students.Find(id);
        }

        public Student Update(Student updateStudent)
        {
            var student = _DbContext.students.Attach(updateStudent);
            student.State = EntityState.Modified;
            _DbContext.SaveChanges();
            return updateStudent;
        }
    }
```
###### EF Core常用命令
+ Get-Help about_enti：显示帮助，`about_enti`全名很长可以只写前面的
+ Add-Migration：添加迁移记录
+ Update-Database：更新数据库

###### 添加种子数据
重写`DbContext`的`OnModelCreating`方法
```
protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.InsertSeedData();
        }
```
为了避免`DbContext`代码太乱，也可以使用扩展方法的方式：
```
public static void InsertSeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, Name = "张三", ClassName = ClassName.ClassFour, Email = "84512211@outlook.com" },
                new Student { Id = 2, Name = "李四", ClassName = ClassName.ClassSix, Email = "451515jshjd@outlook.com" },
                new Student { Id = 3, Name = "王五", ClassName = ClassName.ClassFour, Email = "sghdha52@qq.com" },
                new Student { Id = 4, Name = "赵六", ClassName = ClassName.ClassSix, Email = "45xshxdjn22@outlook.com" },
                new Student { Id = 5, Name = "鲁班", ClassName = ClassName.ClassFive, Email = "dxshjc1515251@163.com" });
        }
```
###### 领域模型与数据库架构
+ 使用迁移功能同步领域模型和数据库架构
+ 使用`add-migration`添加迁移记录
+ 使用`remove-migration`删除最近一条记录
+ 使用`update-database`迁移记录名称 可以回滚至任意一次迁移

# 文件上传
###### 定义ViewModel
要上传的字段采用`IFormFile`类型
```
public class StudentCreateViewModel
    {
        public int Id { get; set; }
        ....

        [Display(Name = "头像")]
        public IFormFile Photo { get; set; }
    }
```
###### 编写视图
修改cshtml视图文件，修改模型绑定：
`@model StudentCreateViewModel`
加入上传文件的表单项
```
<div class="form-group row">
        <label asp-for="Photo" class="col-sm-2 col-form-label"></label>
        <div class="col-sm-10">
            <div class="custom-file">
                <input asp-for="Photo" class="form-control custom-file-input" />
                <label class="custom-file-label">请选择头像...</label>
            </div>
        </div>
    </div>
```
为了选择文件后能显示出文件名还要编写js：
```
@section Scripts{
        <script type="text/javascript">
            $(document).ready(function () {
                $(".custom-file-input").on("change", function () {
                    var fileName = $(this).val().split("\\").pop();
                    $(this).next(".custom-file-label").html(fileName);
                })
            })
        </script>
    }
```
###### 编写控制器
通过构造函数注入`WebHostEnvironment`
```
public HomeController(IStudentRepository studentRepository, IWebHostEnvironment webHostEnvironment)
        {
            _studentRepository = studentRepository;
            this.webHostEnvironment = webHostEnvironment;
        }
```
处理文件上传和保存的逻辑
```
[HttpPost]
        public IActionResult Create(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.Photo != null)
                {
                    //获取上传头像存放的路径
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                    //生成唯一的文件名
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                Student newStudent = new Student()
                {
                    Name = model.Name,
                    Email = model.Email,
                    ClassName = model.ClassName,
                    Photo = uniqueFileName
                };

                _studentRepository.Add(newStudent);
                return RedirectToAction("Details", new { id = newStudent.Id });
            }
            return View();
        }
```
###### 多文件上传
和单文件差不多，其主要修改地方如下：
+ 在ViewModel中将属性改为`public List<IFormFile> Photos { get; set; }`
+ 在视图代码上的`input`中加入`multiple`以此来支持多文件
+ 更改JavaScript代码
```
var fileLable = $(this).next(".custom-file-label");
                    var files = $(this)[0].files;
                    if (files.length > 1) {
                        fileLable.html("您已经选择了：" + files.length + "张图片");
                    } else {
                        fileLable.html(files[0].name);
                    }
```
+ 修改控制器操作方法,使用forech遍历
```
if (model.Photos != null && model.Photos.Count() > 1)
                {
                    foreach (var photo in model.Photos)
                    {
                        //获取上传头像存放的路径
                        string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                        //生成唯一的文件名s
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                        string filePath = Path.Combine(uploadFolder, uniqueFileName);

                        photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                }
```
# 编辑学生信息
###### 编辑视图模型
因为所用的属性大致与添加时一致，所以直接继承
```
public class StudentEditViewModel : StudentCreateViewModel
    {
        public int Id { get; set; }
        public string ExistPhontPath { get; set; }
    }
```
###### 视图Action
+ 先通过学生ID查询学生详细信息
+ 通过实例化的`StudentEditViewModel`将信息传递给视图
```
[HttpGet]
        public ViewResult Edit(int id)
        {
            Student student = _studentRepository.GetStudent(id);

            StudentEditViewModel studentEditViewModel = new StudentEditViewModel()
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                ClassName = student.ClassName,
                ExistPhontPath = student.Photo
            };

            return View(studentEditViewModel);
        }
```
###### 编辑视图页面
基本与添加大致一样
```
//当我们提交表单更新数据库中的数据时，可以通过以下两个隐藏的input存储
<input hidden asp-for="Id" />
<input hidden asp-for="ExistPhontPath"/>
```

###### 提交数据的Action
+ 检查提供的数据是否有效
+ 从数据库中查询正在编辑的学生信息
+ 用模型对象中的数据更新student对象
+ 如果用户上传了新的图片则删除服务器中原有的图片，再将新的图片保存进来
+ 最后调用仓储中的更新服务，然后返回到`Index`Action中

大致代码如下：
```
[HttpPost]
        public IActionResult Edit(StudentEditViewModel model)
        {
            //检查提供的数据是否有效
            if (ModelState.IsValid)
            {
                //从数据库查询正在编辑的学生信息
                Student student = _studentRepository.GetStudent(model.Id);

                student.Name = model.Name;
                student.Email = model.Email;
                student.ClassName = model.ClassName;

                if (model.Photo != null )
                {
                    if (model.ExistPhontPath != null)
                    {
                        string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", model.ExistPhontPath);

                        //因为用户又上传了新的图片，所以为了避免占用资源，直接删掉原来的
                        System.IO.File.Delete(filePath);
                    }

                    student.Photo = ProcessUploadFile(model);
                }
                Student updateStudent = _studentRepository.Update(student);
                return RedirectToAction("Index");
            }
            return View(model);
        }
```
###### 枚举的扩展方法实现
```
public static string GetDisplayClassName(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memberInfos = type.GetMember(en.ToString());
            if (memberInfos != null && memberInfos.Length > 0)
            {
                object[] attrs = memberInfos[0].GetCustomAttributes(typeof(DisplayAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DisplayAttribute)attrs[0]).Name;
                }
            }
            return en.ToString();
        }
```

# 404错误与异常拦截
### 找不到指定ID的异常
修改查看详情的控制器，如果通过EF Core查询的学生信息为null则跳转到错误视图
```
Student student = _studentRepository.GetStudent(id);

            if (student == null)
            {
                return View("StudentNotFound", id);
            }
```

### 统一处理404错误
###### 在`Startup.cs`中添加中间件
`app.UseStatusCodePagesWithRedirects("/Error/{0}");`

###### 编写错误操作控制器
```
[Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "少年,你走错路了吧！";
                    break;
            }
            return View("NotFound");
        }
```
### 设置全局异常跳转
添加中间件
`app.UseExceptionHandler("/Error");`
编写处理用的控制器，这里需要添加AllowAnonymous注解，允许用户在未登录的时候访问到这个异常页面，保证无论如何可以显示出异常页面。
```
[AllowAnonymous]
        [Route("Error")]
        public IActionResult Error()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ViewBag.Message = exception.Error.Message;
            ViewBag.Path = exception.Path;
            ViewBag.StackTrace = exception.Error.StackTrace;

            return View("Error");
        }
```
# 日志记录
AspNetCore里面自带了一套日志系统，默认已经注册到了服务容器里了，只要在控制器的构造函数里注入就可以使用了，比如：
```
public class ErrorController : Controller
{
    private ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        this._logger = logger;
    }
}
```
默认的日志只会记录到控制台或者调试输出，不过我们为了实现更多功能，比如记录到文件或者推送到日志服务器，我们需要使用第三方的日志组件。这里我用的是NLog。

首先要安装NLog.Web.AspNetCore这个包。

之后在Program.cs里引入nlog服务：
```
public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
    .ConfigureLogging((hostingContext, logging) =>
     {
         // 保留官方的代码中的默认日志程序
 logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
         logging.AddConsole();
         logging.AddDebug();
         logging.AddEventSourceLogger();
         // 引入 nlog
         logging.AddNLog();
      }).UseStartup<Startup>();
```
保留官方默认日志程序那里，要看AspNetCore的源代码

然后，为了使用nlog，需要创建一个配置文件，在项目根目录创建 NLog.config：

关于配置文件的说明可以参考：https://github.com/NLog/NLog/wiki

```
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true"
      throwConfigExceptions="true">
  <targets>
    <target name="f1" xsi:type="File" fileName="Logs\nlog-all-${shortdate}.log"/>
    <target name="n1" xsi:type="Network" address="tcp://localhost:4001"/>
    <target name="c1" xsi:type="Console" encoding="utf-8"
            error="true"
            detectConsoleAvailable="true" />
    <target name="c2" xsi:type="ColoredConsole" encoding="utf-8"
          useDefaultRowHighlightingRules="true"
          errorStream="true"
          enableAnsiOutput="true"
          detectConsoleAvailable="true"
          DetectOutputRedirected="true">
    </target>
  </targets>
  <rules>
    <logger name="*" maxLevel="Debug" writeTo="c2" />
    <logger name="*" minLevel="Info" writeTo="f1" />
  </rules>
</nlog>
```
之后在程序中就可以正常使用日志功能了。比如：
```
[AllowAnonymous]
        [Route("Error")]
        public IActionResult Error()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ViewBag.Message = exception.Error.Message;
            //ViewBag.Path = exception.Path;
            //ViewBag.StackTrace = exception.Error.StackTrace;
            logger.LogError($"异常路径：{exception.Path},异常堆栈：{exception.Error}");

            return View("Error");
        }
```
还有可以在`appsettings.json`里面配置日志等级和命名空间过滤，跟在NLog.conf里面配置效果是一样的。例如：
```
"Logging": {
    "LogLevel": {
        "Default": "Warning",
        "StudyManagement.Controllers.ErrorController": 
        "Warning"
    }
}
```
# Identity身份验证框架
### 配置Identity
1、使用NuGet包安装`Microsoft.AspNetCore.Identity.EntityFrameworkCore`
2、`AppDbContext`继承`IdentityDbContext`,`IdentityDbContext`继承了`DbContext`，所以不用再次显示继承`DbContext`
3、注册依赖服务
```
services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
```
4、添加中间件，因为希望在请求到达MVC之前就对用户进行身份验证，所以需要在路由中间件之前添加
```
//添加验证中间件
app.UseAuthentication();

//添加路由中间件
app.UseRouting();
```
5、添加身份迁移
### 使用Identity注册新用户
1、添加ViewModel模型
```
 public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "邮箱地址")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "两次密码输入不一致，请检查后重新输入")]
        [Display(Name = "确认密码")]
        public string ConfirmPassword { get; set; }
    }
```
2、编写控制器
依赖注入`UserManager`和`SignInManager`，这是Identity默认提供用于创新新用户和登录的服务
```
private UserManager<IdentityUser> _usermanager;
        private SignInManager<IdentityUser> _signmanager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _usermanager = userManager;
            _signmanager = signInManager;
        }
```
3、编写视图页面
4、实现注册功能
```[HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //将数据从模型中复制到IdentityUser
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _usermanager.CreateAsync(user, model.Password);

                //如果注册成功，则使用登录服务登录
                //并重定向到HomeController的索引操作
                if (result.Succeeded)
                {
                    await _signmanager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                //如果有任何错误，则天机到ModelState对象中
                //将由验证摘要标记助手显示到视图上
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
            }
            return View(model);
        }
```
### Identity中对密码复杂度的处理
使用`ConfigureServices`方法中的`Configure`，具体代码如下：
```
services.Configure<IdentityOptions>(options =>
            {
                //密码中允许最大的重复数字
                options.Password.RequiredUniqueChars = 3;
                //密码至少包含一个非字母的数字的字符
                options.Password.RequireNonAlphanumeric = false;
                //密码是否必须包含小写字母
                options.Password.RequireLowercase = false;
                //密码是否必须包含大写字母
                options.Password.RequireUppercase = false;
            });
```
###### 修改中文提示的错误信息
Identity提供了`AddErrorDescriber`方法，需要创建一个错误描述类并继承自`IdentityErrorDescriber`，然后重写
```
 public class CustomerErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
        {
            return new IdentityError { Code = nameof(DefaultError), Description = "发生了未知的故障" };
        }

          .......

}
```
最后注册到`AddIdentity`中即可
```
services.AddIdentity<IdentityUser, IdentityRole>()
                .AddErrorDescriber<CustomerErrorDescriber>()
                .AddEntityFrameworkStores<AppDbContext>();
```
### 登录状态及注销功能
1、修改`_Layout`布局页面
```
@using Microsoft.AspNetCore.Identity
@inject SignInManager<IdentityUser> _signInManager

<ul class="navbar-nav ml-auto">
    @*如果已经登录，则显示注销链接*@
    @if (_signInManager.IsSignedIn(User))
    {
        <form method="post" asp-action="logout" asp-controller="account">
            <button type="submit" class="nav-link btn btn-link py-0" style="width:auto">
                注销 @User.Identity.Name
            </button>
        </form>
    }
    else
    {
        <li class="nav-item">
            <a class="nav-link" asp-controller="Account" asp-action="Register">注册</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" asp-controller="Account" asp-action="Login">登录</a>
        </li>
    }
</ul>
```
2、在控制器添加Logout方法
```
[HttpPost]
public async Task<IActionResult> Logout()
{
    await _signmanager.SignOutAsync();
    return RedirectToAction("index", "home");
}
```

### 使用Identity实现登录功能
大概需要如下步骤：
+ 创建视图模型
+ 登录视图
+ `HttpGet`和`HttpPost`方法

1、创建`LoginViewModel`视图模型，`RememberMe`用于是否记录`Cookie`
```
public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "记住我")]
        public bool RememberMe { get; set; }
    }
```
2、编写登陆视图

3、控制器编写操作方法,调用`Identity`中的`PasswordSignInAsync`并传入对应参数
```
[HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signmanager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "登录失败，请重试");
            }

            return View(model);
        }
```
# 授权与验证
### Authorize属性
+ 可以在控制器上设置`[Authorize]`属性，表示这个控制器需要授权
+ 也可以在各个操作方法上
+ `AllowAnonymous`通常配合`Authorize`使用，功能恰恰相反，表示允许匿名访问，比如注册和登录
+ 全局应用`Authorize`属性

```
services.AddControllersWithViews(config =>
 {
     var poicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
     config.Filters.Add(new AuthorizeFilter(poicy));
 }).AddXmlDataContractSerializerFormatters();
```
### 开放式重定向攻击
```
 if (result.Succeeded)
 {
     if (!string.IsNullOrEmpty(returnUrl))
     {
         if (Url.IsLocalUrl(returnUrl))
         {
             return Redirect(returnUrl);
         }
     }
     else
     {
         return RedirectToAction("Index", "Home");
     }

 }
```
### 客户端验证
服务端验证通常是由验证属性完成，例如常见的`[Required]`等
###### 使用隐式客户端验证库
通过LibMan安装库
```
{
      "library": "jquery-validate@1.19.2",
      "destination": "wwwroot/lib/jquery.min.js"
    },
    { "library": "jquery-validation-unobtrusive@3.2.11",
      "destination": "wwwroot/lib/jquery-validate-nobtrusive"
    }
```
在视图中引用这3个库(还有一个jQuery库，需按照顺序加载)

### 远程验证
> 假设现在有这样一个场景：注册时检查邮箱是否已经被注册了
###### 新增一个操作方法，如下：
```
[AllowAnonymous]
[AcceptVerbs("GET", "POST")]
public async Task<IActionResult> IsEmailUse(string email)
{
    var user = await _usermanager.FindByEmailAsync(email);
    if (user == null)
    {
        return Json(true);
    }
    else
    {
        return Json($"邮箱：{email}已经被注册了！");
    }
}
```
###### 使用远程属性
```
[Remote(action: "IsEmailUse", controller: "Account")]
public string Email { get; set; }
```
### 自定义验证属性
> 假设这样一个场景：注册时只能使用后缀为outlook.com的邮箱

###### 创建一个派生自`ValidationAttribute`的类
```
public class ValidEmailDomainAttribute : ValidationAttribute
{
    private readonly string allowedDomail;

    public ValidEmailDomainAttribute(string allowedDomain)
    {
        this.allowedDomail = allowedDomain;
    }

    public override bool IsValid(object value)
    {
        string[] strings = value.ToString().Split('@');
        return strings[1].ToUpper() == allowedDomail.ToUpper();
    }
}
```
###### 使用自定义验证属性
```
[ValidEmailDomain(allowedDomain: "outlook.com", ErrorMessage = "邮箱的地址后缀必须是outlook.com")]
public string Email { get; set; }
```

# 角色管理与用户扩展
### 扩展IdentityUser类
1、新建一个`ApplicationUser`类继承自`IdentityUser`
2、将引用`IdentityUser`的替换为`ApplicationUser`
3、限定`AppDbContext`中继承的`IdentityUser`类型为`ApplicationUser`
```
public class AppDbContext : IdentityDbContext<ApplicationUser>
```
4、因为实体字段发生了变化，所以还需要使用EF Core迁移到数据库
5、为了能够在`AspNetUsers`表中存储自定义的字段，需要修改以下内容
+ `RegisterViewModel`类
+ `Register.cshtml`视图
+ `AccountController`控制器中的`Register()`操作方法

### 角色管理
1、创建Admin控制器以及在构造方法中注入`RoleManager`服务
```
 private readonly RoleManager<IdentityRole> _roleManager;
private readonly UserManager<ApplicationUser> _userManager;

public AdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
{
    _roleManager = roleManager;
    _userManager = userManager;
}
```
2、实现新建角色的操作方法
```
[HttpGet]
public IActionResult CreateRole()
{
    return View();
}

/// <summary>
/// 创建角色
/// </summary>
/// <param name="model"></param>
/// <returns></returns>
[HttpPost]
public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
{
    if (ModelState.IsValid)
    {
        IdentityRole identityRole = new IdentityRole()
        {
            Name = model.RoleName
        };

        IdentityResult result = await _roleManager.CreateAsync(identityRole);

        if (result.Succeeded)
        {
            return RedirectToAction("ListRoles", "Admin");
        }

        foreach (var item in result.Errors)
        {
            ModelState.AddModelError("", item.Description);
        }
    }
    return View(model);
}
```
3、创建ViewModel视图模型
4、编写视图

### 显示所有角色列表
1、使用`RoleManager`类的角色属性返回所有`IdentityRole`对象
```
 [HttpGet]
 public IActionResult ListRoles()
 {
     var roles = _roleManager.Roles;
     return View(roles);
 }
```
2、编写视图
```
@if (Model.Any())
{
    <a class="btn btn-primary mb-3" style="width:auto" asp-action="CreateRole" asp-controller="Admin">添加新角色</a>
    foreach (var item in Model)
    {
        <div class="card mb-3">
            <div class="card-header">角色ID：@item.Id</div>
            <div class="card-body">
                <h5 class="card-title">@item.Name</h5>
            </div>
            <div class="card-footer">
                <a class="btn btn-info" asp-action="EditRole" asp-controller="Admin" asp-route-id="@item.Id">编辑</a>
                <a class="btn btn-danger" asp-action="DelRole" asp-controller="Admin" asp-route-id="@item.Id">删除</a>
            </div>
        </div>
    }
}
else
{
    <div class="card-header">尚未创捷任何角色</div>
    <div class="card-body">
        <h5 class="card-title">点击下面的按钮创建角色</h5>
        <a class="btn btn-primary" style="width:auto" asp-action="CreateRole" asp-controller="Admin">创建角色</a>
    </div>
}
```

### 编辑角色
1、编写ViewModel类，这里的ID用String类型，因为ID是通过TagHelper传递过来的，这里仅作展示
```
public class EditRoleViewModel
{
    public EditRoleViewModel()
    {
        Users = new List<string>();
    }


    [Display(Name = "角色ID")]
    public string Id { get; set; }

    [Required(ErrorMessage = "角色名称是必填的")]
    [Display(Name = "角色名称")]
    public string RoleName { get; set; }

    public List<string> Users { get; set; }
}
```
2、实现编辑的操作方法，完整代码在`\Controllers\AdminController\EditRole`中
3、编写视图
> 当使用了`Model.Users.Any()`时角色下没有用户信息则会报错，只需要在ViewModel中初始化Users属性即可

### 角色管理中的用户关联
Identity中`AspNetUserRoles`表分别与`AspNetUsers`的ID和`AspNetRoles`的ID对应
+ 创建ViewModel视图模型
+ 实现HttpGet操作方法

  1. 通过roleId查询角色信息
  2. 获取所有用户数据
  3. 判断当前用户是否已经存在于角色当中
  4. 存在则将IsSelected赋值为True，反之则为False

+ 实现HtppPost操作方法

  1. 通过roleId查询角色信息
  2. 循环遍历视图提交的模型数据
  3. 查询当前循环的用户
  4. 检查当前用户ID是否被选中
  5. 如果选中了则添加到角色列表中，反之则移除
  6. 对于其他情况则直接进入下一个循环
  7. 最后再检查一下当前循环的用户是否为最后一个用户
  8. 如果是则跳转到视图中，反之则跳出，进行下一个循环

+ 编写视图

# 角色授权与用户管理
### 基于角色的授权
可以同时授权多个角色，只需用逗号隔开
```
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
```
Tips：需要添加授权中间件`UseAuthorization();`，
要注册到`UseRouting();`和`UseEndpoints`之间。
此中间件与身份验证`UseAuthentication();`中间件很相似，勿混淆。

### 在菜单栏上显示或隐藏管理
在布局视图上使用`SignInManager`服务判断用户是否登录和是否为管理员角色
```
@if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
```

### 拒绝访问功能
Authorize属性可防止未经授权访问，如果登录用户不是管理员角色，
则会重定向到/Account/AccessDenied，所以只需要在Account控制器中添加AccessDenied操作方法，
最后编写与之对应的视图代码即可。

### 获取Identity中的用户列表
1. 添加操作方法，使用`UserManager`查询所有用户
2. 编写视图
3. 修改注册操作方法

```
if (result.Succeeded)
{

    if (_signmanager.IsSignedIn(User)&&User.IsInRole("Admin"))
    {
        return RedirectToAction("ListUsers", "Admin");
    }
    await _signmanager.SignInAsync(user, isPersistent: false);
    return RedirectToAction("Index", "Home");
}
```

### 编辑Identity用户
1. 添加编辑用户ViewModel
2. 添加编辑用户操作方法
3. 编写编辑用户视图

### 删除用户
不建议使用Get情趣删除数据，推荐使用POST请求
1. 添加删除用户操作方法
2. 完善删除按钮

```
<div class="card-footer">
    <a asp-action="EditUser" asp-controller="Admin" asp-route-id="@item.Id" class="btn btn-primary">编辑</a>

    <span id="confirmDeleteSpan_@item.Id" style="display:none">
        <span>您确定要删除？</span>
        <button type="submit" asp-action="DeleteUser" asp-route-id="@item.Id" class="btn btn-danger">是</button>
        <a href="#" class="btn btn-success" onclick="confirmDelete('@item.Id', false)">否</a>
    </span>

    <span id="deleteSpan_@item.Id">
        <a href="#" class="btn btn-danger" onclick="confirmDelete('@item.Id', true)">删除</a>
    </span>

</div>
```
JS代码：
```
function confirmDelete(uniqueId, isDeleteClicked) {
var deleteSpan = "deleteSpan_" + uniqueId;
var confirmDeleteSpan = "confirmDeleteSpan_" + uniqueId;

if (isDeleteClicked) {
    $("#" + deleteSpan).hide();
    $("#" + confirmDeleteSpan).show();
} else {
    $("#" + deleteSpan).show();
    $("#" + confirmDeleteSpan).hide();
}
}
```

### 删除角色
xxxxx

# EF Core中的数据完整性约束
数据完整性约束是指当执行添加、更新、删除等操作时防止不符合规范的数据进入数据库。
> 假设：某个用户因为误操作将Admin角色删除了，那么该角色下关联的用户也会一同被删除。

##### 解决办法：
重新配置它们的关联关系
```
 protected override void OnModelCreating(ModelBuilder modelBuilder)
 {
     //modelBuilder.InsertSeedData();
     base.OnModelCreating(modelBuilder);
     modelBuilder.InsertSeedData();

     //获取当前系统中所有领域模型上的外键列表
     var foreignKeys = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());


     foreach (var item in foreignKeys)
     {
         //将它们的删除行为配置为无操作
         item.DeleteBehavior = DeleteBehavior.Restrict;
     }
 }
```

# 声明授权
### Identity中的用户角色
Tips:详见Admin控制器中管理用户声明
1. 添加视图模型
2. 实现操作方法
3. 编写视图文件

### 启用MARS与模型绑定失效
在`appsettings.json`修改数据库连接字符串
```
 "StudentDBConnection": "server=localhost;database=StudentDB;Trusted_Connection=true;MultipleActiveResultSets=True"
```

### 声明授权
比如，我们希望程序支持只有拥有某个权限声明的用户才能操作某些特殊功能，这时候就可以使用声明授权了
1. 增加`ClaimsStore`类，用于管理一些声明

```
public static List<Claim> AllClaims = new List<Claim>()
{
    new Claim("创建角色","Create Role"),
    new Claim("编辑角色","Edit Role"),
    new Claim("删除角色","Delete Role"),
    new Claim("编辑学生信息", "Edit Student")
};
```
2. 添加ViewModel视图模型

```
public class UserClaimsViewModel
{
    public UserClaimsViewModel()
    {
        Cliams = new List<UserClaim>();
    }

    public string UserId { get; set; }
    public List<UserClaim> Cliams { get; set; }
}
```
3. 添加`UserClaim`类，用户与声明之间存在一对多的关系，用于保存声明列表

```
public class UserClaim
{
    public string ClaimType { get; set; }
    public bool IsSelected { get; set; }
}
```
4. 实现操作方法
5. 编写视图文件

# RBAC与CABC
RBAC:基于角色的访问控制

CABC:基于声明的授权也被称为基于上下文的访问控制协议

### 角色与策略的结合
```
//策略结合声明授权
services.AddAuthorization(options =>
{
    //只有满足删除角色声明，此策略才能成功
    options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("删除角色"));
    options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));

    //策略结合多个角色进行授权
    options.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("Admin", "User", "SuperManager"));

    //必须包含编辑角色声明且为true，此策略才能成功
    //options.AddPolicy("EditRolePolicy", policy =>
    //policy.RequireClaim("编辑角色", "true", "yes")
    //.RequireRole("Admin"));


    options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context => AuthorizeAccess(context)));
});
```

### 在MVC视图中进行角色与声明授权
```
@*将用户和策略名称作为参数传给IAuthorizationService的AuthorizeAsync方法，验证成功则为True*@
@if ((await authorizationService.AuthorizeAsync(User, "EditRolePolicy")).Succeeded)
{
    <a class="btn btn-info" asp-action="EditRole" asp-controller="Admin" asp-route-id="@item.Id">编辑</a>
}
```

### AccessDenied视图的路由配置修改
```
services.ConfigureApplicationCookie(options =>
{
    //修改拒绝访问的路由地址
    options.AccessDeniedPath = new PathString("/Admin/AccessDenied");
    //统一系统的全局Cookie名称
    options.Cookie.Name = "StudentMangementCookie";
    //登录用户Cookie的有效期
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    //是否对Cookie启用滑动过期时间
    options.SlidingExpiration = true;
});
```

### 策略授权中的ClaimType和ClaimValue
Tips：详见代码，管理角色那一块
```
//必须包含编辑角色声明且为true，此策略才能成功
options.AddPolicy("EditRolePolicy", policy =>
policy.RequireClaim("编辑角色", "true", "yes")
.RequireRole("Admin"));
```

### 使用委托创建自定义策略授权
##### 创建自定义策略规则
```
options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context => AuthorizeAccess(context)));

private bool AuthorizeAccess(AuthorizationHandlerContext context)
{
    return context.User.IsInRole("Admin") && context.User.HasClaim(claim => claim.Type == "编辑角色" && claim.Value == "编辑角色") || context.User.IsInRole("Super Admin");
}
```

##### 自定义复杂授权需求
ASP.NET Core中内置的授权需求有以下3个：

+ `RequireClaim()`方法用于管理声明授权的需求
+ `RequireRole()`方法用于管理角色授权的需求
+ `RequireAssertion()`方法可用于自定义授权的需求

##### 自定义授权需求和处理程序
1. 创建自定义需求，继承自`IAuthorizationRequirement`

```
/// <summary>
/// 管理Admin角色与声明的需求
/// </summary>
public class ManageAdminRolesAndClaimsRequirement : IAuthorizationRequirement
{

}
```
2. 实现自定义授权处理程序

继承自`AuthorizationHandler<T>`，并实现`HandleRequirementAsync()`方法，泛型T传入`ManageAdminRolesAndClaimsRequirement`类型
3. 注册自定义授权处理程序

`services.AddHttpContextAccessor();`
4. 将自定义策略添加到对应控制器的操作方法上
```
[HttpGet]
[Authorize(Policy ="EditRolePolicy")]
public async Task<IActionResult> ManageUserRoles(string userId)
```

##### 多个自定义授权处理程序
+ context.Succed()-----成功
+ context.Fail()-----失败
+ Task.CompletedTask-----任务执行完毕

Tips：如果一个授权需求有多个处理程序，失败是优于成功的，这意味着当其中一个处理程序返回失败时，即使其他处理程序返回成功，策略也会失败。一般来说，不推荐在处理程序返回失败。

1. 创建自定义需求处理程序
2. 注册自定义需求处理程序

` services.AddSingleton<IAuthorizationHandler, SuperAdminHander>();`

# Identity的账户中心设计
### MicroSoft授权登录
1. 创建Azure OAuth凭据

+ 登录`https://portal.azure.com/`
+ 进入`Azure Active Directory`
+ 在左侧导航中进入`应用注册`
+ 注册应用程序，填写应用程序显示名称，选择第三项任何组织目录
+ 在`品牌打造`中填写相关信息
+ 点击`证书和密码`生成密钥
+ 在`概述`中可以找到客户端ID
2. 在ASP.NET Core中启用MicroSoft身份验证

+ 通过Nuget添加`Microsoft.AspNetCore.Authentication.MicrosoftAccount`程序包
+ 在`ConfigureServices`注入

```
services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
{
    microsoftOptions.ClientId = _configuration["Authentication:Microsoft:ClientId"];
    microsoftOptions.ClientSecret = _configuration["Authentication:Microsoft:ClientSecret"];
});
```
+ 将客户端ID和密钥配置到`appsettings.json`

```
{
  "Authentication": {
    "Microsoft": {
      "ClientId": "315001c3-4a3a-4321-928e-d2bb04bcbaa0",
      "ClientSecret": "xxxxxxxxxxxxxxx"
    },
    "GitHub": {
      "ClientId": "96d72070581a0c81435c",
      "ClientSecret": "xxxxxxxxxxxxxxxxx"
    }
  }
}
```
3. 修改登录视图模型

添加ReturnUrl和ExternalLogins属性分别用于重定向和启用第三方登录列表

```
public string ReturnUrl { get; set; }
public IList<AuthenticationScheme> ExternalLogins { get; set; }
```
4. 修改登录视图
使用forech遍历`Model.ExternalLogins`中每个第三方登录提供程序，动态为其创建一个按钮

由于按钮的name属性设置为了`provider`，因此模型绑定器会将name属性设为`provider`，value属性值映射到`ExternalLogin()`操作方法的provider参数中
```
 <div class="col-md-6 border-left">
        <h1>第三方授权登录</h1>
        <form method="post" class="mt-3" asp-action="ExternalLogin" asp-controller="Account" asp-route-returnUrl="@Model.ReturnUrl">
            <div>
                @foreach (var item in Model.ExternalLogins)
                {
                    <button type="submit" class="btn btn-info" name="provider" value="@item.Name" title="使用您的 @item.DisplayName 账户 ">@item.DisplayName</button>
                }
            </div>
        </form>
    </div>
```
5. 实现`ExternalLogin`操作方法

```
[HttpPost]
public IActionResult ExternalLogin(string provider, string returnUrl)
{
    var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
    var properties = _signmanager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
    return new ChallengeResult(provider, properties);
}
```
6. 实现`ExternalLoginCallback()`授权回调操作方法

+ 通过`returnUrl ?? Url.Content("~/")`获取重定向URL
+ 实例化视图模型，通过`Signmanager`下的`GetExternalAuthenticationSchemesAsync()`方法获取第三方登录列表并存储
+ 从第三方登录提供商中获取关于用户的登录信息
+ 如果用户之前已经授权过了，则无需创建新的数据，直接使用当前数据登录系统即可
+ 如果`AspNetUserLogins`表中没有该数据，则创建一个新的数据
+ 获取邮箱地址信息，通过邮箱查询用户是否存在，如不存在则创建，但该用户没有密码
+ 如果获取不到邮箱地址信息，则重定向到错误视图

### 集成GitHub身份验证登录
1. 在`Settings`中进入`Developer settings`,选择`OAuth Apps`进入
2. 点击`New Oauth App`进入填写相关信息创建，然后就会生成客户端ID和密钥
3. 添加`AspNet.Security.OAuth.GitHub`程序包
4. 在`ConfigureServices`注入

```
services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
{
    microsoftOptions.ClientId = _configuration["Authentication:Microsoft:ClientId"];
    microsoftOptions.ClientSecret = _configuration["Authentication:Microsoft:ClientSecret"];
}).AddGitHub(options =>
{
    options.ClientId = _configuration["Authentication:GitHub:ClientId"];
    options.ClientSecret=_configuration["Authentication:GitHub:ClientSecret"];
});
```
5. 将客户端ID和密钥配置到`appsettings.json`

Tips：Github身份验证，获取邮箱信息时可能为null，具体原因待深入探究

### 用户机密
存储在`appsettings.json`中的密钥等信息会随项目部署或开源等暴漏，所以像这类机密信息可以用到`用户机密`

右键项目点击`管理用户机密`，此时会自动生成一个`serrets.json`文件，结构与`appsettings.json`一样，直接搬过来即可

### 验证账户信息安全
##### 阻止未验证的账户登录
1. 启用电子邮箱验证

```
services.Configure<IdentityOptions>(options =>
{
    //启用邮箱验证
    options.SignIn.RequireConfirmedEmail = true;
});
```
2. 修改登录操作方法

```
if (user != null && !user.EmailConfirmed && (await _usermanager.CheckPasswordAsync(user, model.Password)))
{
    ModelState.AddModelError(string.Empty, "邮箱尚未验证");
    return View(model);
}
```
3. 阻止第三方未验证账户登录

修改第三方登录回调操作方法
```
if (email != null)
{
    //通过邮箱去查询用户是否存在
    user = await _usermanager.FindByEmailAsync(email);

    if (user != null && !user.EmailConfirmed)
    {
        ModelState.AddModelError(string.Empty, "您的电子邮箱还未进行验证");
        return View("Login", model);
    }
}
```

##### 电子邮箱确认令牌
假设在注册时需要邮箱验证成功才能注册成功
1. 创建确认邮箱操作方法

```
[HttpGet]
public async Task<IActionResult> ConfirmEmail(string userId, string token)
{
    if (userId == null || token == null)
    {
        return RedirectToAction("index", "home");
    }

    var user = await _usermanager.FindByIdAsync(userId);

    if (user == null)
    {
        ViewBag.ErrorMessage = $"当前{userId}无效";
        return View("NotFound");
    }

    var result = await _usermanager.ConfirmEmailAsync(user, token);
    if (result.Succeeded)
    {
        return View();
    }

    ViewBag.ErrorTitle = "您的电子邮箱还未进行验证";
    return View("Error");
}
```
> `ConfirmEmailAsync()`方法用于确认电子邮箱，需要传入用户和Token两个参数

2. 修改注册操作方法

```
//如果注册成功，则使用登录服务登录
//并重定向到HomeController的索引操作
if (result.Succeeded)
{
    //生成电子游戏确认令牌
    var token = await _usermanager.GenerateEmailConfirmationTokenAsync(user);

    //生成电子邮箱的确认链接
    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
    _logger.Log(LogLevel.Warning, confirmationLink);

    if (_signmanager.IsSignedIn(User) && User.IsInRole("Admin"))
    {
        return RedirectToAction("ListUsers", "Admin");
    }
    //await _signmanager.SignInAsync(user, isPersistent: false);
    //return RedirectToAction("Index", "Home");

    ViewBag.ErrorTitle = "注册成功";
    ViewBag.ErrorMessage = "在您登入系统前，我们已经给您发了一封邮件，需要您进行邮件验证，点击确认链接即可完成";
    return View("Error");
}
```
3. 注入双因子身份验证服务
AddDefaultTokenProviders()
```
services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddErrorDescriber<CustomerErrorDescriber>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
```

##### 第三方登录的电子邮箱确认令牌
修改第三方登录回电操作方法`ExternalLoginCallback`
```
if (email != null)
{

    if (user == null)
    {
        user = new ApplicationUser
        {
            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
        };

        await _usermanager.CreateAsync(user);

        var token = await _usermanager.GenerateEmailConfirmationTokenAsync(user);

        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
        _logger.Log(LogLevel.Warning, confirmationLink);

        ViewBag.ErrorTitle = "注册成功";
        ViewBag.ErrorMessage = "在您登入系统前，我们已经给您发了一封邮件，需要您进行邮件验证，点击确认链接即可完成";
        return View("Error");
    }

    await _usermanager.AddLoginAsync(user, info);
    await _signmanager.SignInAsync(user, isPersistent: false);

    return LocalRedirect(returnUrl);
}
```

##### 激活用户邮箱
1. 添加视图模型

> 因为只需要对邮箱进行操作，所以目前暂时只有`Email`属性


2. 新增两个激活邮箱的操作方法

```
XXXXX


判断是否已经激活邮箱，如果没有激活则生成激活链接
public async Task<IActionResult> ActivateUserEmail(EmailAdressViewModel model)
{
    if (ModelState.IsValid)
    {
        var user = await _usermanager.FindByEmailAsync(model.Email);

        if (user != null)
        {
            if (!await _usermanager.IsEmailConfirmedAsync(user))
            {
                var token = await _usermanager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                _logger.Log(LogLevel.Warning, confirmationLink);


                ViewBag.Message = "如果您在我们系统有注册账户，我们已经发了邮件到您的邮箱中，请前往邮箱激活您的账户！";
                return View("ActivateUserEmailConfirmation", ViewBag.Message);
            }
        }
    }
    ViewBag.Message = "请确认邮箱是否存在异常，现在我们无法给您发送激活链接。";
    return View("ActivateUserEmailConfirmation", ViewBag.Message);
}
```
3. 新增两个视图，分别用于提交邮箱地址和告知用户已经发送了邮箱地址

##### 忘记密码功能
1. 新增忘记密码操作方法

   1. 通过邮箱查询用户
   2. 如果查到了用户并且用户确认了电子邮箱
   3. 生成重置密码令牌`GeneratePasswordResetTokenAsync(用户)`
   4. 生成密码重置链接
   5. 重定向用户到忘记密码确认视图
2. 编写找回密码视图，用于提交邮箱
3. 编写邮件发送通知视图

##### 重置密码功能
1. 创建视图模型

```
public class ResetPasswordViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "邮箱地址：")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "密码：")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "两次密码不一致，请重新输入！")]
    [Display(Name = "确认密码：")]
    public string ConfirmPassword { get; set; }

    public string Token { get; set; }
}
```
2. 编写重置密码视图

> Tips：使用两个`<input>`元素来存储邮箱和重置密码令牌，后面回调的时候需要使用它们


3. 添加重置密码GET请求操作方法
校验token和邮箱是否存在，也就是上面隐藏的那两个元素
```
[HttpGet]
public IActionResult ResetPassword(string email, string token)
{
    if (email == null || token == null)
    {
        ModelState.AddModelError(string.Empty, "无效的密码重置令牌");

    }

    return View();
}
```
4. 添加重置密码POST请求操作方法
使用`ResetPasswordAsync()`方法传入用户、Token和邮箱重置
```
if (user != null)
{
    var result = await _usermanager.ResetPasswordAsync(user, model.Token, model.Password);

    if (result.Succeeded)
    {
        return View("ResetPasswordConfirmation");
    }

    foreach (var item in result.Errors)
    {
        ModelState.AddModelError(string.Empty, item.Description);
    }
    return View(model);
}
```
5. 最后编写一个用于告知用户重置成功视图

# 剖析Identity源代码
### 自定义令牌有效期
1. 创建令牌有效期配置类，并继承内置地`DataProtectionTokenProviderOptions`
2. 创建令牌提供程序

```
public class CustomEmailConfirmationTokenProvider<Tuser> : DataProtectorTokenProvider<Tuser> where Tuser : class
{
    public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<CustomEmailConfirmationTokenProviderOptions> options, ILogger<DataProtectorTokenProvider<Tuser>> logger) : base(dataProtectionProvider, options, logger)
    {

    }
}
```
3. 依赖注入到项目中

```
//通过自定义的CustomEmailconfirmationmin名称覆盖旧有token名称，使它与AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation")关联在一起
options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";


services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddErrorDescriber<CustomerErrorDescriber>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");
```
4. 更改令牌有效期

```
//将所有令牌类型的有效期设置为30分钟
services.Configure<DataProtectionTokenProviderOptions>(o =>
{
    o.TokenLifespan = TimeSpan.FromMinutes(30);
});

//使用自定义令牌有效期将邮箱验证有效期设置为5分钟
services.Configure<CustomEmailConfirmationTokenProviderOptions>(o =>
{
    o.TokenLifespan = TimeSpan.FromMinutes(5);
});
```

### DataProtection加密和解密
1. 创建数据保护字符串类

```
public class DataProtectionPurposeStrings
{
    public readonly string StudentIdRouteValue = "StudentIdRouteValue";
}
```
2. 依赖注入注册数据保护类

```
services.AddSingleton<DataProtectionPurposeStrings>();
```
3. 添加加密模型

> `[NotMapped]`属性指示ORM框架不将此列映射到数据库中
```
[NotMapped]
public string EncryptedId { get; set; }
```
4. 在控制器中注入`IDataProtector`，使用`Protect()`和`Unprotect()`即可实现加密解密
```
string decryptedId = protector.Unprotect(id);
int decryptedStudentId = Convert.ToInt32(decryptedId);
```

### 更改密码
Identity提供了`ChangePasswordAsync()`方法
```
await _usermanager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
```
传入当前用户对象，当前账户密码，以及新密码。用户成功更改后，调用SignInManager的`RefreshSignInAsync()`刷新登录Cookie

### w为第三方账户添加密码
Identity已经封装好了`AddPasswordAsync()`方法,调用时传入用户对象以及新密码。
```
await _usermanager.AddPasswordAsync(user, model.NewPassword);
```

本地账户都是有密码的，所以只需要通过判断HasPassword是否为null。

在授权登录回调操作犯法中判断，如果为null，则重定向到添加密码视图中
```
if (signInResult.Succeeded)
{
    //查询该用户是否有密码，没有则重定向添加密码页面
    var userHasPassword = await _usermanager.HasPasswordAsync(user);
    if (!userHasPassword)
    {
        return RedirectToAction("AddPassword");
    }
    return LocalRedirect(returnUrl);
}
```

### 账户锁定
1. 配置锁定规则

```
//设置登录失败次数和时间
options.Lockout.MaxFailedAccessAttempts = 5;
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
```
2. 启用账户锁定

```
//最后一个布尔参数表示是否启用账户锁定
var result = await _signmanager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);
```
3. 账户锁定后可通过重置密码再次登录，所以当重置密码成功后，需要将解锁时间设置为当前时间
```
if (result.Succeeded)
{
    if (await _usermanager.IsLockedOutAsync(user))
    {
        await _usermanager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
    }
    return View("ResetPasswordConfirmation");
}
```

# 架构
架构规则：
+ 架构是顶层设计
+ 框架是面向编程或配置的半成品
+ 组件是从技术维度上的复用
+ 模块是从业务维度上职责的复用
+ 系统是相互协同可运行的实体

# 仓储模式
### 泛型仓储的实现
创建`IRepository`接口用于仓储实现的约定
```
/// <summary>
/// 此接口是所有仓储的约定, 此接口仅作为约定，用于标识它们。
/// </summary>
/// <typeparam name="TEntity">当前传入仓储的实体类型</typeparam>
/// <typeparam name="TprimaryKey">传入仓储的主键类型</typeparam>
public interface IRepository<TEntity, TprimaryKey> where TEntity : class
{
    #region 查询
    /// <summary>
    /// 获取用于整个表中检索实体的IQueryable
    /// </summary>
    /// <returns>可用于从数据库中选择实体</returns>
    IQueryable<TEntity> GetAll();

    /// <summary>
    /// 用于获取所有实体
    /// </summary>
    /// <returns>所有实体列表</returns>
    List<TEntity> GetAllList();


    xxxxxxxxxx

}
```

### 异步编程与同步编程
异步编码在运行时，会增加少量的开销，在低流量时对性能的影响可以忽略不计，针对高流量的情况下潜在的性能提升时客观的
栗子：
```
public async Task<List<TEntity>> GetAllListAsync()
{
    return await GetAll().ToListAsync();
}
```
> 异步方法要正常执行必须要有`async`、`Task<T>`返回值和`await`关键字


###  `IRepository`接口设计
通过之前定义的接口，了解一下规则和设计
```
List<TEntity> GetAllList(); //同步方法
Task<List<TEntity>> GetAllListAsync(); //异步方法
List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate); //支持动态Linq的同步方法
Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate); //支持动态Linq的异步方法
```


###  `RepositoryBase`实现
```
public class RepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// 通过泛型从上下文中获取领域模型
    /// </summary>
    public virtual DbSet<TEntity> Table => _dbContext.Set<TEntity>();

    public RepositoryBase(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}
```

> 由于像更新添加删除这样的操作，后面都需要调用`SaveChange`，所以可以风封装以便复用

```
#region 私有方法
/// <summary>
/// 调用数据库上下文保存数据
/// </summary>
protected void Save()
{
    _dbContext.SaveChanges();
}

protected async Task SaveAsync()
{
    await _dbContext.SaveChangesAsync();
}

/// <summary>
/// 检查实体是否处于跟踪状态，如果是则返回；反之则添加跟踪状态
/// </summary>
/// <param name="entity"></param>
protected virtual void AttachIfNot(TEntity entity)
{
    var entry = _dbContext.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);

    if (entry != null)
    {
        return;
    }
    Table.Attach(entity);
}
#endregion
```

#### 总结
+ 仓储类应该时无状态的。这意味着我们不该定义仓储等级的状态对象，并且仓储方法的调用也不应该影响到其它调用
+ 仓储可以使用依赖注入，但尽可能较少或是不依赖于其它服务


# 重构学生管理功能
### 学生列表排序
NuGet安装`System.Linq.Dynamic.Core`
```
IQueryable<Student> query = _studentRepository.GetAll();
if (!string.IsNullOrEmpty(serachStr))
{
    query = query.Where(s => s.Name.Contains(serachStr) || s.Email.Contains(serachStr));
}
query = query.OrderBy(sortBy).AsNoTracking();
```

+ 对于返回类型`IQueryable<Student> query`，`OrderBy(sortBy)`即我们提供的扩展方法，只需要传入sortBy字符串即可
+ `AsNoTracking()`方法用于提升性能。该方法会告诉EF Core不要跟踪查询的结果。

### 模糊查询
```
var query = _studentRepository.GetAll();
if (!string.IsNullOrEmpty(searchString))
{
    query = query.Where(s => s.Name.Contains(searchString) || s.Email.Contains(searchString));
    
}
query = query.OrderBy(sortBy);
```

### 一个简单分页的实现
+ 创建模型

```
public class PaginationModel
{
    /// <summary>
    /// 当前页
    /// </summary>
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// 总条数
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 每页分页条数
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

    public List<Student> Data { get; set; }

    //上一页
    public bool ShowPrevious => CurrentPage > 1;
    //下一页
    public bool ShowNext => CurrentPage < TotalPages;
    //返回第一页
    public bool ShowFirst => CurrentPage != 1;
    //返回最后一页
    public bool ShowLast => CurrentPage != TotalPages;
}
```
+ 封装分页方法

```
public  async Task<List<Student>> GetPaginatedResult(int currentPage, string searchString, string sortBy, int pageSize = 10)
{
    var query = _studentRepository.GetAll();
    if (!string.IsNullOrEmpty(searchString))
    {
        query = query.Where(s => s.Name.Contains(searchString) || s.Email.Contains(searchString));
        
    }
    query = query.OrderBy(sortBy);

    return await query.Skip((currentPage - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
}
```
+ 修改视图

# 课程列表与分组统计功能

