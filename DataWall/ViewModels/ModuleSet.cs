using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataWall.ViewModels
{
    public class ModuleSet
    {
        /// <summary>
        /// 模块设置
        /// </summary>
        public List<List<string>> Module { get; set; }

        /// <summary>
        /// 时间设置
        /// </summary>
        public List<string> Time { get; set; }
    }
}