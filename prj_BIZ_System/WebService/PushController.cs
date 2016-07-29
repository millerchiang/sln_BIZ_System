using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace prj_BIZ_System.WebService
{
    public class PushController : ApiController
    {
        private PushService pushService = new PushService();

        [HttpPost]
        public object MobileDeviceInfoInsert(MobileDeviceInfoModel model)
        {
            var mobileDevice = pushService.getMobileDeviceInfo(model);
            return mobileDevice == null ? 
                pushService.MobileDeviceInfoInsertOne(model) :
                Request.CreateResponse(HttpStatusCode.NotModified, model);
        }
    }
}
