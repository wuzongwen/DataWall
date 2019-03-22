using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using DAL;
using DAL.Repositories;
using PagedList;
using Model.ToolModels;
using Common;
using Newtonsoft.Json;
using DataWall.ViewModels;
using System.Data.Entity;
using System.IO;
using OfficeOpenXml;
using DataWall.SignalR;

namespace DataWall.Controllers.Admin
{
    [CustomAuthorize]//权限验证
    public class OtherController : Controller
    {
        IsysLogRepository Lg = new SysLogRepository();

        #region 额外数据
        /// <summary>
        /// 数据列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult AdditionalDataList(SearchInfo searchInfo, int? page, int Type)
        {
            using (DataWallContext db = new DataWallContext())
            {
                //当前用户可管理场馆
                List<SysLibrary> libList = GetLibraryList();
                ViewData["datalist"] = libList;

                int[] libids = new int[libList.Count];
                for (int i = 0; i < libList.Count; i++)
                {
                    libids[i] = libList[i].ID;
                }

                string Title = "";
                switch (Type)
                {
                    case 0:
                        Title = "客流数据";
                        break;
                    case 1:
                        Title = "借还数据";
                        break;
                }
                ViewBag.Title = Title;
                ViewBag.Type = Type;

                //全部内容
                var list = db.SysAdditionalDatas.Where(u => u.DelState == 0 & u.Type == Type & libids.Contains(u.SysLibraryId)).AsNoTracking().ToList();

                //第几页
                int pageNumber = page ?? 1;

                //条件查询
                if (searchInfo != null)
                {
                    int libraryIds = searchInfo.library;
                    string keywords = searchInfo.keyword;
                    if (keywords == null)
                    {
                        keywords = "";
                    }
                    if (libraryIds != 0)
                    {
                        list = db.SysAdditionalDatas.Where(u => u.DelState == 0 & u.Type == Type & u.SysLibraryId == libraryIds).AsNoTracking().ToList();
                    }
                    else
                    {
                        list = db.SysAdditionalDatas.Where(u => u.DelState == 0 & u.Type == Type & libids.Contains(u.SysLibraryId)).AsNoTracking().ToList();
                    }
                }

                //每页显示的数据数量
                int pageSize = 10;

                ViewBag.DataCout = list.Count();
                ViewBag.library = searchInfo.library;

                //通过ToPagedList扩展方法进行分页
                IPagedList<SysAdditionalData> pagedList = list.OrderByDescending(c => c.DataDatetime).ToPagedList(pageNumber, pageSize);

                return View(pagedList);
            }
        }

        /// <summary>
        /// 额外数据修改页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AdditionalDataEdit(int id)
        {
            using (DataWallContext db = new DataWallContext())
            {
                //当前用户可管理场馆
                List<SysLibrary> libList = GetLibraryList();
                ViewData["datalist"] = libList;
                SysAdditionalData sysAdditionalData = db.SysAdditionalDatas.Find(id);
                return View(sysAdditionalData);
            }
        }

        /// <summary>
        /// 修改额外数据
        /// </summary>
        /// <param name="AdditionalData">额外数据</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AdditionalDataEdit(FormCollection AdditionalData)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    //当前用户可管理场馆
                    List<SysLibrary> libList = GetLibraryList();
                    if (libList.Count == 0)
                    {
                        return Json(new { code = "202", msg = "当前用户未分配可管理场馆，请联系系统管理员！" });
                    }
                    ViewData["datalist"] = libList;

                    //额外数据
                    if (AdditionalData["Library"] != null)
                    {
                        int BookNum = 0;
                        int StillPeopleNum = 0;
                        int StillBookNum = 0;

                        int LibraryId = int.Parse(AdditionalData["Library"]);
                        string ToLibraryName = db.SysLibrarys.Find(LibraryId).LibraryName;
                        if (AdditionalData["Type"] == "0")
                        {
                            BookNum = 0;
                            StillPeopleNum = 0;
                            StillPeopleNum = 0;

                            int type = 3;
                            var msg = "客流数据更新";
                            MyHub.Show(ToLibraryName, JsonConvert.SerializeObject(new
                            {
                                msg = msg,
                                action = "Other",
                                type = type
                            }));
                        }
                        else
                        {
                            BookNum = int.Parse(AdditionalData["BookNum"]);
                            StillPeopleNum = int.Parse(AdditionalData["StillPeopleNum"]);
                            StillBookNum = int.Parse(AdditionalData["StillBookNum"]);

                            int type = 4;
                            var msg = "借还数据更新";
                            MyHub.Show(ToLibraryName, JsonConvert.SerializeObject(new
                            {
                                msg = msg,
                                action = "Other",
                                type = type
                            }));
                        }

                        SysAdditionalData sysAdditionalData = new SysAdditionalData()
                        {
                            ID = int.Parse(AdditionalData["ID"]),
                            PeopleNum = int.Parse(AdditionalData["PeopleNum"]),
                            BookNum = BookNum,
                            StillPeopleNum = StillPeopleNum,
                            StillBookNum = StillBookNum,
                            DataDatetime = DateTime.Parse(AdditionalData["DataDatetime"]),
                            SysLibraryId = int.Parse(AdditionalData["Library"]),
                            EditTime = DateTime.Now
                        };

                        db.Entry(sysAdditionalData).State = EntityState.Modified;
                        //不更新的字段
                        db.Entry(sysAdditionalData).Property(x => x.Type).IsModified = false;
                        db.Entry(sysAdditionalData).Property(x => x.CrateTime).IsModified = false;
                        db.Entry(sysAdditionalData).Property(x => x.IsEnable).IsModified = false;
                        db.Entry(sysAdditionalData).Property(x => x.DelState).IsModified = false;

                        db.SaveChanges();

                        Lg.AddLog("修改额外数据", "Other", 2, GetUserName());

                        return Json(new { code = "200", msg = "修改成功!" });
                    }
                    else
                    {
                        return Json(new { code = "202", msg = "请选择内容所属场馆!" });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("额外数据修改失败:" + ex.Message);
                return Json(new { code = "201", msg = "修改失败，请重试或联系管理员!" });
            }
        }

        /// <summary>
        /// 下载示例文档
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public FileStreamResult ExcelDownload(int Type)
        {
            try
            {
                //当前用户可管理场馆
                List<SysLibrary> libList = GetLibraryList();

                string local = "Files\\DownloadFile\\Excel";
                string localPath = Path.Combine(HttpRuntime.AppDomainAppPath, local);
                string filePathName = "";
                string Worksheets = "";
                switch (Type)
                {
                    case 0:
                        filePathName = "客流数据示例文档_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                        Worksheets = "客流数据";
                        break;
                    case 1:
                        filePathName = "借还数据示例文档_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                        Worksheets = "借还数据";
                        break;
                }

                var tmpName = Server.MapPath("~/Files/DownloadFile/Excel/");
                string localURL = Path.Combine(local, filePathName);

                FileInfo newFile = new FileInfo(tmpName + "\\" + filePathName);
                if (newFile.Exists)
                {
                    newFile.Delete();
                    newFile = new FileInfo(tmpName + "\\" + filePathName);
                }
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    if (!System.IO.Directory.Exists(tmpName))
                        System.IO.Directory.CreateDirectory(tmpName);
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(Worksheets);
                    worksheet.Cells[1, 1].Value = "场馆名称";
                    worksheet.Cells[1, 2].Value = "场馆ID";
                    switch (Type)
                    {
                        case 0:
                            worksheet.Cells[1, 3].Value = "到馆人数";
                            worksheet.Cells[1, 4].Value = "时间";

                            foreach (var item in libList)
                            {
                                int index = libList.IndexOf(item);
                                worksheet.Cells[2 + index, 1].Value = item.LibraryName;
                                worksheet.Cells[2 + index, 2].Value = item.ID;
                                worksheet.Cells[2 + index, 3].Value = 1;
                                worksheet.Cells[2 + index, 4].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            };

                            break;
                        case 1:
                            worksheet.Cells[1, 3].Value = "借阅人次";
                            worksheet.Cells[1, 4].Value = "借阅册次";
                            worksheet.Cells[1, 5].Value = "归还人次";
                            worksheet.Cells[1, 6].Value = "归还册次";
                            worksheet.Cells[1, 7].Value = "时间";

                            foreach (var item in libList)
                            {
                                int index = libList.IndexOf(item);
                                worksheet.Cells[2 + index, 1].Value = item.LibraryName;
                                worksheet.Cells[2 + index, 2].Value = item.ID;
                                worksheet.Cells[2 + index, 3].Value = 1;
                                worksheet.Cells[2 + index, 4].Value = 1;
                                worksheet.Cells[2 + index, 5].Value = 1;
                                worksheet.Cells[2 + index, 6].Value = 1;
                                worksheet.Cells[2 + index, 7].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            
                            break;
                    }
                    package.Save();
                }

                string filePath = Server.MapPath("~/" + localURL);//路径
                return File(new FileStream(filePath, FileMode.Open), "text/plain", filePathName);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("额外数据示例文档下载失败:" + ex.Message);
                throw;
            }
            
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadExcel(int Type)
        {
            int location = 0;
            try
            {
                var file = Request.Files[0];
                var filecombin = file.FileName.Split('.');
                if (file == null || String.IsNullOrEmpty(file.FileName) || file.ContentLength == 0 || filecombin.Length < 2)
                {
                    return Json(new
                    {
                        code = "201",
                        msg = "上传出错请检查文件名或文件内容"
                    });
                }

                using (ExcelPackage package = new ExcelPackage(file.InputStream))
                {
                    int count = package.Workbook.Worksheets.Count;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    //获取表格的列数和行数
                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;
                    var sysAdditionalDatas = new List<SysAdditionalData>();
                    //插入数据
                    using (DataWallContext db = new DataWallContext())
                    {
                        for (int row = 2; row <= rowCount; row++)
                        {
                            location = row;
                            int PeopleNum = 0;
                            int BookNum = 0;
                            int StillPeopleNum = 0;
                            int StillBookNum = 0;
                            DateTime DataDatetime = DateTime.Now;

                            switch (Type)
                            {
                                case 0:
                                    PeopleNum = int.Parse(worksheet.Cells[row, 3].Value.ToString());
                                    DataDatetime = DateTime.Parse(worksheet.Cells[row, 4].Value.ToString());
                                    break;
                                case 1:
                                    PeopleNum = int.Parse(worksheet.Cells[row, 3].Value.ToString());
                                    BookNum = int.Parse(worksheet.Cells[row, 4].Value.ToString());
                                    StillPeopleNum = int.Parse(worksheet.Cells[row, 5].Value.ToString());
                                    StillBookNum = int.Parse(worksheet.Cells[row, 6].Value.ToString());
                                    DataDatetime = DateTime.Parse(worksheet.Cells[row, 7].Value.ToString());
                                    break;
                            }

                            SysAdditionalData sysAdditionalData = new SysAdditionalData()
                            {
                                PeopleNum = PeopleNum,
                                BookNum = BookNum,
                                StillPeopleNum = StillPeopleNum,
                                StillBookNum = StillBookNum,
                                DataDatetime = DataDatetime,
                                SysLibraryId = int.Parse(worksheet.Cells[row, 2].Value.ToString()),
                                CrateTime = DateTime.Now,
                                EditTime = DateTime.Now,
                                DelState = 0,
                                Type = Type
                            };

                            sysAdditionalDatas.Add(sysAdditionalData);

                            //数据检查
                            if (!CheckLibraryId(worksheet.Cells[row, 2].Value.ToString(), worksheet.Cells[row, 1].Value.ToString()))
                            {
                                return Json(new
                                {
                                    code = "201",
                                    msg = "场馆不存在或您没有场馆" + worksheet.Cells[row, 1].Value.ToString() + "(ID:" + worksheet.Cells[row, 2].Value.ToString() + ")" + "的数据导入权限"
                                });
                            }

                            //推送客流更新
                            string ToLibraryName = db.SysLibrarys.Find(sysAdditionalData.SysLibraryId).LibraryName;
                            int type = 3;
                            var msg = "客流数据更新";
                            MyHub.Show(ToLibraryName, JsonConvert.SerializeObject(new
                            {
                                msg = msg,
                                action = "CustData",
                                type = type
                            }));
                        }

                        sysAdditionalDatas.ForEach(s => db.SysAdditionalDatas.Add(s));
                        db.SaveChanges();
                    }

                    Lg.AddLog("导入额外数据", "Other", 1, GetUserName());

                    return Json(new
                    {
                        code = "200",
                        msg = "导入成功"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("数据导入失败:" + ex.Message);
                return Json(new
                {
                    code = "201",
                    msg = "导入失败,请检查内容格式,错误行号:" + location
                });
            }
        }

        /// <summary>
        /// 数据检查
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool CheckLibraryId(string id, string name)
        {
            using (DataWallContext db = new DataWallContext())
            {
                var Cookies = SecurityHelper.DecryptDES(CookieHelper.GetCookieValue("User"), db.SysProgramInfos.AsNoTracking().FirstOrDefault().CookieSecretKey);
                UserCookie user = new UserCookie();
                if (Cookies != "")
                {
                    user = JsonConvert.DeserializeObject<UserCookie>(Cookies);
                }
                var LibraryIdList = db.SysLibraryUsers.Where(slu => slu.SysUserID == user.UserId).Select(slu => new { slu.SysLibraryId }).AsNoTracking().ToList();
                List<String> idlist = new List<string>();
                foreach (var item in LibraryIdList)
                {
                    idlist.Add(item.SysLibraryId.ToString());
                }
                if (!idlist.Contains(id))
                {
                    return false;
                }
                List<String> namelist = new List<string>();
                foreach (var item in LibraryIdList)
                {
                    var LibraryList = db.SysLibrarys.Where(sl => sl.ID == item.SysLibraryId).Select(sl => new { sl.LibraryName }).AsNoTracking().ToList();
                    foreach (var items in LibraryList)
                    {
                        namelist.Add(items.LibraryName.ToString());
                    }
                }
                if (!namelist.Contains(name))
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 读取sheet 内的数据进入实体
        /// </summary>
        /// <param name="worksheet"></param>
        /// <returns></returns>
        public bool GetSheetValues(string filepath)
        {
            FileInfo file = new FileInfo(filepath);
            if (file != null)
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    //获取表格的列数和行数
                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;
                    var sysAdditionalDatas = new List<SysAdditionalData>();
                    for (int row = 1; row <= rowCount; row++)
                    {
                        SysAdditionalData sysAdditionalData = new SysAdditionalData();
                        sysAdditionalData.SysLibraryId = int.Parse(worksheet.Cells[row, 1].Value.ToString());
                        sysAdditionalDatas.Add(sysAdditionalData);
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 删除内容
        /// </summary>
        /// <param name="id">内容id</param>
        /// <param name="page">当前页码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelAdditionalData(int id, int page)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    SysAdditionalData sysAdditionalData = new SysAdditionalData()
                    {
                        ID = id,
                        DelState = 1,
                        EditTime = DateTime.Now
                    };

                    db.Entry(sysAdditionalData).State = EntityState.Modified;
                    //不更新的字段
                    db.Entry(sysAdditionalData).Property(x => x.Type).IsModified = false;
                    db.Entry(sysAdditionalData).Property(x => x.PeopleNum).IsModified = false;
                    db.Entry(sysAdditionalData).Property(x => x.BookNum).IsModified = false;
                    db.Entry(sysAdditionalData).Property(x => x.StillPeopleNum).IsModified = false;
                    db.Entry(sysAdditionalData).Property(x => x.StillBookNum).IsModified = false;
                    db.Entry(sysAdditionalData).Property(x => x.DataDatetime).IsModified = false;
                    db.Entry(sysAdditionalData).Property(x => x.SysLibraryId).IsModified = false;
                    db.Entry(sysAdditionalData).Property(x => x.CrateTime).IsModified = false;
                    db.Entry(sysAdditionalData).Property(x => x.IsEnable).IsModified = false;
                    db.SaveChanges();
                    int npage = 0;
                    int Count = db.SysAdditionalDatas.Where(u => u.DelState == 0).Count();
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

                    Lg.AddLog("删除额外数据", "Other", 3, GetUserName());

                    return Json(new { code = "200", page = npage, msg = "删除成功!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("额外数据删除失败:" + ex.Message);
                return Json(new { code = "201", msg = "删除失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id">数据id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AdditionalDataEditEnable(int id, int enable)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    SysAdditionalData sysAdditionalData = new SysAdditionalData()
                    {
                        ID = id,
                        IsEnable = enable,
                        EditTime = DateTime.Now
                    };

                    db.Entry(sysAdditionalData).State = EntityState.Modified;
                    //不更新的字段
                    db.Entry(sysAdditionalData).Property(x => x.Type).IsModified = false;
                    db.Entry(sysAdditionalData).Property(x => x.PeopleNum).IsModified = false;
                    db.Entry(sysAdditionalData).Property(x => x.BookNum).IsModified = false;
                    db.Entry(sysAdditionalData).Property(x => x.StillPeopleNum).IsModified = false;
                    db.Entry(sysAdditionalData).Property(x => x.StillBookNum).IsModified = false;
                    db.Entry(sysAdditionalData).Property(x => x.DataDatetime).IsModified = false;
                    db.Entry(sysAdditionalData).Property(x => x.SysLibraryId).IsModified = false;
                    db.Entry(sysAdditionalData).Property(x => x.CrateTime).IsModified = false;
                    db.Entry(sysAdditionalData).Property(x => x.DelState).IsModified = false;
                    db.SaveChanges();

                    Lg.AddLog("修改额外数据状态", "Other", 2, GetUserName());
                    return Json(new { code = "200", msg = "修改成功!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("额外数据状态修改失败:" + ex.Message);
                return Json(new { code = "201", msg = "修改失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 批量删除内容
        /// </summary>
        /// <param name="idList">内容id集合</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelAdditionalDataAll(string idList, int page)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    //获取待删除id集合
                    string[] sArray = idList.Split(',');
                    int[] IdList = new int[sArray.Length];
                    for (int i = 0; i < sArray.Length; i++)
                    {
                        IdList[i] = Int32.Parse(sArray[i]);
                    }
                    for (int i = 0; i < IdList.Length; i++)
                    {
                        int id = IdList[i];
                        SysAdditionalData sysAdditionalData = new SysAdditionalData()
                        {
                            ID = id,
                            DelState = 1,
                            EditTime = DateTime.Now
                        };

                        db.Entry(sysAdditionalData).State = EntityState.Modified;
                        //不更新的字段
                        db.Entry(sysAdditionalData).Property(x => x.Type).IsModified = false;
                        db.Entry(sysAdditionalData).Property(x => x.PeopleNum).IsModified = false;
                        db.Entry(sysAdditionalData).Property(x => x.BookNum).IsModified = false;
                        db.Entry(sysAdditionalData).Property(x => x.StillPeopleNum).IsModified = false;
                        db.Entry(sysAdditionalData).Property(x => x.StillBookNum).IsModified = false;
                        db.Entry(sysAdditionalData).Property(x => x.DataDatetime).IsModified = false;
                        db.Entry(sysAdditionalData).Property(x => x.SysLibraryId).IsModified = false;
                        db.Entry(sysAdditionalData).Property(x => x.CrateTime).IsModified = false;
                        db.Entry(sysAdditionalData).Property(x => x.IsEnable).IsModified = false;
                        db.SaveChanges();
                    }
                    int npage = 0;
                    int Count = db.SysAdditionalDatas.Where(u => u.DelState == 0).AsNoTracking().Count();
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

                    Lg.AddLog("删除额外数据", "Other", 3, GetUserName());

                    return Json(new { code = "200", page = npage, msg = "删除成功!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("额外数据删除失败:" + ex.Message);
                return Json(new { code = "201", msg = "删除失败，请重试或联/系管理员!" });
            }
        }
        #endregion

        #region 客流设备管理
        /// <summary>
        /// 客流设备列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult CustDeviceList(SearchInfo searchInfo, int? page)
        {
            using (DataWallContext db = new DataWallContext())
            {
                //当前用户可管理场馆
                List<SysLibrary> libList = GetLibraryList();
                ViewData["datalist"] = libList;

                int[] libids = new int[libList.Count];
                for (int i = 0; i < libList.Count; i++)
                {
                    libids[i] = libList[i].ID;
                }


                //全部内容
                var list = db.SysCustDevices.Where(u => u.DelState == 0 & libids.Contains(u.SysLibraryId)).AsNoTracking().ToList();

                //第几页
                int pageNumber = page ?? 1;

                //条件查询
                if (searchInfo != null)
                {
                    int libraryIds = searchInfo.library;
                    string keywords = searchInfo.keyword;
                    if (keywords == null)
                    {
                        keywords = "";
                    }
                    if (libraryIds != 0)
                    {
                        list = db.SysCustDevices.Where(u => u.DelState == 0 & u.SysLibraryId == libraryIds & u.CustDeviceName.Contains(keywords)).AsNoTracking().ToList();
                    }
                    else
                    {
                        list = db.SysCustDevices.Where(u => u.DelState == 0 & libids.Contains(u.SysLibraryId) & u.CustDeviceName.Contains(keywords)).AsNoTracking().ToList();
                    }
                }

                //每页显示的数据数量
                int pageSize = 10;

                ViewBag.DataCout = list.Count();
                ViewBag.library = searchInfo.library;
                ViewBag.keyword = searchInfo.keyword;

                //通过ToPagedList扩展方法进行分页
                IPagedList<SysCustDevice> pagedList = list.OrderByDescending(c => c.CrateTime).ToPagedList(pageNumber, pageSize);

                return View(pagedList);
            }
        }

        /// <summary>
        /// 客流设备添加页
        /// </summary>
        /// <returns></returns>
        public ActionResult CustDeviceAdd()
        {
            using (DataWallContext db = new DataWallContext())
            {
                //当前用户可管理场馆
                List<SysLibrary> libList = GetLibraryList();
                ViewData["datalist"] = libList;
                return View();
            }
        }

        /// <summary>
        /// 添加客流设备
        /// </summary>
        /// <param name="CustDevice">额外数据</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CustDeviceAdd(FormCollection CustDevice)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    //当前用户可管理场馆
                    List<SysLibrary> libList = GetLibraryList();
                    if (libList.Count == 0)
                    {
                        return Json(new { code = "202", msg = "当前用户未分配可管理场馆，请联系系统管理员!" });
                    }
                    ViewData["datalist"] = libList;

                    //客流设备
                    if (CustDevice["Library"] != null)
                    {
                        string CustDeviceName = CustDevice["CustDeviceName"];
                        if (db.SysCustDevices.Where(d => d.CustDeviceName == DbFunctions.AsNonUnicode(CustDeviceName)).AsNoTracking().Count() >=1)
                        {
                            return Json(new { code = "202", msg = "设备名已存在!" });
                        }
                        int LibraryId = int.Parse(CustDevice["Library"]);
                        string LibraryName = db.SysLibrarys.Find(LibraryId).LibraryName;
                        string Uuid = PingYinHelper.GetFirstSpell(LibraryName + CustDevice["CustDeviceName"]) + DateTime.Now.Millisecond;
                        string DataGuid = System.Guid.NewGuid().ToString("D");

                        SysCustDevice sysCustDevice = new SysCustDevice()
                        {
                            CustDeviceName = CustDevice["CustDeviceName"],
                            Uuid = Uuid,
                            DataGuid = DataGuid,
                            SysLibraryId = LibraryId,
                            CrateTime=DateTime.Now,
                            EditTime = DateTime.Now,
                            IsEnable=0,
                            DelState=0
                        };

                        db.SysCustDevices.Add(sysCustDevice);

                        //添加客流设备
                        db.SaveChanges();

                        Lg.AddLog("添加客流设备", "Other", 1, GetUserName());

                        return Json(new { code = "200", msg = "添加成功!" });
                    }
                    else
                    {
                        return Json(new { code = "202", msg = "请选择内容所属场馆!" });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("客流设备添加失败:" + ex.Message);
                return Json(new { code = "201", msg = "修改失败，请重试或联系管理员!" });
            }
        }

        /// <summary>
        /// 客流设备修改页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CustDeviceEdit(int id)
        {
            using (DataWallContext db = new DataWallContext())
            {
                //当前用户可管理场馆
                List<SysLibrary> libList = GetLibraryList();
                ViewData["datalist"] = libList;
                SysCustDevice sysCustDevice = db.SysCustDevices.Find(id);
                return View(sysCustDevice);
            }
        }

        /// <summary>
        /// 修改客流设备
        /// </summary>
        /// <param name="CustDevice">额外数据</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CustDeviceEdit(FormCollection CustDevice)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    //当前用户可管理场馆
                    List<SysLibrary> libList = GetLibraryList();
                    if (libList.Count == 0)
                    {
                        return Json(new { code = "202", msg = "当前用户未分配可管理场馆，请联系系统管理员！" });
                    }
                    ViewData["datalist"] = libList;

                    //客流设备
                    if (CustDevice["Library"] != null)
                    {
                        SysCustDevice sysCustDevice = new SysCustDevice()
                        {
                            ID = int.Parse(CustDevice["ID"]),
                            CustDeviceName = CustDevice["CustDeviceName"],
                            SysLibraryId = int.Parse(CustDevice["Library"]),
                            EditTime = DateTime.Now
                        };

                        db.Entry(sysCustDevice).State = EntityState.Modified;
                        //不更新的字段
                        db.Entry(sysCustDevice).Property(x => x.Uuid).IsModified = false;
                        db.Entry(sysCustDevice).Property(x => x.DataGuid).IsModified = false;
                        db.Entry(sysCustDevice).Property(x => x.CrateTime).IsModified = false;
                        db.Entry(sysCustDevice).Property(x => x.IsEnable).IsModified = false;
                        db.Entry(sysCustDevice).Property(x => x.DelState).IsModified = false;

                        db.SaveChanges();

                        Lg.AddLog("修改客流设备", "Other", 2, GetUserName());

                        return Json(new { code = "200", msg = "修改成功!" });
                    }
                    else
                    {
                        return Json(new { code = "202", msg = "请选择内容所属场馆!" });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("客流设备修改失败:" + ex.Message);
                return Json(new { code = "201", msg = "修改失败，请重试或联系管理员!" });
            }
        }

        /// <summary>
        /// 删除客流设备
        /// </summary>
        /// <param name="id">内容id</param>
        /// <param name="page">当前页码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelCustDevice(int id, int page)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    SysCustDevice sysCustDevice = new SysCustDevice()
                    {
                        ID = id,
                        DelState = 1,
                        EditTime = DateTime.Now
                    };

                    db.Entry(sysCustDevice).State = EntityState.Modified;
                    //不更新的字段
                    db.Entry(sysCustDevice).Property(x => x.CustDeviceName).IsModified = false;
                    db.Entry(sysCustDevice).Property(x => x.Uuid).IsModified = false;
                    db.Entry(sysCustDevice).Property(x => x.DataGuid).IsModified = false;
                    db.Entry(sysCustDevice).Property(x => x.SysLibraryId).IsModified = false;
                    db.Entry(sysCustDevice).Property(x => x.CrateTime).IsModified = false;
                    db.Entry(sysCustDevice).Property(x => x.IsEnable).IsModified = false;
                    db.SaveChanges();
                    int npage = 0;
                    int Count = db.SysAdditionalDatas.Where(u => u.DelState == 0).Count();
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

                    Lg.AddLog("删除客流设备", "Other", 3, GetUserName());

                    return Json(new { code = "200", page = npage, msg = "删除成功!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("客流设备删除失败:" + ex.Message);
                return Json(new { code = "201", msg = "删除失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id">数据id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CustDeviceEditEnable(int id, int enable)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    SysCustDevice sysCustDevice = new SysCustDevice()
                    {
                        ID = id,
                        IsEnable = enable,
                        EditTime = DateTime.Now
                    };

                    db.Entry(sysCustDevice).State = EntityState.Modified;
                    //不更新的字段
                    db.Entry(sysCustDevice).Property(x => x.CustDeviceName).IsModified = false;
                    db.Entry(sysCustDevice).Property(x => x.Uuid).IsModified = false;
                    db.Entry(sysCustDevice).Property(x => x.DataGuid).IsModified = false;
                    db.Entry(sysCustDevice).Property(x => x.SysLibraryId).IsModified = false;
                    db.Entry(sysCustDevice).Property(x => x.CrateTime).IsModified = false;
                    db.Entry(sysCustDevice).Property(x => x.DelState).IsModified = false;
                    db.SaveChanges();

                    Lg.AddLog("修改客流设备状态", "Other", 2, GetUserName());
                    return Json(new { code = "200", msg = "修改成功!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("客流设备状态修改失败:" + ex.Message);
                return Json(new { code = "201", msg = "修改失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 批量删除客流设备
        /// </summary>
        /// <param name="idList">内容id集合</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelCustDeviceAll(string idList, int page)
        {
            try
            {
                using (DataWallContext db = new DataWallContext())
                {
                    //获取待删除id集合
                    string[] sArray = idList.Split(',');
                    int[] IdList = new int[sArray.Length];
                    for (int i = 0; i < sArray.Length; i++)
                    {
                        IdList[i] = Int32.Parse(sArray[i]);
                    }
                    for (int i = 0; i < IdList.Length; i++)
                    {
                        int id = IdList[i];
                        SysCustDevice sysCustDevice = new SysCustDevice()
                        {
                            ID = id,
                            DelState = 1,
                            EditTime = DateTime.Now
                        };

                        db.Entry(sysCustDevice).State = EntityState.Modified;
                        //不更新的字段
                        db.Entry(sysCustDevice).Property(x => x.CustDeviceName).IsModified = false;
                        db.Entry(sysCustDevice).Property(x => x.Uuid).IsModified = false;
                        db.Entry(sysCustDevice).Property(x => x.DataGuid).IsModified = false;
                        db.Entry(sysCustDevice).Property(x => x.SysLibraryId).IsModified = false;
                        db.Entry(sysCustDevice).Property(x => x.CrateTime).IsModified = false;
                        db.Entry(sysCustDevice).Property(x => x.IsEnable).IsModified = false;
                        db.SaveChanges();
                    }
                    int npage = 0;
                    int Count = db.SysAdditionalDatas.Where(u => u.DelState == 0).AsNoTracking().Count();
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

                    Lg.AddLog("删除客流设备", "Other", 3, GetUserName());

                    return Json(new { code = "200", page = npage, msg = "删除成功!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("客流设备删除失败:" + ex.Message);
                return Json(new { code = "201", msg = "删除失败，请重试或联/系管理员!" });
            }
        }
        #endregion

        #region 操作日志
        /// <summary>
        /// 数据列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult LogList(SearchInfo searchInfo, int? page)
        {
            using (DataWallContext db = new DataWallContext())
            {
                //当前用户可管理场馆
                List<SysLibrary> libList = GetLibraryList();
                ViewData["datalist"] = libList;

                int[] libids = new int[libList.Count];
                for (int i = 0; i < libList.Count; i++)
                {
                    libids[i] = libList[i].ID;
                }


                //全部内容
                var list = db.SysLogs.AsNoTracking().ToList();

                //第几页
                int pageNumber = page ?? 1;

                //条件查询
                if (searchInfo != null)
                {
                    int libraryIds = searchInfo.library;
                    string keywords = searchInfo.keyword;
                    if (keywords == null)
                    {
                        keywords = "";
                    }
                    if (libraryIds != 0)
                    {
                        list = db.SysLogs.Where(u => u.Details.Contains(keywords)).AsNoTracking().ToList();
                    }
                    else
                    {
                        list = db.SysLogs.Where(u => u.Details.Contains(keywords)).AsNoTracking().ToList();
                    }
                }

                //每页显示的数据数量
                int pageSize = 10;

                ViewBag.DataCout = list.Count();
                ViewBag.library = searchInfo.library;
                ViewBag.keyword = searchInfo.keyword;

                //通过ToPagedList扩展方法进行分页
                IPagedList<SysLog> pagedList = list.OrderByDescending(c => c.CrateTime).ToPagedList(pageNumber, pageSize);

                return View(pagedList);
            }
        }
        #endregion

        /// <summary>
        /// 获取当前用户可管理场馆
        /// </summary>
        /// <returns></returns>
        public List<SysLibrary> GetLibraryList()
        {
            using (DataWallContext db = new DataWallContext())
            {
                var Cookies = SecurityHelper.DecryptDES(CookieHelper.GetCookieValue("User"), db.SysProgramInfos.AsNoTracking().FirstOrDefault().CookieSecretKey);
                UserCookie user = JsonConvert.DeserializeObject<UserCookie>(Cookies);
                var Libids = db.SysLibraryUsers.Where(u => u.SysUserID == user.UserId).Select(u => u.SysLibraryId).ToList();
                return db.SysLibrarys.Where(lib => Libids.Contains(lib.ID) & lib.DelState == 0 & lib.IsEnable == 0).AsNoTracking().ToList();
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