using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFramework.Data
{
    public interface IDb<T>
    {
        bool Add(T t);

        bool Update(T t, int id);

        bool Delete(int id);

        T Get(int id);
    }
}
