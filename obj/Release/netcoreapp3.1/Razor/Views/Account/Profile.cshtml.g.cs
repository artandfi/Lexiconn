#pragma checksum "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Account\Profile.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1153f8ea8875eca80271e4ca2608edb553709555"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_Profile), @"mvc.1.0.view", @"/Views/Account/Profile.cshtml")]
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
#nullable restore
#line 1 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Account\Profile.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1153f8ea8875eca80271e4ca2608edb553709555", @"/Views/Account/Profile.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b419682b5d042966bc09e5f067bb48fd22eec535", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_Profile : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Lexiconn.Models.User>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-warning"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "ChangePassword", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "SignOut", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\n");
#nullable restore
#line 4 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Account\Profile.cshtml"
  
    ViewData["Title"] = "Профіль";

#line default
#line hidden
#nullable disable
            WriteLiteral("\n<h1>Профіль</h1>\n<hr />\n<p style=\"font-size:16px\">Ім\'я користувача: <b>");
#nullable restore
#line 10 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Account\Profile.cshtml"
                                          Write(User.Identity.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</b></p>\n<p style=\"font-size:16px\">Email: <b>");
#nullable restore
#line 11 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Account\Profile.cshtml"
                               Write(Html.DisplayFor(model => model.Email));

#line default
#line hidden
#nullable disable
            WriteLiteral("</b></p>\n<p style=\"font-size:16px\">Ролі: <b>");
#nullable restore
#line 12 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Account\Profile.cshtml"
                              Write(ViewBag.Roles);

#line default
#line hidden
#nullable disable
            WriteLiteral("</b></p>\n<p style=\"font-size:16px\">Додано слів: ");
#nullable restore
#line 13 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Account\Profile.cshtml"
                                  Write(ViewBag.WordCount);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\n<p style=\"font-size:16px\">Усього мов: ");
#nullable restore
#line 14 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Account\Profile.cshtml"
                                 Write(ViewBag.LangCount);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\n<p style=\"font-size:16px\">Усього категорій: ");
#nullable restore
#line 15 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Account\Profile.cshtml"
                                       Write(ViewBag.CatCount);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\n\n<table>\n    <tbody>\n        <tr>\n            <td>\n                <div>\n                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "1153f8ea8875eca80271e4ca2608edb5537095556998", async() => {
                WriteLiteral("Змінити пароль");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
                </div>
            </td>
            <td>
                &nbsp;
            </td>
            <td>
                <div>
                    <button type=""button"" class=""btn btn-danger"" data-toggle=""modal"" data-target=""#confirm"">
                        Вийти
                    </button>
                </div>

                <div class=""modal fade"" id=""confirm"" tabindex=""-1"" role=""dialog"" aria-hidden=""true"">
                    <div class=""modal-dialog modal-dialog-centered"" role=""document"">
                        <div class=""modal-content"">
                            <div class=""modal-header"">
                                <h5 class=""modal-title"">Попередження</h5>
                                <button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""Close"">
                                    <span aria-hidden=""true"">&times;</span>
                                </button>
                            </div>
                            <div class=""modal-body delete-modal");
            WriteLiteral("-body\">\n                                Ви впевнені, що хочете вийти?\n                            </div>\n                            <div class=\"modal-footer\">\n                                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "1153f8ea8875eca80271e4ca2608edb5537095559534", async() => {
                WriteLiteral("\n                                    <button type=\"submit\" class=\"btn btn-danger\">Так</button>\n                                    <button type=\"button\" class=\"btn btn-secondary\" data-dismiss=\"modal\">Ні</button>\n                                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_3.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n                            </div>\n                        </div>\n                    </div>\n                </div>\n            </td>\n        </tr>\n    </tbody>\n</table>\n<br />\n<a>\n    ");
#nullable restore
#line 62 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Account\Profile.cshtml"
Write(Html.ActionLink("На головну", "Index", "Home"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n</a>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Lexiconn.Models.User> Html { get; private set; }
    }
}
#pragma warning restore 1591
