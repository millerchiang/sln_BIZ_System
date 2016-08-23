using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace prj_BIZ_System.Services
{
    public class MessageService : _BaseService
    {
        private UserService userService;
        public MessageService()
        {
            userService = new UserService();
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

        public IList<MsgModel> SelectMsgPrivate(string keyword , string user_id)
        {
            var param = new MsgModel { msg_title = keyword , creater_id  = user_id };
            return mapper.QueryForList<MsgModel>("Message.SelectMsg", param);
        }

        public IList<MsgModel> SelectMsgPrivateForMobile(string user_id, DateTime create_time)
        {
            var param = new MsgModel { creater_id = user_id, create_time = create_time };
            return mapper.QueryForList<MsgModel>("Message.SelectMsgForMobile", param);
        }

        public bool isOwnViewPower(int msg_no , string user_id)
        {
            var param = new MsgModel { msg_no = msg_no, creater_id = user_id };
            return (int)(mapper.QueryForObject("Message.isOwnViewPower", param)) > 0;
        }
        

        public object InsertMsgPrivate(MsgModel param)
        {
            param.msg_member = " " + param.msg_member;
            param.is_public = "0";
            return mapper.Insert("Message.InsertMsg", param);
        }

        public MsgModel SelectMsgPrivateOne(int msg_no)
        {
            MsgModel param = new MsgModel() { msg_no = msg_no };
            return mapper.QueryForObject<MsgModel>("Message.SelectMsgOne", param);
        }

        public string transferMsg_member2Msg_company(string msg_member)
        {
            string result = "";
            StringBuilder sb = new StringBuilder();
            if(!string.IsNullOrEmpty(msg_member)){
                string[] msg_member_arr = msg_member.Split(',');
                const string separate = " , ";
                for (int i=0; i< msg_member_arr.Length;i++)
                {
                    // msg_member_arr[i] 為 user_id
                    UserInfoModel userInfoModel = userService.GeUserInfoOne(msg_member_arr[i].Trim());
                    if (userInfoModel != null)
                    {
                        sb.Append(separate);
                        sb.Append(userInfoModel.company);
                    }
                }
                if (msg_member_arr.Length>0)
                {
                    result = sb.ToString().Substring(separate.Length);
                }
            }
            return result;
        }

        public void InsertMsgPrivateFile(long msg_no , string filepath)
        {
            var param = new MsgFileModel { msg_no = msg_no , msg_file_site = filepath };
            mapper.Insert("Message.InsertMsgFile", param);
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
        #endregion
    }
}