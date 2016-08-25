using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using prj_BIZ_System.WebService.Model;
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

            var clusterInfoList = clusterService.GetClusterInfoListkw(user_id)
                .Select(clusterInfoModel =>
                    new Cluster
                    {
                        cluster_no = clusterInfoModel.cluster_no,
                        cluster_name = clusterInfoModel.cluster_name,
                        user_id = clusterInfoModel.user_id,
                        cluster_info = clusterInfoModel.cluster_info
                    }                              
                ).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, clusterInfoList);
        }


    }
}
