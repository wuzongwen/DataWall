using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class SysCustData
    {
        [Key]
        [DisplayName("数据ID")]
        public int ID { get; set; }

        [DisplayName("设备ID")]
        public int SysCustDeviceId { get; set; }

        [DisplayName("数据时间")]
        public DateTime D_Date { get; set; }

        [DisplayName("进馆人数")]
        public int D_InNum { get; set; }

        [DisplayName("出馆人数")]
        public int D_OutNum { get; set; }

        public virtual SysCustDevice SysCustDevice { get; set; }
    }
}