using Common;
using DAL;
using DataWall.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DataWall.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        // GET: Error
        public ActionResult Error404()
        {
            ViewBag.Title = "很抱歉！当前页面找不到了";
            return View();
        }

        // GET: ErrorLoginTimeout
        public ActionResult ErrorLoginTimeout()
        {
            ViewBag.Title = "登录超时";
            return View();
        }

        // GET: Error
        public ActionResult Error(string Msg)
        {
            using (DataWallContext db = new DataWallContext())
            {
                MsgInfo msgInfo = JsonConvert.DeserializeObject<MsgInfo>(SecurityHelper.DecryptDES(Msg, db.SysProgramInfos.FirstOrDefault().CookieSecretKey));
                ViewBag.Title = msgInfo.Title;
                ViewBag.Content = msgInfo.Content;
                return View();
            }  
        }
    }
}