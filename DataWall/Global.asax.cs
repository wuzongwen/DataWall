using DataWall.App_Start;
using DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Web.Http;
using Common;

namespace DataWall
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            RegisterView_Custom_routing();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //开启js&css压缩
            BundleTable.EnableOptimizations = true;
        }

        //自定义路由
        protected void RegisterView_Custom_routing()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new admin_routing());
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception lastError = Server.GetLastError();
            if (lastError != null)
            {
                //异常信息
                string strExceptionMessage = string.Empty;

                //对HTTP 404做额外处理，其他错误全部当成500服务器错误
                HttpException httpError = lastError as HttpException;
                if (httpError != null)
                {
                    //获取错误代码
                    int httpCode = httpError.GetHttpCode();
                    strExceptionMessage = httpError.Message;
                    if (httpCode == 400 || httpCode == 404)
                    {
                        //Response.StatusCode = 404;
                        //跳转到指定的静态404信息页面，根据需求自己更改URL
                        Server.ClearError();
                        Response.WriteFile("~/HttpError/Error404.html");
                    }
                }

                strExceptionMessage = lastError.Message;

                if (strExceptionMessage.Contains("服务器无法在已发送 HTTP 标头之后设置状态。") || strExceptionMessage.Contains("favicon.ico"))
                {
                    Server.ClearError();
                }
                else
                {
                    //打印错误日志
                    LogHelper.ErrorLog(strExceptionMessage);

                    /*-----------------------------------------------------
                     * 此处代码可根据需求进行日志记录，或者处理其他业务流程
                     * ---------------------------------------------------*/

                    //一定要调用Server.ClearError()否则会触发错误详情页（就是黄页）
                    if (strExceptionMessage.Contains("数据库") || strExceptionMessage.Contains("用户 'sa' 登录失败。") || strExceptionMessage.Contains("SQL"))
                    {
                        Server.ClearError();
                        Response.WriteFile("~/HttpError/DataBaseError.html");
                    }
                    else
                    {
                        Server.ClearError();
                        Response.WriteFile("~/HttpError/ServerError.html");
                    }
                }
            }
        }
    }
}
