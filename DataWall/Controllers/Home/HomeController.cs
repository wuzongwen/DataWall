using System.Linq;
using System.Web.Mvc;
using Model;
using DAL;
using Common;
using DAL.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Configuration;
using System.Text;
using System.Web;
using System.IO;
using Newtonsoft.Json.Serialization;
using DataWall.ViewModels;

namespace DataWall.Controllers.Home
{
    public class HomeController : Controller
    {
        IsysLogRepository Lg = new SysLogRepository();

        #region Home
        /// <summary>
        /// 前台首页
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult Index(string LibraryCode)
         {
            using (DataWallContext db = new DataWallContext())
            {
                string CookieKey = db.SysProgramInfos.AsNoTracking().FirstOrDefault().CookieSecretKey;
                if (!CheckAuthorization())
                {
                    MsgInfo msg = new MsgInfo
                    {
                        Title = "授权过期",
                        Content = "请购买正式版，或联系相关负责人延长试用期"
                    };
                    return RedirectToAction("Error", "Error", new { Msg = SecurityHelper.EncryptDES(JsonConvert.SerializeObject(msg), CookieKey) });
                }
                else
                {
                    string Code = SecurityHelper.DecryptDES(LibraryCode, CookieKey);
                    SysLibrary sysLibrary = db.SysLibrarys.Where(l => l.LibraryCode == Code).FirstOrDefault();
                    if (sysLibrary != null)
                    {
                        if (db.SysThemeSets.Where(s => s.SysLibraryId == sysLibrary.ID).FirstOrDefault() != null)
                        {
                            if (sysLibrary.DelState == 1)
                            {
                                MsgInfo msg = new MsgInfo
                                {
                                    Title = "场馆已删除",
                                    Content = "该场馆已删除，请联系系统管理员"
                                };
                                return RedirectToAction("Error", "Error", new { Msg = SecurityHelper.EncryptDES(JsonConvert.SerializeObject(msg), CookieKey) });
                            }
                            if (sysLibrary.IsEnable == 1)
                            {
                                MsgInfo msg = new MsgInfo
                                {
                                    Title = "场馆已禁用",
                                    Content = "该场馆已禁用，请联系系统管理员"
                                };
                                return RedirectToAction("Error", "Error", new { Msg = SecurityHelper.EncryptDES(JsonConvert.SerializeObject(msg), CookieKey) });
                            }
                            ViewBag.Title = sysLibrary.LibraryName;
                            CookieHelper.SetCookie("Library", SecurityHelper.EncryptDES(JsonConvert.SerializeObject(sysLibrary), CookieKey), DateTime.Now.AddYears(1));
                        }
                        else
                        {
                            MsgInfo msg = new MsgInfo
                            {
                                Title = "当前场馆未设置主题",
                                Content = "请到管理后台为当前场馆配置主题内容"
                            };
                            return RedirectToAction("Error", "Error", new { Msg = SecurityHelper.EncryptDES(JsonConvert.SerializeObject(msg), CookieKey) });
                        }
                    }
                    else
                    {
                        MsgInfo msg = new MsgInfo
                        {
                            Title = "验证失败",
                            Content = "请检查当前访问链接是否正确"
                        };
                        return RedirectToAction("Error", "Error", new { Msg = SecurityHelper.EncryptDES(JsonConvert.SerializeObject(msg), CookieKey) });
                    }
                    return View();
                }
            }
        }

        /// <summary>
        /// 获取模块配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetModuleSet()
        {
            using (DataWallContext db = new DataWallContext())
            {
                var Cookies = SecurityHelper.DecryptDES(CookieHelper.GetCookieValue("Library"), db.SysProgramInfos.AsNoTracking().FirstOrDefault().CookieSecretKey);
                if (Cookies != null & Cookies != "")
                {
                    SysLibrary Library = JsonConvert.DeserializeObject<SysLibrary>(Cookies);
                    var Module = db.SysThemeSets.Where(m => m.SysLibraryId == Library.ID).Select(m => new { m.SysSeting, m.SysStyle }).FirstOrDefault();
                    if (Module != null)
                    {
                        return Json(new { code = "200", data = Module }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { code = "202", msg = "没有数据" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        code = 202,
                        msg = "验证失败"
                    }, JsonRequestBehavior.AllowGet);
                }
                
            }
        }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <returns></returns>
        public ActionResult Content(int Type)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    var Cookies = SecurityHelper.DecryptDES(CookieHelper.GetCookieValue("Library"), db.SysProgramInfos.AsNoTracking().FirstOrDefault().CookieSecretKey);
                    if (Cookies != null & Cookies != "")
                    {
                        SysLibrary Library = JsonConvert.DeserializeObject<SysLibrary>(Cookies);
                        var Content = db.SysContents.Include("SysFiles").AsNoTracking().Where(c => c.SysLibraryId == Library.ID & c.Type == Type & c.IsEnable == 0 & c.DelState == 0).Select(c => new { c.ID, c.Title, c.ContentDescribe, c.Type, c.Sort, c.EditTime, c.SysFiles }).OrderBy(c => c.Sort).ToList();
                        if (Content != null)
                        {
                            var data = JsonConvert.SerializeObject(Content.Select(c => new { c.ID, c.Title, c.ContentDescribe, c.Type, c.Sort, c.EditTime }).FirstOrDefault());
                            var files = JsonConvert.SerializeObject(Content.FirstOrDefault().SysFiles.Where(f => f.DelState == 0 && f.FilePath != null));
                            return Json(new
                            {
                                code = 201,
                                data = data,
                                files = files
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new
                            {
                                code = 202,
                                msg = "没有数据"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            code = 202,
                            msg = "验证失败"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex.Message);
                return Json(new
                {
                    code = 202,
                    msg = "服务器错误"
                }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 获取历史上的今天
        /// </summary>
        /// <returns></returns>
        public ActionResult History()
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    int month = DateTime.Now.Month;
                    int day = DateTime.Now.Day;
                    var History = db.Historys.Where(h => h.month == month & h.day == day & h.pic.Contains(".")).AsNoTracking().ToList();
                    return Json(new
                    {
                        code = 201,
                        data = History
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex.Message);
                return Json(new
                {
                    code = 202,
                    data = "服务器错误"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取客流数据
        /// </summary>
        /// <returns></returns>
        public ActionResult CustData()
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    var Cookies = SecurityHelper.DecryptDES(CookieHelper.GetCookieValue("Library"), db.SysProgramInfos.AsNoTracking().FirstOrDefault().CookieSecretKey);
                    if (Cookies != null)
                    {
                        SysLibrary Library = JsonConvert.DeserializeObject<SysLibrary>(Cookies);
                        List<SysCustDevice> SysCustDevices = db.SysCustDevices.Where(u => u.DelState == 0).AsNoTracking().ToList();
                        var query = from a in db.SysCustDevices
                                    join b in db.SysCustDatas
                                    on a.ID equals b.SysCustDeviceId
                                    where a.SysLibraryId == Library.ID
                                    select b;

                        //额外数据
                        var AdditionalData = from a in db.SysAdditionalDatas where a.SysLibraryId == Library.ID & a.IsEnable == 0 & a.Type == 0 select a;
                        int AllNumEw = 0;
                        int LastMonthNumEw = 0;
                        int YesterdayNumEw = 0;
                        int ThisMonthNumEw = 0;
                        int TodayNumEw = 0;
                        if (AdditionalData != null)
                        {
                            AllNumEw = AdditionalData.Sum(d => (int?)d.PeopleNum) ?? 0;
                            LastMonthNumEw = AdditionalData.Where(s => DbFunctions.DiffMonths(s.DataDatetime, DateTime.Now) == -1).Sum(d => (int?)d.PeopleNum) ?? 0;
                            YesterdayNumEw = AdditionalData.Where(s => DbFunctions.DiffMonths(s.DataDatetime, DateTime.Now) == 0).Sum(d => (int?)d.PeopleNum) ?? 0;
                            ThisMonthNumEw = AdditionalData.Where(s => DbFunctions.DiffDays(s.DataDatetime, DateTime.Now) == -1).Sum(d => (int?)d.PeopleNum) ?? 0;
                            TodayNumEw = AdditionalData.Where(s => DbFunctions.DiffDays(s.DataDatetime, DateTime.Now) == 0).Sum(d => (int?)d.PeopleNum) ?? 0;
                        }

                        var AllNum = (query.Sum(d => (decimal?)d.D_InNum) ?? 0) + AllNumEw;
                        var LastMonthNum = (query.Where(d => DbFunctions.DiffMonths(d.D_Date, DateTime.Now) == -1).Sum(d => (decimal?)d.D_InNum) ?? 0) + LastMonthNumEw;
                        var ThisMonthNum = (query.Where(d => DbFunctions.DiffMonths(d.D_Date, DateTime.Now) == 0).Sum(d => (decimal?)d.D_InNum) ?? 0) + YesterdayNumEw;
                        var YesterdayNum = (query.Where(d => DbFunctions.DiffDays(d.D_Date, DateTime.Now) == -1).Sum(d => (decimal?)d.D_InNum) ?? 0) + ThisMonthNumEw;
                        var TodayNum = (query.Where(d => DbFunctions.DiffDays(d.D_Date, DateTime.Now) == 0).Sum(d => (decimal?)d.D_InNum) ?? 0) + TodayNumEw;

                        return Json(new
                        {
                            code = 201,
                            AllNum = AllNum,
                            LastMonthNum = LastMonthNum,
                            ThisMonthNum = ThisMonthNum,
                            YesterdayNum = YesterdayNum,
                            TodayNum
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            code = 202,
                            msg = "验证失败"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex.Message);
                return Json(new
                {
                    code = 202,
                    msg = "服务器错误"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取新书推荐
        /// </summary>
        /// <returns></returns>
        public ActionResult NewBook()
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    //消除序列化类型的对象时检测到循环引用问题
                    db.Configuration.ProxyCreationEnabled = false;

                    var Cookies = SecurityHelper.DecryptDES(CookieHelper.GetCookieValue("Library"), db.SysProgramInfos.AsNoTracking().FirstOrDefault().CookieSecretKey);
                    if (Cookies != null & Cookies != "")
                    {
                        SysLibrary Library = JsonConvert.DeserializeObject<SysLibrary>(Cookies);
                        var NewBook = db.SysNewBooks.Where(n => n.SysLibraryId == Library.ID).ToList();
                        return Json(new
                        {
                            code = 201,
                            data = NewBook
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            code = 202,
                            msg = "验证失败"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex.Message);
                return Json(new
                {
                    code = 202,
                    msg = "服务器错误"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Module
        /// <summary>
        /// 通知公告
        /// </summary>
        /// <returns></returns>
        public ActionResult 通知公告()
        {
            return View();
        }

        /// <summary>
        /// 风采展示
        /// </summary>
        /// <returns></returns>
        public ActionResult 风采展示()
        {
            return View();
        }

        /// <summary>
        /// 照片墙
        /// </summary>
        /// <returns></returns>
        public ActionResult 照片墙()
        {
            return View();
        }

        /// <summary>
        /// 新书推荐
        /// </summary>
        /// <returns></returns>
        public ActionResult 新书推荐()
        {
            return View();
        }

        /// <summary>
        /// 天气时间
        /// </summary>
        /// <returns></returns>
        public ActionResult 天气时间()
        {
            return View();
        }

        /// <summary>
        /// 客流统计
        /// </summary>
        /// <returns></returns>
        public ActionResult 客流统计()
        {
            return View();
        }

        /// <summary>
        /// 历史上的今天
        /// </summary>
        /// <returns></returns>
        public ActionResult 历史上的今天()
        {
            return View();
        }

        /// <summary>
        /// 分时借还
        /// </summary>
        /// <returns></returns>
        public ActionResult 分时借还()
        {
            return View();
        }

        /// <summary>
        /// 借阅统计
        /// </summary>
        /// <returns></returns>
        public ActionResult 借阅统计()
        {
            return View();
        }

        /// <summary>
        /// 客流分时统计
        /// </summary>
        /// <returns></returns>
        public ActionResult 客流分时统计()
        {
            return View();
        }

        /// <summary>
        /// 分时借还
        /// </summary>
        /// <returns></returns>
        public ActionResult 图书借阅排行榜()
        {
            return View();
        }

        #endregion

        #region Public
        /// <summary>
        /// 检查授权
        /// </summary>
        /// <returns></returns>
        public bool CheckAuthorization()
        {
            using (DataWallContext db = new DataWallContext())
            {
                SysProgramInfo sysProgram = db.SysProgramInfos.FirstOrDefault();
                if (sysProgram.Type == 0)
                {
                    return true;
                }
                else
                {
                    if (sysProgram.ExpirationDate > DateTime.Now)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        #endregion

    }

    public static class ObjectExtentions
    {
        public static string ToJsonString(this Object obj)
        {

            JsonSerializerSettings jsSettings = new JsonSerializerSettings();
            jsSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            return JsonConvert.SerializeObject(obj, jsSettings);
        }
    }
}