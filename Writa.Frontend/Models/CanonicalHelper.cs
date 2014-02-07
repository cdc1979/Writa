using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Writa.Frontend.Models
{
    public static class CanonicalHelper
    {
        public static MvcHtmlString CanonicalUrl(this HtmlHelper html)
        {
            var rawUrl = html.ViewContext.RequestContext.HttpContext.Request.Url;

            return CanonicalUrl(html, String.Format("{0}://{1}{2}", rawUrl.Scheme, rawUrl.Authority, rawUrl.AbsolutePath));
        }

        public static MvcHtmlString GetBaseUrl(this HtmlHelper html)
        {
            return new MvcHtmlString(html.ViewContext.RequestContext.HttpContext.Request.Url.Scheme + "://" + html.ViewContext.RequestContext.HttpContext.Request.Url.Authority + html.ViewContext.RequestContext.HttpContext.Request.ApplicationPath.TrimEnd('/') + "/");
        }

        public static MvcHtmlString CanonicalUrl(this HtmlHelper html, string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                var rawUrl = html.ViewContext.RequestContext.HttpContext.Request.Url;
                path = String.Format("{0}://{1}{2}", rawUrl.Scheme, rawUrl.Host, rawUrl.AbsolutePath);
            }

            path = path.ToLower();

            if (path.Count(c => c == '/') > 3)
            {
                path = path.TrimEnd('/');
            }

            if (path.EndsWith("/index"))
            {
                path = path.Substring(0, path.Length - 6);
            }

            var canonical = new TagBuilder("link");
            canonical.MergeAttribute("rel", "canonical");
            canonical.MergeAttribute("href", path);
            return new MvcHtmlString(canonical.ToString(TagRenderMode.SelfClosing));
        }
    }
}