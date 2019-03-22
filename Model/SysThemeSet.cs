using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class SysThemeSet
    {
        [Key]
        [DisplayName("主题ID")]
        public int ID { get; set; }

        [DisplayName("主题样式")]
        public int SysStyle { get; set; }

        [DisplayName("主题设置")]
        public string SysSeting { get; set; }

        [DisplayName("所属场馆ID")]
        public int SysLibraryId { get; set; }

        [DisplayName("创建时间")]
        public DateTime CrateTime { get; set; }

        [DisplayName("修改时间")]
        public DateTime EditTime { get; set; }

        [DisplayName("启用状态:0启用,1禁用")]
        public int IsEnable { get; set; }

        [DisplayName("删除状态:0正常,1已删除")]
        public int? DelState { get; set; }

        public virtual SysLibrary SysLibrary { get; set; }
    }
}
