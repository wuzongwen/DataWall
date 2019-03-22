using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class SysNewBook
    {
        [Key]
        [DisplayName("书籍ID")]
        public int ID { get; set; }

        [DisplayName("书籍名称")]
        public string BookName { get; set; }

        [DisplayName("书籍作者")]
        public string Author { get; set; }

        [DisplayName("出版社")]
        public string Press { get; set; }

        [DisplayName("封面")]
        public string Image { get; set; }

        [DisplayName("出版日期")]
        public string PublishDate { get; set; }

        [DisplayName("书籍描述")]
        public string BookDescribe { get; set; }

        [DisplayName("书籍ISBN号")]
        public string BookISBN { get; set; }

        [DisplayName("书籍所属馆Id")]
        public int SysLibraryId { get; set; }

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

        [JsonIgnore]
        public virtual SysLibrary SysLibrary { get; set; }
    }
}
