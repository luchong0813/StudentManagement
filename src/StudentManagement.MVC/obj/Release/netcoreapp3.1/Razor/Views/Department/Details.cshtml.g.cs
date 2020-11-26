#pragma checksum "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\Department\Details.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7dd448dea1426da234c17825fa2137b2003b4980"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Department_Details), @"mvc.1.0.view", @"/Views/Department/Details.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\_ViewImports.cshtml"
using StudentManagement.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\_ViewImports.cshtml"
using StudentManagement.ViewModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\_ViewImports.cshtml"
using StudentManagement.Extensions;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Authorization;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"7dd448dea1426da234c17825fa2137b2003b4980", @"/Views/Department/Details.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"57fcf8d6079b90bfcc447036a5e5f309709907e1", @"/Views/_ViewImports.cshtml")]
    public class Views_Department_Details : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Department>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Index", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-info"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Edit", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-primary"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\Department\Details.cshtml"
   
    ViewBag.Title = "学院详情";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div class=\"row justify-content-center m-3\">\r\n    <div class=\"col-sm-6\">\r\n        <div class=\"card bg-light mb-3\" style=\"max-width:18rem;\">\r\n            <div class=\"card-header\">\r\n                <h3 class=\"text-center\">");
#nullable restore
#line 10 "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\Department\Details.cshtml"
                                   Write(Model.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h3>\r\n            </div>\r\n\r\n            <div class=\"card-body text-center\">\r\n                <h5 class=\"card-title\">\r\n                    ");
#nullable restore
#line 15 "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\Department\Details.cshtml"
               Write(Html.DisplayNameFor(c => c.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("：");
#nullable restore
#line 15 "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\Department\Details.cshtml"
                                                 Write(Model.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </h5>\r\n                <h5 class=\"card-title\">\r\n                    ");
#nullable restore
#line 18 "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\Department\Details.cshtml"
               Write(Html.DisplayNameFor(c => c.StartDate));

#line default
#line hidden
#nullable disable
            WriteLiteral("：");
#nullable restore
#line 18 "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\Department\Details.cshtml"
                                                      Write(Html.DisplayFor(c=>c.StartDate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </h5>\r\n\r\n                <h5 class=\"card-title\">\r\n                    ");
#nullable restore
#line 22 "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\Department\Details.cshtml"
               Write(Html.DisplayNameFor(c => c.Budget));

#line default
#line hidden
#nullable disable
            WriteLiteral("：");
#nullable restore
#line 22 "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\Department\Details.cshtml"
                                                   Write(Html.DisplayFor(c=>c.Budget));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </h5>\r\n\r\n                <h5 class=\"card-title\">\r\n                    ");
#nullable restore
#line 26 "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\Department\Details.cshtml"
               Write(Html.DisplayNameFor(c => c.TeacherId));

#line default
#line hidden
#nullable disable
            WriteLiteral("：");
#nullable restore
#line 26 "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\Department\Details.cshtml"
                                                      Write(Model.Administrator.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n\r\n            <div class=\"card-footer text-center\">\r\n                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "7dd448dea1426da234c17825fa2137b2003b49808439", async() => {
                WriteLiteral("返回");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "7dd448dea1426da234c17825fa2137b2003b49809689", async() => {
                WriteLiteral("编辑");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 31 "E:\Source\StudentManagement\src\StudentManagement.MVC\Views\Department\Details.cshtml"
                                       WriteLiteral(Model.DepartmentId);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Department> Html { get; private set; }
    }
}
#pragma warning restore 1591