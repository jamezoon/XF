using System.Collections.Generic;

using XFramework.Entity;

namespace XFramework.BLL
{
    public class CategoryBLL
    {
        public static CategoryEntity Get(int categoryID)
        {
            return Data.CategoryData.Get(categoryID);
        }
    }
}
