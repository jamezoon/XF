using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFramework.Util
{
    /// <summary>
    /// Url格式
    /// </summary>
    public enum UrlFormat
    {
        /// <summary>
        /// 动态Url(不做任何处理)
        /// </summary>
        Dynamic1,

        /// <summary>
        /// 静态url，格式为url-pagenumber.后缀 
        /// </summary>
        Static1,

        /// <summary>
        ///  url-pagenumber-pagesize-all.html 
        /// </summary>
        Static2,

        /// <summary>
        /// 占位符格式的url,在url上自定义
        /// </summary>
        Placeholder
    }

    /// <summary>
    /// 分页实体
    /// </summary>
    public sealed class PaginationEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PaginationEntity()
        {
            UrlFormat = UrlFormat.Dynamic1;
            RecordCount = 0;
            PageSize = 10;
            CurrentPage = 1;
            IsShowAll = false;
            IsShowInfo = false;
            IsShowFirstPageAndLastPage = true;
            IsShowPreviousAndNext = true;
            IsShowPageNumber = true;
            ShowPageNumber = 10;
            IsShowGo = false;
            StaticExtension = "html";
            Url = string.Empty;
            OnlyOneIsShow = false;
            IsShowMore = true;
            IsShowSimplifyInfo = false;
            PreviousInnerText = "上一页";
            NextInnerText = "下一页";
            FirstInnerText = "首页";
            LastInnerText = "末页";
            IsDisabledPreviousAndNext = false;
            IsDisabledFirstPageAndLastPage = false;
        }

        /// <summary>
        /// Url格式
        /// </summary>
        public UrlFormat UrlFormat { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// 页面显示记录数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 跳转的Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 是否启用全部功能
        /// </summary>
        public bool IsShowAll { get; set; }

        /// <summary>
        /// 是否显示分页信息
        /// 如：总记录数：[ 1415 ]　 每页20条　　当前[ 1 ] / [ 71 ]
        /// </summary>
        public bool IsShowInfo { get; set; }

        /// <summary>
        /// 是否启用第一页和最末页功能
        /// </summary>
        public bool IsShowFirstPageAndLastPage { get; set; }

        /// <summary>
        /// 第一页InnerText
        /// </summary>
        public string FirstInnerText { get; set; }

        /// <summary>
        /// 最末页InnerText
        /// </summary>
        public string LastInnerText { get; set; }

        /// <summary>
        /// 是否启用上一页和下一页功能
        /// </summary>
        public bool IsShowPreviousAndNext { get; set; }

        /// <summary>
        /// 上一页InnerText
        /// </summary>
        public string PreviousInnerText { get; set; }

        /// <summary>
        /// 下一页InnerText
        /// </summary>
        public string NextInnerText { get; set; }

        /// <summary>
        /// 是否显示页码
        /// </summary>
        public bool IsShowPageNumber { get; set; }

        /// <summary>
        /// 显示页码的个数,-1为显示全部
        /// </summary>
        public int ShowPageNumber { get; set; }

        /// <summary>
        /// 是否启用go功能
        /// </summary>
        public bool IsShowGo { get; set; }

        /// <summary>
        /// 是否显示省略号
        /// </summary>
        public bool IsShowMore { get; set; }

        /// <summary>
        /// 静态后缀名(不需要加点)
        /// </summary>
        public string StaticExtension { get; set; }

        /// <summary>
        /// 仅存在一页是否显示
        /// </summary>
        public bool OnlyOneIsShow { get; set; }

        /// <summary>
        /// 是否显示简化的分页信息
        /// 1/71
        /// </summary>
        public bool IsShowSimplifyInfo { get; set; }

        /// <summary>
        /// 当上一页和下一页无法点击时是否要disabled
        /// </summary>
        public bool IsDisabledPreviousAndNext { get; set; }

        /// <summary>
        /// 当第一页和最后一页无法点击时是否要disabled
        /// </summary>
        public bool IsDisabledFirstPageAndLastPage { get; set; }

        public int Sort { get; set; }
    }

    /// <summary>
    /// 分页
    /// </summary>
    public static class Pagination
    {
        private const string PageMaster = "@pageInfo@pageFirst@pagePrevious@pageNumber@pageSimplifyInfo@pageNext@pageLast@pageAll@pageGo";

        private const string PageInfo = "<span class='pageinfo'><lable>@RecordCount条数据</lable><lable>每页@PageSize条</lable><lable>共@TotalPage页</lable></span>";

        private const string PageFirst = "<a class='pagefirst' @disabled @url>@FirstInnerText</a>";

        private const string PagePrevious = "<a class='pageprev' @disabled @url>@PreviousInnerText</a>";

        private const string PageIndex = "<a class='pageindex' @url>@index</a>";

        private const string PageCurrentIndex = "<a class='pagecurrendindex' @url>@index</a>";

        private const string PageMore = "<a class='pagemore' @url>...</a>";

        private const string PageAll = "<a class='pageall' @url>全部</a>";

        private const string PageSimplifyInfo = "<span class='pagesimplifyinfo'>@CurrentPage/@TotalPage</span>";

        private const string PageNext = "<a class='pagenext' @disabled @url>@NextInnerText</a>";

        private const string PageLast = "<a class='pagelast' @disabled @url>@LastInnerText</a>";

        private const string PageGo = "<select id='pagego' class='pagego'>@Subset</select><script>document.getElementById('pagego').onchange=function(){window.location.href=this.value;}</script>";

        private const string PageGoSub = "<option @selected value='@value'>第@index页</option>";

        /// <summary>
        /// 获得分页内容
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public static string GetPage(PaginationEntity pagination)
        {
            if (pagination == null)
                return string.Empty;

            int pageCount = 1;
            if (pagination.RecordCount != 0)
            {
                if (pagination.RecordCount % pagination.PageSize == 0)
                {
                    pageCount = pagination.RecordCount / pagination.PageSize; // 获得总页数
                }
                else
                {
                    pageCount = (pagination.RecordCount / pagination.PageSize) + 1; // 获得总页数
                }
            }

            if (pageCount <= 1 && !pagination.OnlyOneIsShow)
                return string.Empty;

            if (pageCount < pagination.CurrentPage)
                pagination.CurrentPage = pageCount;

            string pageMaster = PageMaster;

            pageMaster = pageMaster.Replace("@pageInfo", GetPageInfo(pagination, pageCount));
            pageMaster = pageMaster.Replace("@pageFirst", GetPageFirst(pagination));
            pageMaster = pageMaster.Replace("@pagePrevious", GetPagePrevious(pagination));
            pageMaster = pageMaster.Replace("@pageNumber", GetPageNumber(pagination, pageCount));
            pageMaster = pageMaster.Replace("@pageSimplifyInfo", GetPageSimplifyInfo(pagination, pageCount));
            pageMaster = pageMaster.Replace("@pageAll", GetPageAll(pagination));
            pageMaster = pageMaster.Replace("@pageNext", GetPageNext(pagination, pageCount));
            pageMaster = pageMaster.Replace("@pageLast", GetPageLast(pagination, pageCount));
            pageMaster = pageMaster.Replace("@pageGo", GetPageGo(pagination, pageCount));

            return pageMaster;
        }

        /// <summary>
        /// 获取分页信息
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="totalPage"></param>
        /// <returns></returns>
        private static string GetPageInfo(PaginationEntity pagination, int totalPage)
        {
            if (!pagination.IsShowInfo)
                return string.Empty;

            string pageInfo = PageInfo;

            pageInfo = pageInfo.Replace("@RecordCount", pagination.RecordCount.ToString());
            pageInfo = pageInfo.Replace("@PageSize", pagination.PageSize.ToString());
            pageInfo = pageInfo.Replace("@CurrentPage", pagination.CurrentPage.ToString());
            pageInfo = pageInfo.Replace("@TotalPage", totalPage.ToString());

            return pageInfo;
        }

        /// <summary>
        /// 获取分页信息
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="totalPage"></param>
        /// <returns></returns>
        private static string GetPageSimplifyInfo(PaginationEntity pagination, int totalPage)
        {
            if (!pagination.IsShowSimplifyInfo)
                return string.Empty;

            string pageSimplifyInfo = PageSimplifyInfo;
            pageSimplifyInfo = pageSimplifyInfo.Replace("@CurrentPage", pagination.CurrentPage.ToString());
            pageSimplifyInfo = pageSimplifyInfo.Replace("@TotalPage", totalPage.ToString());
            return pageSimplifyInfo;
        }

        /// <summary>
        /// 获取跳到第一页
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        private static string GetPageFirst(PaginationEntity pagination)
        {
            if (!pagination.IsShowFirstPageAndLastPage)
                return string.Empty;

            string pageFirst = PageFirst;
            string url;
            // 如果为第一页，就不跳转
            if (pagination.CurrentPage <= 1)
            {
                if (pagination.IsDisabledFirstPageAndLastPage)
                {
                    pageFirst = pageFirst.Replace("@disabled", "disabled=disabled");
                    url = string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                pageFirst = pageFirst.Replace("@disabled", string.Empty);
                url = GetUrl(pagination, 1);
                if (!string.IsNullOrEmpty(url))
                {
                    url = "href='" + url + "'";
                }
            }

            pageFirst = pageFirst.Replace("@url", url);
            pageFirst = pageFirst.Replace("@FirstInnerText", pagination.FirstInnerText);
            return pageFirst;
        }

        /// <summary>
        /// 获取跳到上一页
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        private static string GetPagePrevious(PaginationEntity pagination)
        {
            if (!pagination.IsShowPreviousAndNext)
                return string.Empty;

            string pagePrevious = PagePrevious;
            string url;
            if (pagination.CurrentPage <= 1)
            {
                if (pagination.IsDisabledPreviousAndNext)
                {
                    pagePrevious = pagePrevious.Replace("@disabled", "disabled=disabled");
                    url = string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                pagePrevious = pagePrevious.Replace("@disabled", string.Empty);
                url = GetUrl(pagination, pagination.CurrentPage - 1);
                if (!string.IsNullOrEmpty(url))
                {
                    url = "href='" + url + "'";
                }
            }

            pagePrevious = pagePrevious.Replace("@url", url);
            pagePrevious = pagePrevious.Replace("@PreviousInnerText", pagination.PreviousInnerText);
            return pagePrevious;
        }

        /// <summary>
        /// 获取url
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="pageIndex"></param>
        /// <param name="isAll">是否为全部url</param>
        /// <returns></returns>
        private static string GetUrl(PaginationEntity pagination, int pageIndex, bool isAll = false)
        {
            if (pagination == null)
                return string.Empty;

            switch (pagination.UrlFormat)
            {
                case UrlFormat.Dynamic1:
                    // 如果有参数则直接加page参数
                    string url;
                    if (pagination.Url.Contains("?"))
                    {
                        url = pagination.Url + "&page=" + pageIndex;
                    }
                    else
                    {
                        url = pagination.Url + "?page=" + pageIndex;
                    }
                    return url;
                case UrlFormat.Static1:
                    return pagination.Url + "-" + pageIndex + "." + pagination.StaticExtension.Replace(".", string.Empty);
                case UrlFormat.Static2:
                    if (isAll) return pagination.Url + "-0-0-1." + pagination.StaticExtension.Replace(".", string.Empty);
                    return pagination.Url + "-" + pageIndex + "-" + pagination.PageSize + "-0." + pagination.StaticExtension.Replace(".", string.Empty);
                case UrlFormat.Placeholder:
                    return pagination.Url.Replace("{page}", pageIndex.ToString()).Replace("{pagesize}", pagination.PageSize.ToString()).Replace("{sort}", pagination.Sort.ToString());
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取分页数字
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="totalPage"></param>
        /// <returns></returns>
        private static string GetPageNumber(PaginationEntity pagination, int totalPage)
        {
            if (pagination.IsShowPageNumber == false) return string.Empty;

            int number = pagination.ShowPageNumber == -1
                             ? totalPage
                             : pagination.ShowPageNumber > 0 ? pagination.ShowPageNumber : 10;
            var numberHtml = new StringBuilder();
            if (number > totalPage || pagination.ShowPageNumber == -1)
            {
                for (int i = 1; i <= totalPage; i++)
                {
                    string pageIndex = PageIndex;
                    string pageCurrentIndex = PageCurrentIndex;
                    string url = GetUrl(pagination, i);
                    if (!string.IsNullOrEmpty(url))
                    {
                        url = "href='" + url + "'";
                    }

                    if (i == pagination.CurrentPage)
                    {
                        pageCurrentIndex = pageCurrentIndex.Replace("@url", string.Empty);
                        pageCurrentIndex = pageCurrentIndex.Replace("@index", i.ToString());
                        numberHtml.Append(pageCurrentIndex);
                    }
                    else
                    {
                        pageIndex = pageIndex.Replace("@url", url);
                        pageIndex = pageIndex.Replace("@index", i.ToString());
                        numberHtml.Append(pageIndex);
                    }
                }
            }
            else
            {
                // 如果设置显示部分页码就将当前页显示在页码中间
                var avgNumber = pagination.ShowPageNumber == -1 ? number : number / 2;

                int before = avgNumber;
                int after = number - avgNumber - 1;

                if (pagination.CurrentPage - before < 1)
                {
                    while (pagination.CurrentPage - before < 1)
                    {
                        before -= 1;
                        after += 1;
                    }
                }
                else
                {
                    while (pagination.CurrentPage + after > totalPage)
                    {
                        before += 1;
                        after -= 1;
                    }
                }

                // 如果前半部分还有页码未显示就显示省略号
                if (pagination.CurrentPage - before > 1 && pagination.IsShowMore)
                {
                    string pageMore = PageMore;
                    string url = GetUrl(pagination, pagination.CurrentPage - before - 1);
                    if (!string.IsNullOrEmpty(url))
                    {
                        url = "href='" + url + "'";
                    }

                    pageMore = pageMore.Replace("@url", url);
                    numberHtml.Append(pageMore);
                }

                // 显示前半部分页码
                for (int i = before; i > 0; i--)
                {
                    string pageIndex = PageIndex;
                    string url = GetUrl(pagination, pagination.CurrentPage - i);
                    if (!string.IsNullOrEmpty(url))
                    {
                        url = "href='" + url + "'";
                    }

                    pageIndex = pageIndex.Replace("@url", url);
                    pageIndex = pageIndex.Replace("@index", (pagination.CurrentPage - i).ToString());
                    numberHtml.Append(pageIndex);
                }

                string pageCurrentIndex = PageCurrentIndex;
                pageCurrentIndex = pageCurrentIndex.Replace("@url", string.Empty);
                pageCurrentIndex = pageCurrentIndex.Replace("@index", pagination.CurrentPage.ToString());
                numberHtml.Append(pageCurrentIndex);

                // 显示后半部分页码
                for (int i = 1; i <= after; i++)
                {
                    string pageIndex = PageIndex;

                    string url = GetUrl(pagination, pagination.CurrentPage + i);
                    if (!string.IsNullOrEmpty(url))
                    {
                        url = "href='" + url + "'";
                    }

                    pageIndex = pageIndex.Replace("@url", url);
                    pageIndex = pageIndex.Replace("@index", (pagination.CurrentPage + i).ToString());
                    numberHtml.Append(pageIndex);
                }
                // 如何后半部分还有页码未显示则显示省略号
                if (pagination.CurrentPage + after < totalPage && pagination.IsShowMore)
                {
                    string pageIndex = PageIndex;
                    string url = GetUrl(pagination, pagination.CurrentPage + after + 1);
                    if (!string.IsNullOrEmpty(url))
                    {
                        url = "href='" + url + "'";
                    }

                    pageIndex = pageIndex.Replace("@url", url);
                    pageIndex = pageIndex.Replace("@index", "...");
                    numberHtml.Append(pageIndex);
                }
            }

            return numberHtml.ToString();
        }

        /// <summary>
        /// 获取所有页码
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        private static string GetPageAll(PaginationEntity pagination)
        {
            if (!pagination.IsShowAll) return string.Empty;

            return PageAll.Replace("@url", "href='" + GetUrl(pagination, pagination.CurrentPage, true) + "'");
        }

        /// <summary>
        /// 获取下一页
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="totalPage"></param>
        /// <returns></returns>
        private static string GetPageNext(PaginationEntity pagination, int totalPage)
        {
            if (!pagination.IsShowPreviousAndNext) return string.Empty;

            string pageNext = PageNext;
            string url;
            if (pagination.CurrentPage >= totalPage)
            {
                if (pagination.IsDisabledPreviousAndNext)
                {
                    pageNext = pageNext.Replace("@disabled", "disabled=disabled");
                    url = string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                pageNext = pageNext.Replace("@disabled", string.Empty);
                url = GetUrl(pagination, pagination.CurrentPage + 1);
                if (!string.IsNullOrEmpty(url))
                {
                    url = "href='" + url + "'";
                }
            }

            pageNext = pageNext.Replace("@url", url);
            pageNext = pageNext.Replace("@NextInnerText", pagination.NextInnerText);
            return pageNext;
        }

        /// <summary>
        /// 获取最后一页
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="totalPage"></param>
        /// <returns></returns>
        private static string GetPageLast(PaginationEntity pagination, int totalPage)
        {
            if (pagination.IsShowFirstPageAndLastPage == false) return string.Empty;

            string pageLast = PageLast;
            string url;
            if (pagination.CurrentPage >= totalPage)
            {
                if (pagination.IsDisabledFirstPageAndLastPage)
                {
                    pageLast = pageLast.Replace("@disabled", "disabled=disabled");
                    url = string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                pageLast = pageLast.Replace("@disabled", string.Empty);
                url = GetUrl(pagination, totalPage);
                if (!string.IsNullOrEmpty(url))
                {
                    url = "href='" + url + "'";
                }
            }

            pageLast = pageLast.Replace("@url", url);
            pageLast = pageLast.Replace("@LastInnerText", pagination.LastInnerText);
            return pageLast;
        }

        /// <summary>
        /// 获取跳转页
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="totalPage"></param>
        /// <returns></returns>
        private static string GetPageGo(PaginationEntity pagination, int totalPage)
        {
            if (!pagination.IsShowGo) return string.Empty;

            var subHtml = new StringBuilder();
            for (int i = 1; i <= totalPage; i++)
            {
                string pageGoSub = PageGoSub;
                pageGoSub = pageGoSub.Replace("@selected", i == pagination.CurrentPage ? "selected" : string.Empty);
                pageGoSub = pageGoSub.Replace("@value", GetUrl(pagination, i));
                pageGoSub = pageGoSub.Replace("@index", i.ToString());
                subHtml.Append(pageGoSub);
            }

            string pageGo = PageGo;
            pageGo = pageGo.Replace("@Subset", subHtml.ToString());
            return pageGo;
        }
    }
}
