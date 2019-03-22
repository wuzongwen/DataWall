using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SysAdditionalData
    {
        [Key]
        [DisplayName("数据ID")]
        public int ID { get; set; }

        [DisplayName("数据类型:0客流数据,1借还数据")]
        public int Type { get; set; }

        [DisplayName("客流入馆人次/借书人次")]
        public int PeopleNum { get; set; }

        [DisplayName("借阅图书数量")]
        public int BookNum { get; set; }

        [DisplayName("还书人数")]
        public int StillPeopleNum { get; set; }

        [DisplayName("归还图书数量")]
        public int StillBookNum { get; set; }

        [DisplayName("数据时间")]
        public DateTime DataDatetime { get; set; }

        [DisplayName("资源所属馆Id")]
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
