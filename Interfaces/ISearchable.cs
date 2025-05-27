using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFLS.Interfaces
{
    internal interface ISearchable
    {
        void PerformSearch(string query);
        void EnableSearch();
    }
}
