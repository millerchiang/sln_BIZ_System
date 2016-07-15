using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;

namespace prj_BIZ_System.Services
{
    public class MessageService : _BaseService
    {
        public IList<MsgPrivateModel> SelectMsgPrivate(string keyword , string user_id)
        {
            var param = new MsgPrivateModel { msg_title = keyword , creater_id  = user_id };
            return mapper.QueryForList<MsgPrivateModel>("Message.SelectMsgPrivate", param);
        }

        public IList<MsgPrivateModel> SelectMsgPrivateForMobile(string user_id, DateTime create_time)
        {
            var param = new MsgPrivateModel { creater_id = user_id, create_time = create_time };
            return mapper.QueryForList<MsgPrivateModel>("Message.SelectMsgPrivateForMobile", param);
        }

        public bool isOwnViewPower(int msg_no , string user_id)
        {
            var param = new MsgPrivateModel { msg_no = msg_no, creater_id = user_id };
            return (int)(mapper.QueryForObject("Message.isOwnViewPower", param)) > 0;
        }
        

        public object InsertMsgPrivate(MsgPrivateModel param)
        {
            param.msg_member = " " + param.msg_member;
            return mapper.Insert("Message.InsertMsgPrivate", param);
        }

        public MsgPrivateModel SelectMsgPrivateOne(int msg_no)
        {
            MsgPrivateModel param = new MsgPrivateModel() { msg_no = msg_no };
            return mapper.QueryForObject<MsgPrivateModel>("Message.SelectMsgPrivateOne", param);
        }

        public void InsertMsgPrivateFile(long msg_no , string filepath)
        {
            var param = new MsgPrivateFileModel { msg_no = msg_no , msg_file_site = filepath };
            mapper.Insert("Message.InsertMsgPrivateFile", param);
        }

        public IList<MsgPrivateFileModel> SelectMsgPrivateFileByMsg_no(int msg_no)
        {
            MsgPrivateFileModel param = new MsgPrivateFileModel() { msg_no = msg_no };
            return mapper.QueryForList<MsgPrivateFileModel>("Message.SelectMsgPrivateFileByMsg_no", param);
        }

        public IList<MsgPrivateReplyModel> SelectMsgPrivateReplyMsg_no(int msg_no)
        {
            MsgPrivateReplyModel param = new MsgPrivateReplyModel() { msg_no = msg_no };
            return mapper.QueryForList<MsgPrivateReplyModel>("Message.SelectMsgPrivateReplyMsg_no", param);
        }

        public void InsertMsgPrivateReply(MsgPrivateReplyModel param)
        {
            mapper.Insert("Message.InsertMsgPrivateReply", param);
        }
    }
}