using System;
using System.Linq;
using System.Web.Mvc;
using DAL;
using DAL.Repositories;
using Common;
using Model;
using Model.ToolModels;
using DataWall.ViewModels;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataWall.Controllers
{
    public class AdminController : Controller
    {
        IsysLogRepository Lg = new SysLogRepository();

        /// <summary>
        /// 后台首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            using (DataWallContext db = new DataWallContext())
            {
                var Cookies = SecurityHelper.DecryptDES(CookieHelper.GetCookieValue("User"), db.SysProgramInfos.AsNoTracking().FirstOrDefault().CookieSecretKey);
                if (Cookies != "")
                {
                    UserCookie user = JsonConvert.DeserializeObject<UserCookie>(Cookies);
                    ViewBag.UserName = user.UserName;
                    ViewBag.RoleName = user.RoleName;
                }
                else
                {
                    return RedirectToAction("Login");
                }
                SysProgramInfo sysProgramInfo = db.SysProgramInfos.AsNoTracking().FirstOrDefault();
                ViewBag.Title = "数据墙后台管理系统" + sysProgramInfo.ProgramVersion;

                //获取菜单
                List<SysMenu> menuList = db.SysMenus.Where(m => m.IsEnable == 0 & m.DelState == 0).AsNoTracking().ToList();
                ViewData["datalist"] = menuList;

                //模块菜单
                List<SysModule> ModuleMenu = db.SysModules.Where(m => m.IsEnable == 0 & m.DelState == 0 & m.IsAddToMenu == 0).AsNoTracking().ToList();
                ViewData["modulelist"] = ModuleMenu;

                return View(sysProgramInfo);
            }
        }

        /// <summary>
        /// 欢迎页
        /// </summary>
        /// <returns></returns>
        public ActionResult Welcome()
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    List<SysLog> logList = db.SysLogs.Where(s => s.Modular != "User").OrderByDescending(u => u.CrateTime).Take(5).AsNoTracking().ToList();
                    ViewData["datalist"] = logList;
                    TimeSpan Time = DateTime.Now - db.SysProgramInfos.AsNoTracking().FirstOrDefault().FirstRunTime;
                    ViewBag.Time = Math.Round(Time.TotalHours, 0);

                    //获取软件信息
                    SysProgramInfo sysProgramInfo = db.SysProgramInfos.AsNoTracking().FirstOrDefault();
                    ViewBag.ProgramVersion = sysProgramInfo.ProgramVersion;
                    string Msg = string.Empty;
                    if (sysProgramInfo.Type == 1)
                    {
                        if (sysProgramInfo.ExpirationDate > DateTime.Now)
                        {
                            Msg = "当前版本为<span style='color:red;font-size:16px'><strong>试用版</strong></span>,到期时间:<span style='color:red;font-size:16px'><strong>" + sysProgramInfo.ExpirationDate.ToString("f") + "</strong></span>";
                        }
                        else
                        {
                            Msg = "当前版本<span style='color:red;font-size:16px'><strong>已过期</strong></span>,到期时间:<span style='color:red;font-size:16px'><strong>" + sysProgramInfo.ExpirationDate.ToString("f") + "</strong></span>";
                        }
                    }
                    ViewBag.Msg = Msg;

                    //获取服务器信息
                    ViewModels.SystemInfo systemInfo = JsonConvert.DeserializeObject<ViewModels.SystemInfo>(Common.SystemInfo.GetSystemInfo());

                    //可用模块
                    List<SysModule> Module = db.SysModules.Where(m => m.IsEnable == 0 & m.DelState == 0).AsNoTracking().ToList();
                    List<SysMenu> Menu = db.SysMenus.Where(m => m.IsEnable == 0 & m.DelState == 0).AsNoTracking().ToList();
                    foreach (var item in Menu)
                    {
                        SysModule sysModule = new SysModule()
                        {
                            Title = item.MenuName,
                            Describe=item.Describe,
                            Sort = item.Sort
                        };
                        Module.Add(sysModule);
                    }
                    ViewData["modulelist"] = Module.OrderBy(m => m.Sort).ToList();
                    return View(systemInfo);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("欢迎页加载失败:" + ex.Message);
                return Json(new { error = "页面加载失败，请联系管理员" });
            }
        }

        /// <summary>
        /// 登陆页
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            using (DataWallContext db = new DataWallContext())
            {
                SysProgramInfo sysProgramInfo = db.SysProgramInfos.AsNoTracking().FirstOrDefault();
                var Cookies = CookieHelper.GetCookieValue("User");
                if (Cookies != "")
                {
                    return RedirectToAction("Index");
                }
                return View(sysProgramInfo);
            }
        }

        #region 用户

        /// <summary>
        /// 验证登陆
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoginValidate(string UserName, string Password)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    string PasswordToMd5 = MD5Helper.MD5Encrypt32(Password);
                    SysUser user = db.SysUsers.FirstOrDefault(u => (u.UserName == UserName || u.Email == UserName) & u.Password == PasswordToMd5);
                    if (user != null)
                    {
                        if (user.DelState == 0)
                        {
                            if (user.IsEnable == 0)
                            {
                                string RoleName = "";
                                int RoleId = db.SysUserRoles.Where(u => u.SysUserID == user.ID).AsNoTracking().FirstOrDefault().SysRoleId;
                                SysRole Role = db.SysRoles.Where(su => su.ID == RoleId).AsNoTracking().FirstOrDefault();
                                RoleName = Role.RoleName;

                                SysProgramInfo sysProgramInfo = db.SysProgramInfos.FirstOrDefault();
                                CookieHelper.SetCookie("User", SecurityHelper.EncryptDES(JsonConvert.SerializeObject(new { UserId = user.ID, UserName = user.UserName, RoleName = RoleName }), db.SysProgramInfos.AsNoTracking().FirstOrDefault().CookieSecretKey), DateTime.Now.AddMinutes(sysProgramInfo.CookieRetentionTime));

                                Lg.AddLog("用户登陆", "User", 0, user.UserName);
                                
                                return Json(new { code = "200", data = new { token = new { UserId = user.ID, UserName = user.UserName, RoleName = RoleName, RoleId = RoleId } }, msg = "success" });
                            }
                            else
                            {
                                return Json(new { code = "202", data = new { }, msg = "该用户已禁用" });
                            }
                        }
                        else
                        {
                            return Json(new { code = "202", data = new { }, msg = "该用户已删除" });
                        }
                    }
                    else
                    {
                        return Json(new { code = "201", data = new { }, msg = "用户名或密码不正确" });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("用户登录错误:" + ex.Message);
                return Json(new { code = "202", data = new { }, msg = ex.Message });
            }
        }

        /// <summary>
        /// 注销登陆
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginOut()
        {
            CookieHelper.RemoveCookie("User");
            return RedirectToAction("Login");
        }

        #endregion 
    }
}