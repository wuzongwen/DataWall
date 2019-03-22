using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class SysProgramInfo
    {
        [Key]
        [DisplayName("ID")]
        public int ID { get; set; }

        [DisplayName("系统版本号")]
        public string ProgramVersion { get; set; }

        [DisplayName("总馆数量")]
        public int MasterNum { get; set; }

        [DisplayName("分馆数量")]
        public int BranchNum { get; set; }

        [DisplayName("程序类型0:正式;1试用")]
        public int Type { get; set; }

        [DisplayName("到期时间,单位/分钟")]
        public DateTime ExpirationDate { get; set; }

        [DisplayName("Cookie密钥")]
        public string CookieSecretKey { get; set; }

        [DisplayName("登录保持时长")]
        public int CookieRetentionTime { get; set; }

        [DisplayName("更新日期")]
        public DateTime UpdateTime { get; set; }

        [DisplayName("首次运行时间")]
        public DateTime FirstRunTime { get; set; }
    }
}
