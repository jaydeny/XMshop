﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XM.IDAL
{
    public interface IFirstDAL
    {
        IEnumerable<T>GetStore<T>(Dictionary<string,object>paras);
    }
}
