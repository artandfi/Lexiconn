#pragma checksum "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Help\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "92d05531a33ac0581c210ba966ebbbfd06a3348e"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Help_Index), @"mvc.1.0.view", @"/Views/Help/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"92d05531a33ac0581c210ba966ebbbfd06a3348e", @"/Views/Help/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b419682b5d042966bc09e5f067bb48fd22eec535", @"/Views/_ViewImports.cshtml")]
    public class Views_Help_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/Samples/Sample Report.xlsx"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("download", new global::Microsoft.AspNetCore.Html.HtmlString("Sample Report"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\n");
#nullable restore
#line 2 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Help\Index.cshtml"
  
    ViewData["Title"] = "Довідка";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<h1>Довідка</h1>
<hr />

<h3>Вступ</h3>
<p style=""font-size:15px;"">
    <b>Lexiconn</b> &#8211; це нова версія зошита, в який ви записуєте іншомовні слова із перекладами.
    І, на відміну від звичайного паперового зошита, який швидко закінчиться за місяць чи два, причому напевно лиш для однієї мови, Lexiconn надає набагато ширші можливості.
    Додавайте слова, поповнюйте свій запас на запропоновані та будь-які бажані мови, оперуйте розмаїтими частинами мови, визначайте один чи кілька перекладів для кожного слова.
    <br />
    І пам&#39;ятайте: слова формують реальність. Створюйте власну.
</p>

<h3>Формат</h3>
<p style=""font-size:15px;"">
    В усіх полях максимальна довжина введення &#8211; 50 символів.
    <br />
    Для введення допускаються будь-які символи Unicode.
    <br />
    Обмеження на зміст введених даних не накладаються &#8211; французькі слова в англійській мові не вважатимуться за помилку.
</p>

<h3>Слова</h3>
<p style=""font-size:15px;"">
    При створенні нового запису необхідно вказати сло");
            WriteLiteral(@"во, мову, категорію та принаймні один переклад. Якщо перекладів декілька &#8211; перераховуйте їх через кому.
    <br />
    Помилковими вважаються записи, у яких:
    <br />
    &#8211; слово, мова та категорія одночасно співпадають із такими для вже наявного запису;
    <br />
    &#8211; коми чи пробіли в перекладі записано некоректно (наприклад, &quot;   переклад&quot;, &quot;,переклад&quot;, &quot;переклад1,,,переклад2&quot; тощо).
    <br/>
    Для редагування запису вищенаведені принципи зберігаються (окрім того, що не змінений при редагуванні запис не вважається дублікатом самим собі).
</p>

<h3>Мови</h3>
<p style=""font-size:15px;"">
    За замовчуванням надаються три іноземні мови &#8211; англйіська, німецька та російська.
    За бажання ви можете додавати будь-які інші; введення вже наявної мови при створенні чи редагуванні розцінюється як помилка.
    <br />
    Для зручної навігації словником на сторінці кожної мови є перехід до відповідного їй списку слів.
</p>

<h3>Категорії</h3>
<p style=""font-s");
            WriteLiteral(@"ize:15px;"">
    За замовчуванням надається десять основних категорій (частин мови): іменник, прикметник, числівник, займенник, дієслово, прислівник, сполучник, прийменник, частка, вигук, модальник.
    <br />
    За бажання ви можете додавати будь-які інші; введення вже наявної категорії при створенні чи редагуванні розцінюється як помилка.
</p>

<h3>Статистика</h3>
<p style=""font-size:15px;"">
    Дані про записи представлено на двох діаграмах: слова за мовами та слова за категоріями. За повної відсутності записів відповідні діаграми відсутні.
    <br />
</p>

<h3>Звіти</h3>
<p style=""font-size:15px;"">
    <b>Імпорт</b>
    <br />
    Щоб заповнити словник записами із зовнішнього носія, потрібно завантажити Excel-файл (.xls, .xlsx) у визначеному форматі. ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "92d05531a33ac0581c210ba966ebbbfd06a3348e6903", async() => {
                WriteLiteral("Приклад звіту");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
    <br />
    Надсилання файлу з розширенням, відмінним від зазначених вище, відхиляється з поверненням на сторінку &quot;Звіти&quot; та повідомленням про помилку.
    <br />
    У звіті може бути декілька листів. Дані починають читатися з другого рядка (перший &#8211; заголовний). Читаються тільки перші чотири комірки (слово, мова, категорія, переклад). Переклади кожного зі слів перераховуються в четвертій комірці через кому.
    <br />
    Помилковими вважаються файли, які містять записи:
    <br />
    &#8211; з одним або більше порожнім полем;
    <br />
    &#8211; з полями, довжина яких перевищує 50 символів;
    <br />
    &#8211; слово, мова та категорія яких одночасно співпадають із такими для вже наявного запису;
    <br />
    &#8211; з перекладами, наведеними в некоректному форматі (див. &quot;Слова&quot;).
    <br />
    Одразу після надсилання звіту записи додаються в словник і здійснюється перенаправлення на головну сторінку.
    <br />
    <b>Експорт</b>
    <br />
    Перед завантаженням по");
            WriteLiteral(@"точних записів можна вибрати критерії фільтрації цих записів у відповідних випадних списках. Якщо дані за встановленими критеріями відсутні, завантаження файлу відхиляється з повідомленням про помилку.
    <br />
    Файл генерується в тому ж самому форматі, який було описано вище.
</p>

<a>
    ");
#nullable restore
#line 91 "D:\Женя\Projects\C# Projects\Dictionary\Lexiconn\Views\Help\Index.cshtml"
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
