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

        //[HttpGet]
        //public IList<MsgPrivate> GetMessagePrivateList(string user_id)
        //{
        //    clusterService.GetClusterInfoListkw(user_id);
        //    return msgPrivates;
        //}
    }
}
