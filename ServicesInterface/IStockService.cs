using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesInterface
{
    public interface IStockService
    {
        void GetStockValue(string user, string staockName);
    }
}
