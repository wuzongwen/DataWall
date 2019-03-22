using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class SysModule
    {
        [Key]
        [DisplayName("模块ID")]
        public int ID { get; set; }

        [DisplayName("模块名称")]
        public string Title { get; set; }

        [DisplayName("是否添加管理菜单")]
        public int IsAddToMenu { get; set; }

        [DisplayName("菜单链接,可空")]
        public string MenuLink { get; set; }

        [DisplayName("描述")]
        public string Describe { get; set; }

        [DisplayName("排序")]
        public int Sort { get; set; }

        [DisplayName("创建时间")]
        public DateTime CrateTime { get; set; }

        [DisplayName("修改时间")]
        public DateTime EditTime { get; set; }

        [DisplayName("启用状态:0启用,1禁用")]
        public int IsEnable { get; set; }

        [DisplayName("删除状态:0正常,1已删除")]
        public int? DelState { get; set; }
    }
}
