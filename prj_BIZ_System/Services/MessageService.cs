using prj_BIZ_System.Models;
using System.Collections.Generic;

namespace prj_BIZ_System.Services
{
    public class MessageService : _BaseService
    {
        public IList<MsgPrivateModel> SelectMsgPrivate(string keyword)
        {
            var param = new MsgPrivateModel { msg_title = keyword };
            return mapper.QueryForList<MsgPrivateModel>("Message.SelectMsgPrivate", param);
        }

        public void  InsertMsgPrivate(MsgPrivateModel param)
        {
            mapper.Insert("Message.InsertMsgPrivate", param);
        }

        public MsgPrivateModel SelectMsgPrivateOne(int msg_no)
        {
            MsgPrivateModel param = new MsgPrivateModel() { msg_no = msg_no };
            return mapper.QueryForObject<MsgPrivateModel>("Message.SelectMsgPrivateOne", param);
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

    }
}