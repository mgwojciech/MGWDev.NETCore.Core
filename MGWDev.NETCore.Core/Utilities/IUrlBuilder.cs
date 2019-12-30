using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MGWDev.NETCore.Core.Utilities
{
    public interface IUrlBuilder<T>
    {
        string BuildIdQuery<U>(string baseUrl, U id);
        string BuildFilterClause(Expression<Func<T, bool>> query);
        string BuildTop(int top);
        string BuildSkip(int skip);
        string BuildSelect();
        string BuildExpand();
    }
}
