using prj_BIZ_System.App_Start;
using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prj_BIZ_System.Controllers
{
    public class _BaseController : Controller
    {
        public HttpCookie _loginUser { get; set; }
        public string     _loginUserId { get; set; }

        public _BaseController()
        {
            _loginUser = Request.Cookies["UserInfo"];
            if (_loginUser == null ) {
                // 測試用資料
                _loginUserId = "12345678";
            }else
            {
                _loginUserId = Request.Cookies["UserInfo"]["user_id"];
            }
        }
    }
}