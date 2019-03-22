using Common;
using DAL;
using DataWall.SignalR;
using Model;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DataWall.Controllers.WebApi
{
    public class CustCountController : ApiController
    {
        /// <summary>
        /// 接收摄像头传输的数据
        /// </summary>
        /// <param name="value"></param>
        [HttpGet]
        public void CustCountingServlet([FromBody]string value)
        {
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象
                string uuid = request["uuid"];
                string dataguid = request["dataguid"];
                string innum = request["innum"];
                string outnum = request["outnum"];
                string curtime = request["curtime"];
                string accessid = request["accessid"];

                using (DataWallContext db = new DataWallContext())
                {
                    SysCustDevice sysCustDevice = db.SysCustDevices.Where(s => s.Uuid == uuid & s.DataGuid == dataguid).FirstOrDefault();
                    if (sysCustDevice != null)
                    {
                        SysCustData sysCustData = new SysCustData()
                        {
                            SysCustDeviceId = sysCustDevice.ID,
                            D_Date = DateTime.Parse(curtime),
                            D_InNum = int.Parse(innum),
                            D_OutNum = int.Parse(outnum)
                        };
                        db.SysCustDatas.Add(sysCustData);
                        db.SaveChanges();

                        LogHelper.InfoLog("客流数据插入成功,入馆:" + innum + ";出馆:" + outnum);

                        //推送客流更新
                        string ToLibraryName = db.SysLibrarys.Find(sysCustDevice.SysLibraryId).LibraryName;
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
                        LogHelper.ErrorLog("客流设备不存在;uuid:" + request["uuid"] + ",dataguid:" + request["dataguid"]);
                    }
                    HttpContext.Current.Response.Charset = "GB2312";
                    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                    HttpContext.Current.Response.Write("JunYuFr_CustFlow_ReturnCode=0");
                    HttpContext.Current.Response.End();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("客流数据插入失败:" + ex.ToString());
                HttpContext.Current.Response.Charset = "GB2312";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                HttpContext.Current.Response.Write("JunYuFr_CustFlow_ReturnCode=0");
                HttpContext.Current.Response.End();
            }
        }

        /// <summary>
        /// 摄像头时间矫正
        /// </summary>
        /// <param name="value"></param>
        [HttpGet]
        public void TimeCorrectServlet([FromBody]string value)
        {
            try
            {
                LogHelper.InfoLog("时间矫正 " + DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                HttpContext.Current.Response.Charset = "GB2312";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                HttpContext.Current.Response.Write("JunYuFr_CustFlow_ReturnCode=" + DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex.ToString());
                HttpContext.Current.Response.Charset = "GB2312";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                HttpContext.Current.Response.Write("JunYuFr_CustFlow_ReturnCode=0");
                HttpContext.Current.Response.End();
            }
        }

        /// <summary>
        /// 心跳
        /// </summary>
        /// <param name="value"></param>
        [HttpGet]
        public void HeartBeatServlet([FromBody]string value)
        {
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象
                string uuid = request["uuid"];
                string ip = request["ip"];
                string cpu = request["cpu"];

                LogHelper.InfoLog("设备：" + uuid + " ip：" + ip + " CPU使用率：" + cpu);
                HttpContext.Current.Response.Charset = "GB2312";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                HttpContext.Current.Response.Write("JunYuFr_CustFlow_ReturnCode=0");
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex.ToString());
                LogHelper.ErrorLog(ex.ToString());
                HttpContext.Current.Response.Charset = "GB2312";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                HttpContext.Current.Response.Write("JunYuFr_CustFlow_ReturnCode=0");
                HttpContext.Current.Response.End();
            }
        }

        /// <summary>
        /// 远程连接
        /// </summary>
        /// <param name="context"></param>
        [HttpGet]
        public void RemoteConnectServlet([FromBody]string value)
        {
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象
                string uuid = request["uuid"];
                string isconnect = request["isconnect"];

                string strcz = "";
                if (isconnect.Contains("true"))
                {
                    strcz = "请求连接";
                    LogHelper.InfoLog("设备：" + uuid + strcz);
                    HttpContext.Current.Response.Charset = "GB2312";
                    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                    HttpContext.Current.Response.Write("JunYuFr_CustFlow_ReturnCode=1");
                    HttpContext.Current.Response.End();
                }
                else
                {
                    strcz = "请求断开";
                    LogHelper.InfoLog("设备：" + uuid + strcz);
                    HttpContext.Current.Response.Charset = "GB2312";
                    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                    HttpContext.Current.Response.Write("JunYuFr_CustFlow_ReturnCode=0");
                    HttpContext.Current.Response.End();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex.ToString());
                LogHelper.ErrorLog(ex.ToString());
                HttpContext.Current.Response.Charset = "GB2312";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                HttpContext.Current.Response.Write("JunYuFr_CustFlow_ReturnCode=2");
                HttpContext.Current.Response.End();
            }
        }
    }
}