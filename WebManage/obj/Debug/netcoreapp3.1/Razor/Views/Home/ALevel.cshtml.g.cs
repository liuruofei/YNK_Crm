#pragma checksum "D:\Dingzhihua2\WebManage\Views\Home\ALevel.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "d0548f31bcd789d286d689f9b1f4d2b669517d92"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_ALevel), @"mvc.1.0.view", @"/Views/Home/ALevel.cshtml")]
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
#line 1 "D:\Dingzhihua2\WebManage\Views\_ViewImports.cshtml"
using WebManage;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Dingzhihua2\WebManage\Views\_ViewImports.cshtml"
using WebManage.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d0548f31bcd789d286d689f9b1f4d2b669517d92", @"/Views/Home/ALevel.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"649054e3ac1db40e7bcbdbc26821fff4b68650e8", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_ALevel : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "D:\Dingzhihua2\WebManage\Views\Home\ALevel.cshtml"
  
    Layout = "/Views/Shared/_Templete.cshtml";
    var keyId = ViewBag.Id;
    var type = ViewBag.type;

#line default
#line hidden
#nullable disable
            WriteLiteral(@"<style>
    .front .section--content {
        background: transparent linear-gradient( 0deg,#f5f5f5,#f5f5f5);
        margin-top: initial;
    }

    .article__content .article__paragraphs > .entity-paragraphs-item {
        margin-left: initial;
        margin-right: initial;
        max-width: initial;
    }
</style>
<div id=""section--content"" class=""section section--content "">
    <div class=""section__inner-wrapper section__inner wrapper "">
        <div class=""section__content"">
            <div class=""region region__content brick--main region--content"">

                <article class=""node node-news article article--news article--view-full article-type--news prose clearfix"">

                    <div class=""article__content"">
                        <div class=""article__summary"" id=""acticleTitle"" style=""text-align:center"">
                        </div>
                        <div class=""article__paragraphs"">

                            <div class=""entity entity-paragraphs-item");
            WriteLiteral(@" paragraphs-item-text entity-paragraphs-item--narrow paragraphs-item-text--"">
                                <div class=""content wrapper"">
                                    <div class=""paragraphs_item__body prose"" id=""acticleContent"">


                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>

                </article>
            </div>
        </div>
    </div>
</div>
");
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script type=""text/javascript"">
        $(function () {
            $(""#muenUl"").find(""li"").find(""a"").removeClass(""activeA"");
            $(""#muenUl"").find(""li"").find(""a[data-class='alevel']"").addClass(""activeA"");
        })
        //绑定Alevel全日制
        ajaxRequst(""post"", ""Home/GetAlevelDetail"",{}, function (data) {
            if (data != null && data.code != undefined && data.code != null && data.data != null) {
                $(""#acticleTitle"").html(data.data.Title);
                $(""#acticleContent"").html(data.data.Content);
            }
        });
    </script>
");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
