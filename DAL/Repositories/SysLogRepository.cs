using Common;
using Model;
using Model.ToolModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAL.Repositories
{
    public class SysLogRepository : IsysLogRepository
    {
        protected DataWallContext db = new DataWallContext();

        //添加日志
        public void AddLog(string Details, string Modular, int Type, string UserName)
        {
            string ip = IpHelper.GetWebClientIp();
            Action action = new Action(() =>
            {
                try
                {
                    using (DataWallContext db = new DataWallContext())
                    {
                        string city = IpHelper.GetCity(ip);
                        SysLog sysLog = new SysLog();
                        sysLog.Type = Type;
                        sysLog.Modular = Modular;
                        sysLog.Details = Details;
                        sysLog.UserName = UserName;
                        sysLog.CrateTime = DateTime.Now;
                        sysLog.Ip = ip;
                        sysLog.Address = city;
                        db.SysLogs.Add(sysLog);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog(Details + "操作日志添加失败!" + ex.Message);
                }
            });
            action.BeginInvoke(null, null);
        }
    }
}