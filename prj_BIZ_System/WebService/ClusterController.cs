using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiContrib.ModelBinders;

namespace prj_BIZ_System.WebService
{
    [MvcStyleBinding]
    public class ClusterController : ApiController
    {
        private ClusterService clusterService = new ClusterService();

        [HttpGet]
        public object GetClusterInfoList(string user_id)
        {
            if (user_id == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User id is null.");
            return Request.CreateResponse(HttpStatusCode.OK, clusterService.GetClusterInfoListkw(user_id));
        }


    }
}
