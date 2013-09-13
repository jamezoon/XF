using System.Collections.Generic;

using XFramework.Entity;

namespace XFramework.BLL
{
    public class ArticleBLL
    {
        public static PageData<ArticleEntity> GetList(int categoryID, int pageIndex, int pageSize)
        {
            return Data.ArticleData.GetList(categoryID, pageIndex, pageSize);
        }
    }
}
