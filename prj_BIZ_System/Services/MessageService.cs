using prj_BIZ_System.Controllers;
using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace prj_BIZ_System.Services
{
    public class MessageService : _BaseService
    {
        private UserService userService;
        private SalesService salesService;

        public MessageService()
        {
            userService = new UserService();
            salesService = new SalesService();
        }
        
        public IList<UserInfoModel> SelectUserKw(string user_id , string kw)
        {
            UserInfoModel param = new UserInfoModel() { user_id = user_id , company = kw };
            return mapper.QueryForList<UserInfoModel>("Message.SelectUserKw", param);
        }

        public IList<UserInfoModel> SelectUserKwForMobile(string user_id)
        {
            UserInfoModel param = new UserInfoModel() { user_id = user_id };
            return mapper.QueryForList<UserInfoModel>("Message.SelectUserKwForMobile", param);
        }

        public IList<UserInfoModel> SelectUserUserBySalesId(string sales_id, string term)
        {
            SalesInfoModel param = new SalesInfoModel() { sales_id = sales_id , sales_name = term }; //term是公司name的條件
            return mapper.QueryForList<UserInfoModel>("Message.SelectUserUserBySalesId", param);
        }

        public IList<SalesInfoModel> SelectSalesKw(string user_id, string kw)
        {
            SalesInfoModel param = new SalesInfoModel() { user_id = user_id, sales_name = kw };
            return mapper.QueryForList<SalesInfoModel>("Message.SelectSalesKw", param);
        }

        public IList<MsgModel> SelectMsgPrivate(string keyword , string user_id)
        {
            var param = new MsgModel { msg_title = keyword , creater_id  = user_id , user_id = "0"};
            return mapper.QueryForList<MsgModel>("Message.SelectMsg", param);
        }

        public IList<MsgModel> SelectMsgPrivateForMobile(string user_id, DateTime create_time)
        {
            var param = new MsgModel { creater_id = user_id, user_id = "0", create_time = create_time };
            return mapper.QueryForList<MsgModel>("Message.SelectMsgForMobile", param);
        }

        public bool isOwnViewPower(int msg_no , string user_id)
        {
            var param = new MsgModel { msg_no = msg_no, creater_id = user_id };
            return (int)(mapper.QueryForObject("Message.isOwnViewPower", param)) > 0;
        }

        public bool isOwnViewPowerForCompany(int msg_no, string user_id, string sales_id)
        {
            var target_id = string.IsNullOrEmpty(sales_id) ? user_id : sales_id;
            var param = new MsgModel { msg_no = msg_no, creater_id = user_id };
            return (int)(mapper.QueryForObject("Message.isOwnViewPowerForCompany", param)) > 0;
        }

        public bool isOwnViewPowerForClusterPublic(int msg_no, int cluster_no , string user_id)
        {
            var param = new MsgModel { msg_no = msg_no, creater_id = user_id , cluster_no = cluster_no, is_public = "1" };
            return (int)(mapper.QueryForObject("Message.isOwnViewPowerForCluster", param)) > 0;
        }

        public bool isOwnViewPowerForClusterPrivate(int msg_no, int cluster_no, string user_id)
        {
            var param = new MsgModel { msg_no = msg_no, creater_id = user_id, cluster_no = cluster_no ,is_public="0" };
            return (int)(mapper.QueryForObject("Message.isOwnViewPowerForCluster", param)) > 0;
        }

        public object InsertMsgPrivate(MsgModel param)
        {
            param.msg_member = " " + param.msg_member;
            param.is_public = "0";
            param.user_id = "0";
            param.cluster_no = 0;
            return mapper.Insert("Message.InsertMsg", param);
        }

        public object InsertMsgCluster(MsgModel param)
        {
            param.msg_member = " " + param.msg_member;
            param.user_id = "0";
            return mapper.Insert("Message.InsertMsg", param);
        }

        public MsgModel SelectMsgPrivateOne(long msg_no)
        {
            MsgModel param = new MsgModel() { msg_no = msg_no };
            return mapper.QueryForObject<MsgModel>("Message.SelectMsgOne", param);
        }

        public MsgModel SelectMsgPrivateOneAndRead(int msg_no, string user_id)
        {
            MsgModel param = new MsgModel() { msg_no = msg_no, user_id = user_id };
            return mapper.QueryForObject<MsgModel>("Message.SelectMsgOneAndRead", param);
        }

        public IList<long> SelectMsgReadNo(string user_id)
        {
            MsgModel param = new MsgModel() { user_id = user_id };
            return mapper.QueryForList<long>("Message.SelectMsgReadNo", param);
        }

        public string transferMsg_member2Msg_company(string msg_member, prj_BIZ_System.Controllers.MessageCatalog catalog)
        {
            string result = "";
            StringBuilder sb = new StringBuilder();
            if(!string.IsNullOrEmpty(msg_member)){
                string[] msg_member_arr = msg_member.Split(',');
                const string separate = " , ";
                for (int i=0; i< msg_member_arr.Length;i++)
                {
                    // msg_member_arr[i] 為 user_id
                    switch (catalog)
                    {
                        case Controllers.MessageCatalog.Private:
                            UserInfoModel userInfoModel = userService.GeUserInfoOne(msg_member_arr[i].Trim());
                            if (userInfoModel != null)
                            {
                                sb.Append(separate);
                                sb.Append(userInfoModel.company);
                            }
                            break;
                        case Controllers.MessageCatalog.Company:
                            SalesInfoModel salesInfoModel = salesService.GeSalesInfoOne(msg_member_arr[i].Trim());
                            if (salesInfoModel != null)
                            {
                                sb.Append(separate);
                                sb.Append(salesInfoModel.sales_name);
                            }
                            break;
                    }
                }
                if (msg_member_arr.Length>0 && sb.ToString().Length > separate.Length)
                {
                    result = sb.ToString().Substring(separate.Length);
                }
            }
            return result;
        }

        public Tuple<string, string> transferMsg_member2Msg_company_AndEn(string msg_member)
        {
            string result = "";
            string result_en = "";
            StringBuilder sb = new StringBuilder();
            StringBuilder sb_en = new StringBuilder();
            if (!string.IsNullOrEmpty(msg_member))
            {
                string[] msg_member_arr = msg_member.Split(',');
                const string separate = " , ";
                for (int i = 0; i < msg_member_arr.Length; i++)
                {
                    UserInfoModel userInfoModel = userService.GeUserInfoOne(msg_member_arr[i].Trim());
                    if (userInfoModel != null)
                    {
                        sb.Append(separate);
                        sb.Append(userInfoModel.company);
                        sb_en.Append(separate);
                        string company_en = string.IsNullOrEmpty(userInfoModel.company_en) ? 
                                            userInfoModel.company : 
                                            userInfoModel.company_en;
                        sb_en.Append(company_en);
                    }
                }
                if (msg_member_arr.Length > 0 && sb.ToString().Length > separate.Length)
                {
                    result = sb.ToString().Substring(separate.Length);
                    result_en = sb_en.ToString().Substring(separate.Length);
                }
            }
            return new Tuple<string, string>(result, result_en);
        }

        public void InsertMsgPrivateFile(long msg_no , string filepath)
        {
            var param = new MsgFileModel { msg_no = msg_no , msg_file_site = filepath };
            mapper.Insert("Message.InsertMsgFile", param);
        }

        public void InsertMsgPrivateReplyFile(long msg_reply_no, string filepath)
        {
            var param = new MsgReplyFileModel { msg_reply_no = msg_reply_no, msg_reply_file_site = filepath };
            mapper.Insert("Message.InsertMsgReplyFile", param);
        }

        public IList<MsgPushModel> getPushMdFromCreateMsg(MsgModel msgMd)
        {
            IList<MsgPushModel> result = new List<MsgPushModel>();
            if (msgMd != null && !string.IsNullOrEmpty(msgMd.msg_member))
            {
                string[] msg_member_arr = msgMd.msg_member.Split(',');
                msg_member_arr = msg_member_arr.Select(msg_member => msg_member.Trim()).ToArray();
                ISet<string> msg_member_set = new HashSet<string>(msg_member_arr);
                //msg_member_set.Add(msgMd.creater_id);
                List<UserInfoModel> temp_result = new List<UserInfoModel>();
                foreach (string user_ids in msg_member_set)
                {
                    var temp = mapper.QueryForList<UserInfoModel>("Message.SelectMsgMembers", user_ids);
                    if (temp != null && temp.Count > 0)
                    {
                        temp_result.AddRange(temp);
                    }
                }

                var createrInfo = mapper.QueryForObject<UserInfoModel>("Message.SelectMsgMembersByLeft", msgMd.creater_id);
                result = temp_result
                    //.Where(userMd => !userMd.user_id.Equals(rpyMd.msg_reply))
                    .Select(userMd => new MsgPushModel()
                    {
                          msg_type = getMsgType(msgMd)
                        , msg_no = msgMd.msg_no
                        , msg_title = msgMd.msg_title
                        , msg_content = msgMd.msg_content
                        //, reply_user_id = rpyMd.msg_reply
                        , company = createrInfo.company
                        , company_en = createrInfo.company_en
                        , msg_reply_no = 0      //pyMd.msg_reply_no   //手機端判斷依據
                        , reply_content = null  //rpyMd.reply_content //手機端判斷依據
                        , device_id = userMd.device_id
                        , device_os = userMd.device_os
                    }).ToList();
            }
            return result;
        }

        public IList<MsgPushModel> getPushMdFromReply(MsgReplyModel rpyMd , MsgModel msgMd)
        {
            IList<MsgPushModel> result = new List<MsgPushModel>();
            if (msgMd!=null && !string.IsNullOrEmpty(msgMd.msg_member))
            {
                string[] msg_member_arr = msgMd.msg_member.Split(',');
                msg_member_arr = msg_member_arr.Select(msg_member => msg_member.Trim()).ToArray();
                ISet<string> msg_member_set = new HashSet<string>(msg_member_arr);
                msg_member_set.Add(msgMd.creater_id);
                List<UserInfoModel> temp_result = new List<UserInfoModel>();
                foreach (string user_ids in msg_member_set)
                {
                    var temp = mapper.QueryForList<UserInfoModel>("Message.SelectMsgMembers", user_ids);
                    if (temp!=null && temp.Count>0)
                    {
                        temp_result.AddRange(temp);
                    }
                }

                var replyInfo = mapper.QueryForObject<UserInfoModel>("Message.SelectMsgMembersByLeft", rpyMd.msg_reply);
                result = temp_result
                    .Where(userMd => !userMd.user_id.Equals(rpyMd.msg_reply))
                    .Select(userMd => new MsgPushModel()
                    {
                          msg_type = getMsgType(msgMd)
                        , msg_no = msgMd.msg_no
                        , msg_title = msgMd.msg_title
                        , msg_content = msgMd.msg_content
                        //, reply_user_id = rpyMd.msg_reply
                        , company = replyInfo.company
                        , company_en = replyInfo.company_en
                        , msg_reply_no = rpyMd.msg_reply_no
                        , reply_content = rpyMd.reply_content
                        , device_id = userMd.device_id
                        , device_os = userMd.device_os
                    }).ToList();
                
            }
            return result;
        }

        private int getMsgType(MsgModel msgMd)
        {
            int cluster_no = (int)(msgMd.cluster_no);
            string user_id = msgMd.user_id;
            string is_public = msgMd.is_public;
            int result = 0;

            MessageType msg_type;
            try
            {

                if (string.IsNullOrEmpty(user_id) || user_id.Equals("0"))
                {
                    if (cluster_no == 0) {
                        msg_type = MessageType.Person;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(is_public)||is_public.Equals("0"))
                        {
                            msg_type = MessageType.ClusterPrivate;
                        }
                        else
                        {
                            msg_type = MessageType.ClusterPublic;
                        }
                    }
                }
                else
                {
                    msg_type = MessageType.CompanyPrivate;
                }

                result = (int)msg_type;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                result = 0;
            }

            return result;
        }

        public void InsertMsgReplyFile(long msg_reply_no, string filepath)
        {
            var param = new MsgReplyFileModel { msg_reply_no = msg_reply_no, msg_reply_file_site = filepath };
            mapper.Insert("Message.InsertMsgReplyFile", param);
        }

        public IList<MsgFileModel> SelectMsgPrivateFileByMsg_no(int msg_no)
        {
            MsgFileModel param = new MsgFileModel() { msg_no = msg_no };
            return mapper.QueryForList<MsgFileModel>("Message.SelectMsgFileByMsg_no", param);
        }

        public IList<MsgReplyModel> SelectMsgPrivateReplyMsg_no(int msg_no)
        {
            MsgReplyModel param = new MsgReplyModel() { msg_no = msg_no };
            return mapper.QueryForList<MsgReplyModel>("Message.SelectMsgReplyMsg_no", param);
        }

        public IList<MsgReplyFileModel> SelectMsgReplyFileByMsg_no(int msg_no)
        {
            MsgFileModel param = new MsgFileModel() { msg_no = msg_no };
            return mapper.QueryForList<MsgReplyFileModel>("Message.SelectMsgReplyFileByMsg_no", param);
        }

        public object InsertMsgPrivateReply(MsgReplyModel param)
        {
            var result = mapper.Insert("Message.InsertMsgReply", param);
            return result;
        }

        #region --公司訊息--
        public IList<MsgModel> SelectMsgCompany(string keyword, string user_id)
        {
            var param = new MsgModel { msg_title = keyword, creater_id = user_id , user_id = user_id };
            return mapper.QueryForList<MsgModel>("Message.SelectMsg", param);
        }

        public object InsertMsgCompany(MsgModel param)
        {
            param.msg_member = " " + param.msg_member;
            param.is_public = "0";
            return mapper.Insert("Message.InsertMsg", param);
        }
        
        public IList<MsgModel> SelectMsgClusterPublic(string cluster_no , string user_id, string kw)
        {
            var cluster_no_int = 0;
            var param = new MsgModel();
            if (Int32.TryParse(cluster_no, out cluster_no_int))
            {
                param = new MsgModel() { cluster_no = cluster_no_int , is_public ="1", msg_member = null , msg_title = kw , creater_id = user_id };
            }
            return mapper.QueryForList<MsgModel>("Message.SelectClusterMsg", param);
        }

        public IList<MsgModel> SelectMsgClusterPrivate(string cluster_no , string user_id, string kw)
        {
            var cluster_no_int = 0;
            var param = new MsgModel();
            if (Int32.TryParse(cluster_no, out cluster_no_int))
            {
                param = new MsgModel() { cluster_no = cluster_no_int , is_public = "0" , msg_title = kw , creater_id = user_id };
            }
            var result =  mapper.QueryForList<MsgModel>("Message.SelectClusterMsg", param);
            return result;
        }

        public IList<ClusterInfoModel> SelectClusterByMsg_no(int msg_no)
        {
            MsgReplyModel param = new MsgReplyModel() { msg_no = msg_no };
            return mapper.QueryForList<ClusterInfoModel>("Message.SelectClusterByMsg_no", param);
        }

        #endregion
    }
}