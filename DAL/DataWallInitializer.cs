using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Model;
using System.IO;

namespace DAL
{
    public class DataWallInitializer:DropCreateDatabaseIfModelChanges<DataWallContext>
    {
        protected override void Seed(DataWallContext context)
        {
            //添加默认用户
            var sysUsers = new List<SysUser> {
                new SysUser{UserName="admin",Email="admin@qq.com",Password="7AA33D66415397B416E99821B7C3F13",DelState=0,IsEnable=0,CrateTime=DateTime.Now,EditTime=DateTime.Now },
                new SysUser{UserName="general",Email="general@qq.com",Password="7AA33D66415397B416E99821B7C3F13",DelState=0,IsEnable=0,CrateTime=DateTime.Now,EditTime=DateTime.Now }
            };
            sysUsers.ForEach(s => context.SysUsers.Add(s));
            context.SaveChanges();

            //添加系统信息
            var sysProgramInfos = new List<SysProgramInfo> {
                new SysProgramInfo{
                    ProgramVersion="2.0 build-20190201",MasterNum=1,BranchNum=1,Type=0,ExpirationDate=DateTime.Now.AddDays(30),CookieSecretKey="DataWall",CookieRetentionTime=60,UpdateTime=DateTime.Parse("2018-12-26 10:50:00"),FirstRunTime=DateTime.Now }
            };
            sysProgramInfos.ForEach(s => context.SysProgramInfos.Add(s));
            context.SaveChanges();

            //添加默认菜单
            var sysMenus = new List<SysMenu> {
                new SysMenu{MenuName="通知公告",Describe="通知公告内容展示",ContentType=2,Type=2,Sort=0,CrateTime=DateTime.Now,EditTime=DateTime.Now,IsEnable=0,DelState=0 },
                new SysMenu{MenuName="风采展示",Describe="视频内容展示",ContentType=1,Type=1,Sort=1,CrateTime=DateTime.Now,EditTime=DateTime.Now,IsEnable=0,DelState=0 },
                new SysMenu{MenuName="照片墙",Describe="图片内容展示",ContentType=0,Type=0,Sort=2,CrateTime=DateTime.Now,EditTime=DateTime.Now,IsEnable=0,DelState=0 }
            };
            sysMenus.ForEach(s => context.SysMenus.Add(s));
            context.SaveChanges();

            //添加权限
            var sysRoles = new List<SysRole> {
                new SysRole{ RoleName="Administrator",RoleDesc="超级管理员"},
                new SysRole{ RoleName="General",RoleDesc="一般用户"}
            };
            sysRoles.ForEach(s => context.SysRoles.Add(s));
            context.SaveChanges();

            var sysUserRoles = new List<SysUserRole> {
                new SysUserRole{ SysUserID=1,SysRoleId=1},
                new SysUserRole{ SysUserID=2,SysRoleId=2}
            };
            sysUserRoles.ForEach(s => context.SysUserRoles.Add(s));
            context.SaveChanges();

            var sysActionRoles = new List<SysActionRole> {
                //用户模块权限
                new SysActionRole{ControllerName="User",ActionName="List",User="Administrator"},
                new SysActionRole{ControllerName="User",ActionName="UserAdd",User="Administrator"},
                new SysActionRole{ControllerName="User",ActionName="UserEdit",User="Administrator"},
                new SysActionRole{ControllerName="User",ActionName="EditEnable",User="Administrator"},
                new SysActionRole{ControllerName="User",ActionName="PwdEdit",User="Administrator,General"},
                new SysActionRole{ControllerName="User",ActionName="DelUser",User="Administrator"},
                new SysActionRole{ControllerName="User",ActionName="DelUserAll",User="Administrator"},
                //场馆用户管理模块权限
                new SysActionRole{ControllerName="Library",ActionName="List",User="Administrator"},
                new SysActionRole{ControllerName="Library",ActionName="LibraryAdd",User="Administrator"},
                new SysActionRole{ControllerName="Library",ActionName="LibraryEdit",User="Administrator"},
                new SysActionRole{ControllerName="Library",ActionName="EditEnable",User="Administrator"},
                new SysActionRole{ControllerName="Library",ActionName="DelLibrary",User="Administrator"},
                new SysActionRole{ControllerName="Library",ActionName="DelLibraryAll",User="Administrator"},
                new SysActionRole{ControllerName="Library",ActionName="LibraryUser",User="Administrator"},
                new SysActionRole{ControllerName="Library",ActionName="DelLibraryUser",User="Administrator"},
                new SysActionRole{ControllerName="Library",ActionName="AddLibraryUser",User="Administrator"},
                //内容模块权限
                new SysActionRole{ControllerName="Content",ActionName="ContentList",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="DelContent",User="Administrator"},
                new SysActionRole{ControllerName="Content",ActionName="EditEnable",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="DelContentAll",User="Administrator"},
                new SysActionRole{ControllerName="Content",ActionName="NoticeAdd",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="NoticeEdit",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="VideoAdd",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="VideoEdit",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="VideoPlay",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="SlideAdd",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="SlideEdit",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="SlideImageEdit",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="DelImage",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="UploadForImageEdit",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="Upload",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="NewBookList",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="NewBookAdd",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="NewBookEdit",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="DelNewBook",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="NewBookEditEnable",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="DelNewBookAll",User="Administrator"},
                new SysActionRole{ControllerName="Content",ActionName="GetBookInfo",User="Administrator,General"},
                //其它模块
                new SysActionRole{ControllerName="Other",ActionName="AdditionalDataList",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="AdditionalDataEdit",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="ExcelDownload",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="UploadExcel",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="DelAdditionalData",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="AdditionalDataEditEnable",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="DelAdditionalDataAll",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="CustDeviceList",User="Administrator,General"},
                new SysActionRole{ControllerName="Other",ActionName="CustDeviceAdd",User="Administrator,General"},
                new SysActionRole{ControllerName="Other",ActionName="CustDeviceEdit",User="Administrator,General"},
                new SysActionRole{ControllerName="Other",ActionName="DelCustDevice",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="CustDeviceEditEnable",User="Administrator,General"},
                new SysActionRole{ControllerName="Other",ActionName="DelCustDeviceAll",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="LogList",User="Administrator"},
                //主题设置
                new SysActionRole{ControllerName="Module",ActionName="ModuleEdit",User="Administrator"},
                new SysActionRole{ControllerName="Module",ActionName="EditModule",User="Administrator"},
                new SysActionRole{ControllerName="Module",ActionName="ModuleStyle",User="Administrator"},
                new SysActionRole{ControllerName="Module",ActionName="GetSeting",User="Administrator"},
                new SysActionRole{ControllerName="Module",ActionName="Module1",User="Administrator"},
                new SysActionRole{ControllerName="Module",ActionName="Module2",User="Administrator"},
                new SysActionRole{ControllerName="Module",ActionName="Module3",User="Administrator"},
                new SysActionRole{ControllerName="Module",ActionName="Module4",User="Administrator"},
                new SysActionRole{ControllerName="Module",ActionName="Module5",User="Administrator"},
                new SysActionRole{ControllerName="Module",ActionName="Module6",User="Administrator"}
            };
            sysActionRoles.ForEach(s => context.SysActionRoles.Add(s));
            context.SaveChanges();

            //添加模块
            var sysModules = new List<SysModule> {
                new SysModule{Title="新书推荐",IsAddToMenu=0,MenuLink="/Content/NewBookList",Describe="场馆最新馆藏书籍推荐",Sort=0,CrateTime=DateTime.Now,EditTime=DateTime.Now,IsEnable=0,DelState=0 },
                new SysModule{Title="图书借阅排行榜",IsAddToMenu=1,MenuLink=null,Describe="场馆借阅热度书籍展示",Sort=0,CrateTime=DateTime.Now,EditTime=DateTime.Now,IsEnable=0,DelState=0 },
                new SysModule{Title="借阅统计",IsAddToMenu=1,MenuLink=null,Describe="场馆书籍借阅信息统计分析",Sort=0,CrateTime=DateTime.Now,EditTime=DateTime.Now,IsEnable=0,DelState=0 },
                new SysModule{Title="分时借还",IsAddToMenu=1,MenuLink=null,Describe="场馆书籍借阅信息按时间统计分析",Sort=0,CrateTime=DateTime.Now,EditTime=DateTime.Now,IsEnable=0,DelState=0 },
                new SysModule{Title="天气时间",IsAddToMenu=1,MenuLink=null,Describe="时间天气等信息展示",Sort=0,CrateTime=DateTime.Now,EditTime=DateTime.Now,IsEnable=0,DelState=0 },
                new SysModule{Title="客流统计",IsAddToMenu=1,MenuLink=null,Describe="场馆进出馆人次统计分析",Sort=0,CrateTime=DateTime.Now,EditTime=DateTime.Now,IsEnable=0,DelState=0 },
                new SysModule{Title="客流分时统计",IsAddToMenu=1,MenuLink=null,Describe="场馆进出馆人次按时间统计分析",Sort=0,CrateTime=DateTime.Now,EditTime=DateTime.Now,IsEnable=0,DelState=0 },
                new SysModule{Title="历史上的今天",IsAddToMenu=1,MenuLink=null,Describe="历史上的今日事件展示",Sort=0,CrateTime=DateTime.Now,EditTime=DateTime.Now,IsEnable=0,DelState=0 }
            };
            sysModules.ForEach(s => context.SysModules.Add(s));
            context.SaveChanges();

            //历史上的今天数据导入
            string sqlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "HIstoryData", "history.sql");
            StreamReader strRead = File.OpenText(sqlPath);
            string strContent = strRead.ReadToEnd();
            strRead.Close();
            context.Database.ExecuteSqlCommand(strContent);
        }
    }
}