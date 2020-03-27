#pragma checksum "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Reports\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e9ff06bdbb10f151900e565e993ba2652bb8c271"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Reports_Index), @"mvc.1.0.view", @"/Views/Reports/Index.cshtml")]
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
#line 1 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\_ViewImports.cshtml"
using Lexiconn;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\_ViewImports.cshtml"
using Lexiconn.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e9ff06bdbb10f151900e565e993ba2652bb8c271", @"/Views/Reports/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b7f983f2bbefa874f10aa70d8bba9a296c08fe64", @"/Views/_ViewImports.cshtml")]
    public class Views_Reports_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Lexiconn.Models.Report>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Reports\Index.cshtml"
  
    ViewData["Title"] = "Звіти";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h3>Звіти</h3>\r\n<hr />\r\n\r\n<div>\r\n    <h4>Надіслати Excel-звіт</h4>\r\n");
#nullable restore
#line 12 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Reports\Index.cshtml"
     using (Html.BeginForm("Import", "Reports", FormMethod.Post, new { enctype = "multipart/form-data", id = "frm-excel" }))
    {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"        <div>
            <input type=""file"" accept="".xls, .xlsx"" name=""fileExcel"" id=""fileExcel"">
        </div>
        <br />
        <div class=""form-group"">
            <input type=""submit"" class=""btn btn-primary"" name=""submit"" id=""submit"" value=""Надіслати"" disabled>
        </div>
");
#nullable restore
#line 21 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Reports\Index.cshtml"
    }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</div>

<div class=""modal fade"" id=""errorPopup"" tabindex=""-1"" role=""alertdialog"" aria-hidden=""true"">
    <div class=""modal-dialog modal-dialog-centered"" role=""document"">
        <div class=""modal-content"">
            <div class=""modal-header"">
                <h5 class=""modal-title"">Помилка</h5>
                <button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""Close"">
                    <span aria-hidden=""true"">&times;</span>
                </button>
            </div>
            <div class=""modal-body"" id=""errorText"">
                ");
#nullable restore
#line 34 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Reports\Index.cshtml"
           Write(Model.Error);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </div>\r\n            <div class=\"modal-footer\">\r\n                <button type=\"button\" class=\"btn btn-primary\" data-dismiss=\"modal\">ОК</button>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n    \r\n\r\n");
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script>
        var fileControl = document.getElementById(""fileExcel"");
        fileControl.addEventListener(""change"", function (event) {
            var file = fileControl.files[0];
            var parts = file.name.split('.');
            var ext = parts[parts.length - 1];
            var submitBtn = document.getElementById(""submit"");

            if (!(ext.toLowerCase() === ""xls"") && !(ext.toLowerCase() === ""xlsx"")) {
                $(""#errorPopup"").modal();
                submitBtn.disabled = true;
            }
            else {
                submitBtn.disabled = false;
            }

        }, false);
    </script>

    <script>
        window.addEventListener(""load"", function () {
            if ('");
#nullable restore
#line 67 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Reports\Index.cshtml"
            Write(Model.ErrorPopupFlag);

#line default
#line hidden
#nullable disable
                WriteLiteral("\' == 1) {\r\n                $(\"#errorPopup\").modal();\r\n            }\r\n        });\r\n    </script>\r\n");
            }
            );
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Lexiconn.Models.Report> Html { get; private set; }
    }
}
#pragma warning restore 1591
