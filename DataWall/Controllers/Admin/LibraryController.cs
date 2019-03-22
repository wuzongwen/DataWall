using Common;
using DAL;
using Model;
using DAL.Repositories;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Model.ToolModels;
using Newtonsoft.Json;

namespace DataWall.Controllers.Admin
{
    [CustomAuthorize]//权限验证
    public class LibraryController : Controller
    {
        IsysLogRepository Lg = new SysLogRepository();

        /// <summary>
        /// 场馆列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult List(int? page)
        {
            using (DataWallContext db = new DataWallContext())
            {
                List<SysLibrary> Liabrary = db.SysLibrarys.Where(u => u.DelState == 0).AsNoTracking().ToList();
                ViewBag.DataCout = Liabrary.Count();

                ViewBag.Key = db.SysProgramInfos.AsNoTracking().FirstOrDefault().CookieSecretKey;

                //第几页
                int pageNumber = page ?? 1;

                //每页显示多少条
                int pageSize = 10;

                //根据ID排序
                Liabrary = Liabrary.OrderBy(u => u.ID).ToList();

                //通过ToPagedList扩展方法进行分页
                IPagedList<SysLibrary> pagedList = Liabrary.ToPagedList(pageNumber, pageSize);

                //if (Users.Count < 10) {
                //    page
                //}

                //将分页处理后的列表传给View
                return View(pagedList);
            }
        }

        /// <summary>
        /// 添加场馆页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult LibraryAdd()
        {
            using (DataWallContext db = new DataWallContext())
            {
                ViewData["datalist"] = db.SysLibrarys.Where(l => l.IsEnable == 0 & l.DelState == 0 & l.Type == 0).AsNoTracking().ToList();
                return View();
            }
        }

        /// <summary>
        /// 添加场馆
        /// </summary>
        /// <param name="Library">场馆对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LibraryAdd(FormCollection Library)
        {
            try
            {
                if (Check("LibraryAdd", 0, Library["LibraryName"]))
                {
                    using (DataWallContext db = new DataWallContext())
                    {
                        int Type = int.Parse(Library["Type"]);
                        SysProgramInfo sysProgramInfo = db.SysProgramInfos.AsNoTracking().FirstOrDefault();
                        List<SysLibrary> sysLibraryList = db.SysLibrarys.AsNoTracking().Where(s => s.DelState == 0).ToList();
                        switch (Type)
                        {
                            case 0:
                                if (sysLibraryList.Where(s => s.Type == 0).Count() >= sysProgramInfo.MasterNum)
                                {
                                    return Json(new { code = "202", msg = "总馆数量已达上限" });
                                }
                                break;
                            case 1:
                                if (sysLibraryList.Where(s => s.Type == 1).Count() >= sysProgramInfo.BranchNum)
                                {
                                    return Json(new { code = "202", msg = "分馆数量已达上限" });
                                }
                                break;
                        }
                        //场馆
                        int FatherLibraryId = 0;
                        if (Type == 0)
                        {
                            FatherLibraryId = 0;
                        }
                        else
                        {
                            if (Library["FatherLibraryId"] == null)
                            {
                                return Json(new { code = "201", msg = "请选择上级场馆" });
                            }
                            FatherLibraryId = int.Parse(Library["FatherLibraryId"]);
                        }
                        string LibraryName = Library["LibraryName"];
                        string LibraryCode = PingYinHelper.GetFirstSpell(Library["LibraryName"]) + DateTime.Now.Millisecond;
                        SysLibrary sysLibrary = new SysLibrary()
                        {
                            City = "上海",
                            LibraryName = LibraryName,
                            FatherLibraryId = FatherLibraryId,
                            LibraryDescribe = HttpUtility.UrlDecode(Library["Describe"]).Replace("'", "‘"),
                            Type = Type,
                            LibraryCode = LibraryCode,
                            CrateTime = DateTime.Now,
                            EditTime = DateTime.Now,
                            IsEnable = 0,
                            DelState = 0
                        };

                        db.SysLibrarys.Add(sysLibrary);
                        //添加
                        db.SaveChanges();

                        //添加管理员为管理用户
                        int SysLibraryId = sysLibrary.ID;
                        List<SysLibraryUser> sysLibraryUsers = new List<SysLibraryUser> {
                         new SysLibraryUser{ SysUserID=1,SysLibraryId=SysLibraryId }
                        };
                        sysLibraryUsers.ForEach(s => db.SysLibraryUsers.Add(s));

                        //添加
                        db.SaveChanges();

                        Lg.AddLog("添加场馆", "Library", 1, GetUserName());

                        return Json(new { code = "200", msg = "添加成功!" });
                    }
                }
                else
                {
                    return Json(new { code = "202", msg = "场馆已存在!" });
                }
            }
            catch (Exception ex)
            {
                if (Library.Count > 0)
                {
                    LogHelper.ErrorLog("场馆添加失败:" + ex.Message);
                }

                return Json(new { code = "201", msg = "添加失败，请重试或联系管理员!" });
            }
        }

        /// <summary>
        /// 修改场馆
        /// </summary>
        /// <param name="id">场馆ID</param>
        /// <returns></returns>
        public ActionResult LibraryEdit(int id)
        {
            using (DataWallContext db = new DataWallContext())
            {
                ViewData["datalist"] = db.SysLibrarys.Where(l => l.IsEnable == 0 & l.DelState == 0 & l.Type == 0).AsNoTracking().ToList();
                SysLibrary sysLibrary = db.SysLibrarys.Find(id);
                return View(sysLibrary);
            }
        }

        /// <summary>
        /// 修改场馆
        /// </summary>
        /// <param name="Library">场馆对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LibraryEdit(FormCollection Library)
        {
            try
            {
                if (Check("LibraryEdit", int.Parse(Library["ID"]), Library["LibraryName"]))
                {
                    using (DataWallContext db = new DataWallContext())
                    {
                        int LbraryId = int.Parse(Library["ID"]);
                        int Type = int.Parse(Library["Type"]);
                        SysProgramInfo sysProgramInfo = db.SysProgramInfos.AsNoTracking().FirstOrDefault();
                        List<SysLibrary> sysLibraryList = db.SysLibrarys.AsNoTracking().Where(s => s.DelState == 0).ToList();
                        switch (Type)
                        {
                            case 0:
                                if (sysLibraryList.Where(s => s.Type == 0).Count() >= sysProgramInfo.MasterNum)
                                {
                                    if (sysLibraryList.Where(s => s.Type == 0 & s.ID == LbraryId & s.DelState == 0).Count() < 1)
                                    {
                                        return Json(new { code = "202", msg = "总馆数量已达上限" });
                                    }
                                }
                                break;
                            case 1:
                                if (sysLibraryList.Where(s => s.Type == 1).Count() >= sysProgramInfo.BranchNum)
                                {
                                    if (sysLibraryList.Where(s => s.Type == 1 & s.ID == LbraryId & s.DelState == 0).Count() < 1)
                                    {
                                        return Json(new { code = "202", msg = "分馆数量已达上限" });
                                    }
                                }
                                break;
                        }
                        //场馆
                        int FatherLibraryId = 0;
                        if (Type == 0)
                        {
                            FatherLibraryId = 0;
                        }
                        else
                        {
                            if (Library["FatherLibraryId"] == null)
                            {
                                return Json(new { code = "201", msg = "请选择上级场馆" });
                            }
                            FatherLibraryId = int.Parse(Library["FatherLibraryId"]);
                            if (int.Parse(Library["ID"]) == FatherLibraryId)
                            {
                                return Json(new { code = "202", msg = "不可以选择自身为上级场馆" });
                            }
                        }
                        string LibraryName = Library["LibraryName"];
                        string LibraryCode = PingYinHelper.GetFirstSpell(Library["LibraryName"]) + DateTime.Now.Millisecond;

                        SysLibrary sysLibrary = new SysLibrary()
                        {
                            ID = LbraryId,
                            LibraryName = Library["LibraryName"],
                            LibraryDescribe = HttpUtility.UrlDecode(Library["Describe"]).Replace("'", "‘"),
                            FatherLibraryId = FatherLibraryId,
                            Type = int.Parse(Library["Type"]),
                            EditTime = DateTime.Now
                        };

                        db.Entry(sysLibrary).State = EntityState.Modified;
                        //不更新的字段
                        db.Entry(sysLibrary).Property(x => x.City).IsModified = false;
                        db.Entry(sysLibrary).Property(x => x.LibraryCode).IsModified = false;
                        db.Entry(sysLibrary).Property(x => x.CrateTime).IsModified = false;
                        db.Entry(sysLibrary).Property(x => x.IsEnable).IsModified = false;
                        db.Entry(sysLibrary).Property(x => x.DelState).IsModified = false;
                        db.SaveChanges();

                        Lg.AddLog("修改场馆", "Library", 2, GetUserName());

                        return Json(new { code = "200", msg = "修改成功!" });
                    }
                }
                else
                {
                    return Json(new { code = "202", msg = "场馆已存在!" });
                }
            }
            catch (Exception ex)
            {
                if (Library.Count > 0)
                {
                    LogHelper.ErrorLog("场馆修改失败:" + ex.Message);
                }

                return Json(new { code = "201", msg = "修改失败，请重试或联系管理员!" });
            }
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id">场馆id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditEnable(int id, int enable)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    SysLibrary sysLibrary = new SysLibrary()
                    {
                        ID = id,
                        IsEnable = enable,
                        EditTime = DateTime.Now
                    };

                    db.Entry(sysLibrary).State = EntityState.Modified;
                    //不更新的字段
                    db.Entry(sysLibrary).Property(x => x.LibraryName).IsModified = false;
                    db.Entry(sysLibrary).Property(x => x.LibraryDescribe).IsModified = false;
                    db.Entry(sysLibrary).Property(x => x.Type).IsModified = false;
                    db.Entry(sysLibrary).Property(x => x.City).IsModified = false;
                    db.Entry(sysLibrary).Property(x => x.LibraryCode).IsModified = false;
                    db.Entry(sysLibrary).Property(x => x.CrateTime).IsModified = false;
                    db.Entry(sysLibrary).Property(x => x.DelState).IsModified = false;
                    db.SaveChanges();

                    Lg.AddLog("修改场馆状态", "Library", 2, GetUserName());
                    return Json(new { code = "200", msg = "修改成功!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("场馆状态修改失败:" + ex.Message);
                return Json(new { code = "201", msg = "修改失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 删除场馆
        /// </summary>
        /// <param name="id">场馆id</param>
        /// <param name="page">当前页码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelLibrary(int id, int page)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    SysLibrary sysLibrary = new SysLibrary()
                    {
                        ID = id,
                        DelState = 1,
                        EditTime = DateTime.Now
                    };

                    db.Entry(sysLibrary).State = EntityState.Modified;
                    //不更新的字段
                    db.Entry(sysLibrary).Property(x => x.LibraryName).IsModified = false;
                    db.Entry(sysLibrary).Property(x => x.LibraryDescribe).IsModified = false;
                    db.Entry(sysLibrary).Property(x => x.Type).IsModified = false;
                    db.Entry(sysLibrary).Property(x => x.City).IsModified = false;
                    db.Entry(sysLibrary).Property(x => x.LibraryCode).IsModified = false;
                    db.Entry(sysLibrary).Property(x => x.CrateTime).IsModified = false;
                    db.Entry(sysLibrary).Property(x => x.IsEnable).IsModified = false;
                    db.SaveChanges();
                    int npage = 0;
                    int Count = db.SysLibrarys.Where(u => u.DelState == 0).AsNoTracking().Count();
                    double MaxPage = Convert.ToDouble(Convert.ToDouble(Count + 10) / Convert.ToDouble(10));
                    if (MaxPage > page)
                    {
                        npage = page;
                    }
                    else
                    {
                        if (Count <= 10)
                        {
                            npage = 1;
                        }
                        else
                        {
                            npage = page - 1;
                        }
                    }

                    Lg.AddLog("删除场馆", "Library", 3, GetUserName());

                    return Json(new { code = "200", page = npage, msg = "删除成功!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("场馆删除失败:" + ex.Message);
                return Json(new { code = "201", msg = "删除失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 批量删除场馆
        /// </summary>
        /// <param name="idList">场馆id集合</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelLibraryAll(string idList, int page)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    //获取待删除场馆id集
                    string[] sArray = idList.Split(',');
                    int[] IdList = new int[sArray.Length];
                    for (int i = 0; i < sArray.Length; i++)
                    {
                        IdList[i] = Int32.Parse(sArray[i]);
                    }
                    for (int i = 0; i < IdList.Length; i++)
                    {
                        int id = IdList[i];

                        SysLibrary sysLibrary = new SysLibrary()
                        {
                            ID = id,
                            DelState = 1,
                            EditTime = DateTime.Now
                        };

                        db.Entry(sysLibrary).State = EntityState.Modified;
                        //不更新的字段
                        db.Entry(sysLibrary).Property(x => x.LibraryName).IsModified = false;
                        db.Entry(sysLibrary).Property(x => x.LibraryDescribe).IsModified = false;
                        db.Entry(sysLibrary).Property(x => x.Type).IsModified = false;
                        db.Entry(sysLibrary).Property(x => x.City).IsModified = false;
                        db.Entry(sysLibrary).Property(x => x.LibraryCode).IsModified = false;
                        db.Entry(sysLibrary).Property(x => x.CrateTime).IsModified = false;
                        db.Entry(sysLibrary).Property(x => x.IsEnable).IsModified = false;
                        db.SaveChanges();
                    }
                    int npage = 0;
                    int Count = db.SysLibrarys.Where(u => u.DelState == 0).AsNoTracking().Count();
                    double MaxPage = Convert.ToDouble(Convert.ToDouble(Count + 10) / Convert.ToDouble(10));
                    if (MaxPage > page)
                    {
                        npage = page;
                    }
                    else
                    {
                        if (Count <= 10)
                        {
                            npage = 1;
                        }
                        else
                        {
                            if ((Count % 10) <= page)
                            {
                                npage = page - 1;
                            }
                        }
                    }

                    Lg.AddLog("删除场馆", "User", 3, GetUserName());

                    return Json(new { code = "200", page = npage, msg = "删除成功!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("场馆删除失败:" + ex.Message);
                return Json(new { code = "201", msg = "删除失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 场馆用户管理页
        /// </summary>
        /// <returns></returns>
        public ActionResult LibraryUser(int? page, int Id)
        {
            try
            {
                DataWallContext db = new DataWallContext();
                //当前用户
                List<SysLibraryUser> sysList = db.SysLibraryUsers.Where(l => l.SysLibraryId == Id).AsNoTracking().ToList();
                List<SysUser> sysUser = new List<SysUser>();
                foreach (var item in sysList)
                {
                    SysUser newUser = db.SysUsers.AsNoTracking().FirstOrDefault(u => u.ID == item.SysUserID & u.DelState == 0);
                    if (newUser != null)
                    {
                        sysUser.Add(newUser);
                    }
                }
                ViewData["datalist"] = sysUser;

                //未添加用户
                List<SysUser> sysUsers = db.SysUsers.Where(u => u.DelState == 0).AsNoTracking().ToList();
                var queryUsers = from u in sysUsers
                                 where !(from s in sysUser select s.ID).Contains(u.ID)
                                 select u;
                ViewBag.DataCout = queryUsers.Count();
                ViewBag.LibraryId = Id;

                //第几页
                int pageNumber = page ?? 1;

                //每页显示多少条
                int pageSize = 10;

                //根据ID排序
                sysUsers = queryUsers.OrderBy(u => u.ID).ToList();

                //通过ToPagedList扩展方法进行分页
                IPagedList<SysUser> pagedList = queryUsers.ToPagedList(pageNumber, pageSize);

                //将分页处理后的列表传给View
                return View(pagedList);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("场馆用户列表获取失败:" + ex.Message);
                return Json(new { msg = "获取列表失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 删除场馆用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="LibraryId">馆Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelLibraryUser(int id, int LibraryId)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    SysLibraryUser sysLibraryUser = db.SysLibraryUsers.FirstOrDefault(u => u.SysUserID == id & u.SysLibraryId == LibraryId);
                    string RoleName = "";
                    foreach (var item in db.SysUsers.AsNoTracking().FirstOrDefault(u => u.ID == sysLibraryUser.SysUserID).SysUserRoles)
                    {
                        RoleName = item.SysRole.RoleName;
                    }
                    if (RoleName != "Administrator")
                    {
                        db.SysLibraryUsers.Remove(sysLibraryUser);
                        db.SaveChanges();

                        Lg.AddLog("删除场馆", "Library", 3, GetUserName());

                        return Json(new { code = "200", msg = "删除成功!" });
                    }
                    else
                    {
                        return Json(new { code = "201", msg = "管理员用户不可删除!" });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("场馆用户删除失败:" + ex.Message);
                return Json(new { code = "201", msg = "删除失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 添加场馆用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="LibraryId">馆Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddLibraryUser(int id, int LibraryId)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    if (db.SysLibraryUsers.Where(lu => lu.SysUserID == id && lu.SysLibraryId == LibraryId).AsNoTracking().Count() == 0)
                    {
                        SysLibraryUser sysLibraryUser = new SysLibraryUser()
                        {
                            SysUserID = id,
                            SysLibraryId = LibraryId
                        };

                        db.SysLibraryUsers.Add(sysLibraryUser);
                        db.SaveChanges();

                        Lg.AddLog("添加场馆", "Library", 1, GetUserName());

                        return Json(new { code = "200", msg = "添加成功!" });
                    }
                    else
                    {
                        return Json(new { code = "201", msg = "用户已存在于该场馆用户列表中!" });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("场馆用户添加失败:" + ex.Message);
                return Json(new { code = "201", msg = "添加失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 检查场馆是否存在
        /// </summary>
        /// <param name="Action">方法</param>
        /// <param name="Id">新增时为0</param>
        /// <param name="LibraryName">场馆名</param>
        /// <returns></returns>
        public bool Check(string Action, int Id, string LibraryName)
        {
            using (DataWallContext db = new DataWallContext())
            {
                if (Action == "LibraryEdit")
                {
                    if (db.SysLibrarys.Where(u => u.LibraryName == DbFunctions.AsNonUnicode(LibraryName)).AsNoTracking().Count() >= 1)
                    {
                        //AsNoTracking将Hold住的对象释放掉
                        SysLibrary Library = db.SysLibrarys.AsNoTracking().FirstOrDefault(u => u.ID == Id);
                        if (Library.LibraryName == LibraryName)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (db.SysLibrarys.Where(u => u.LibraryName == LibraryName).AsNoTracking().Count() > 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
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