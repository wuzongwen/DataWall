using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using DAL;
using Common;
using DAL.Repositories;
using System.Data.Entity;
using System.Collections;
using Newtonsoft.Json;
using Model.ToolModels;
using DataWall.SignalR;

namespace DataWall.Controllers.Admin
{
    [CustomAuthorize]//权限验证
    public class ModuleController : Controller
    {
        IsysLogRepository Lg = new SysLogRepository();

        /// <summary>
        /// 模块编辑
        /// </summary>
        /// <param name="Style">样式</param>
        /// <param name="Id">场馆Id</param>
        /// <returns></returns>
        public ActionResult ModuleEdit(int? Style, int Id)
        {
            using (DataWallContext db = new DataWallContext())
            {
                int ModuleStyle = 0;
                if (Style > 0)
                {
                    ModuleStyle = Convert.ToInt32(Style);
                }
                else
                {

                    var sysModule = db.SysThemeSets.Where(m => m.SysLibraryId == Id).AsNoTracking().FirstOrDefault();
                    ModuleStyle = sysModule.SysStyle;
                }
                ViewBag.LibraryId = Id;

                return View();
            }
        }

        /// <summary>
        /// 修改主题
        /// </summary>
        /// <param name="Set">设置</param>
        /// <param name="Id">场馆Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditModule(string Set,int Id)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    var Module = db.SysThemeSets.Where(m => m.SysLibraryId == Id).AsNoTracking().FirstOrDefault();
                    if (Module != null)
                    {
                        SysThemeSet sysModule = new SysThemeSet()
                        {
                            ID = Module.ID,
                            SysSeting = Set,
                            EditTime = DateTime.Now,
                        };

                        db.Entry(sysModule).State = EntityState.Modified;
                        //不更新的字段
                        db.Entry(sysModule).Property(x => x.SysStyle).IsModified = false;
                        db.Entry(sysModule).Property(x => x.SysLibraryId).IsModified = false;
                        db.Entry(sysModule).Property(x => x.CrateTime).IsModified = false;
                        db.Entry(sysModule).Property(x => x.IsEnable).IsModified = false;
                        db.Entry(sysModule).Property(x => x.DelState).IsModified = false;
                        db.SaveChanges();
                    }
                    else
                    {
                        SysThemeSet sysModule = new SysThemeSet()
                        {
                            SysStyle = 1,
                            SysSeting = Set,
                            SysLibraryId = Id,
                            CrateTime = DateTime.Now,
                            EditTime = DateTime.Now,
                            IsEnable = 0,
                            DelState = 0
                        };
                        db.SysThemeSets.Add(sysModule);
                        //添加
                        db.SaveChanges();
                    }

                    Lg.AddLog("修改主题", "Module", 2, GetUserName());

                    //推送更新
                    PushUpdate(Id, "主题更新", 6);

                    return Json(new { code = "200", msg = "修改成功!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("内容状态修改失败:" + ex.Message);
                return Json(new { code = "201", msg = "修改失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 模块样式
        /// </summary>
        /// <param name="Id">场馆Id</param>
        /// <returns></returns>
        public ActionResult ModuleStyle(int Id)
        {
            using (DataWallContext db = new DataWallContext())
            {
                var sysModules = db.SysThemeSets.Where(m => m.SysLibraryId == Id).AsNoTracking().ToList();
                if (sysModules.Count() > 0)
                {
                    return RedirectToAction("ModuleEdit", "Module", new { id = 1 });
                }
                else
                {
                    ViewBag.Id = Id;
                    return View();
                }
            }
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="Id">场馆Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSeting(int Id)
        {
            using (DataWallContext db = new DataWallContext())
            {
                var Module = db.SysThemeSets.Where(m => m.SysLibraryId == Id).Select(m => new { m.SysSeting, m.SysStyle }).FirstOrDefault();
                if (Module != null)
                {
                    return Json(new { code = "200", data = Module });
                }
                else
                {
                    return Json(new { code = "201", msg = "请为该场馆配置模块功能" });
                }
            }
        }

        /// <summary>
        /// 模板1
        /// </summary>
        /// <param name="Id">场馆Id</param>
        /// <returns></returns>
        public ActionResult Module1(int Id)
        {
            using (DataWallContext db = new DataWallContext())
            {
                //可用模块
                List<SysModule> Module = db.SysModules.Where(m => m.IsEnable == 0 & m.DelState == 0).AsNoTracking().ToList();
                List<SysMenu> Menu = db.SysMenus.Where(m => m.IsEnable == 0 & m.DelState == 0).AsNoTracking().ToList();
                foreach (var item in Menu)
                {
                    SysModule sysModule = new SysModule()
                    {
                        Title = item.MenuName,
                        Sort = item.Sort
                    };
                    Module.Add(sysModule);
                }
                ViewData["datalist"] = Module.OrderBy(m => m.Sort).ToList();
                ViewBag.LibraryId = Id;
                return View();
            }
        }

        /// <summary>
        /// 模板2
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Module2()
        {
            return View();
        }

        /// <summary>
        /// 模板3
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Module3()
        {
            return View();
        }

        /// <summary>
        /// 模板4
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Module4()
        {
            return View();
        }
        /// <summary>
        /// 模板5
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Module5()
        {
            return View();
        }

        /// <summary>
        /// 模板6
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Module6()
        {
            return View();
        }

        /// <summary>
        /// 推送更新
        /// </summary>
        public void PushUpdate(int LibraryId, string msg, int type)
        {
            using (DataWallContext db = new DataWallContext())
            {
                string ToLibraryName = db.SysLibrarys.Find(LibraryId).LibraryName;
                MyHub.Show(ToLibraryName, JsonConvert.SerializeObject(new
                {
                    msg = msg,
                    action = "Notice",
                    type = type
                }));
            }
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <returns></returns>
        public string GetUserName()
        {
            using (DataWallContext db = new DataWallContext())
            {
                //获取当前登陆用户
                var Cookies = SecurityHelper.DecryptDES(CookieHelper.GetCookieValue("User"), db.SysProgramInfos.AsNoTracking().FirstOrDefault().CookieSecretKey);
                UserCookie user = JsonConvert.DeserializeObject<UserCookie>(Cookies);
                return user.UserName;
            }
        }
    }
}