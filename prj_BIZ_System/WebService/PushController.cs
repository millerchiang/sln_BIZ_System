using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using prj_BIZ_System.Extensions;

namespace prj_BIZ_System.WebService
{
    public class PushController : ApiController
    {
        private PushService pushService = new PushService();

        [HttpPost]
        public object MobileDeviceInfoInsert(MobileDeviceInfoModel model)
        {
            var mobileDevice = pushService.getMobileDeviceInfo(model);
            string message = string.Format("Mobile device has exist.");
            return mobileDevice == null ?
                Request.CreateResponse(HttpStatusCode.OK, pushService.MobileDeviceInfoInsertOne(model)) :
                Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
        }

        [HttpPost]
        public object MobileDeviceInfoDelete(MobileDeviceInfoModel model)
        {
            if (model == null || model.device_id.IsNullOrEmpty() || model.user_id.IsNullOrEmpty()) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "data is null");
            var reslut = pushService.MobileDeviceDeleteOne(model);
            return Request.CreateResponse(HttpStatusCode.OK, reslut);
        }
    }
}
