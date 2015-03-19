using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApprovals.Util
{
    class Constant
    {
        public const string URL_DEFAULT = "http://emrill.36s.dtndev.com";
        public const bool TEST_OFFLINE = false;

        public const string APP_URL = "/mobiapp/";

        public const int ERROR = 1;
        public const int FAILED = -1;
        public const int SUCCESS = 0;
	
	    public const int PAGE_SIZE = 10;
        public const int START_PAGE = 1;
    
        public const int BIZ_NONE = 0;
        public const int BIZ_LOGIN = 1;
        public const int BIZ_GET_MY_ORDERS = 2;
        public const int BIZ_NEXT_APPROVER = 3;
        public const int BIZ_OPTIONAL_APPROVER = 4;
        public const int BIZ_GET_APPROVERS = 5;
        public const int BIZ_ADD_COMMENT = 6;
        public const int BIZ_APPROVE = 7;
        public const int BIZ_REJECT = 8;
        public const int BIZ_GET_ORDER_DETAIL = 9;
        public const int BIZ_REGISTER_DEVICE = 10;
        public const int BIZ_SEND_PUSH_URI = 20;

        public const int BIZ_APPROVE_NEXT = 71;
        public const int BIZ_APPROVE_OPTIONAL = 72;
    
    
        public const int VALUE_ERROR = 0;
        public const int VALUE_OK = 1;
        public const int VALUE_EXPIRED = 2;
    
        public const string KEY_SID = "SID";
        public const string KEY_STORE_URL ="store_url";
        public const string KEY_SESSION ="session";
        public const string KEY_SUB_TOTAL ="sub_total";
        public const string KEY_QTY = "ordered_qty";
        public const string KEY_LOGGED_IN ="logged_in";
        public const string KEY_WRONG_LOGIN ="wrong_login";
        public const string KEY_REJECTED ="rejected";
    

        public const string KEY_RESULT_CODE ="result_code";
        public const string KEY_RESULT ="result";
        public const string KEY_MSG ="message";
        public const string KEY_ID ="id";
        public const string KEY_NAME ="name";
        public const string KEY_INCREMENT_ID ="increment_id";
    
        public const string KEY_CREATED_AT ="created_at";
        public const string KEY_CREATED_DATE ="created_date";
        public const string KEY_CREATED_TIME ="created_time";
    
        public const string KEY_GROUPED_BY_DATE ="grouped_by_date";
        public const string KEY_ORDERED_BY ="ordered_by";
        public const string KEY_CONTRACT_NAME ="contract_name";
        public const string KEY_CONTRACT = "contract";
        public const string KEY_PUSH_URI = "push_uri";
        public const string KEY_STATUS ="status";
        public const string KEY_ORDERS ="orders";
        public const string KEY_ORDER ="order";
        public const string KEY_TOTAL ="total";
        public const string KEY_APPROVERS ="approvers";
        public const string KEY_NEXT_APPROVER ="next";
        public const string KEY_OPTIONAL_APPROVER ="optional";
        public const string KEY_CAN_GIVE_FINAL = "can_give_final";
        public const string KEY_COMMENT = "comment";
        public const string KEY_COMMENTS = "comments";
        public const string KEY_ITEMS = "items";
        public const string KEY_USER_ID = "user_id";
        public const string KEY_UOM = "uom";
    
	
        public const string FORMAT_DATE_TIME = "yyyy-MM-dd HH:mm:ss";
        public const string FORMAT_DATE ="yyyy-MM-dd";
        //public const string FORMAT_TIME ="HH:mm";
        public const string FORMAT_TIME_AMPM = "h:mm tt";

    
        public const string TAG_LOGIN_DIALOG = "login";
        public const string TAG_MESSAGE_DIALOG = "message";
    
        public const string TAG_NEXT_APPROVER_DIALOG = "next";
        public const string TAG_OPTIONAL_APPROVER_DIALOG = "optional";
        public const string TAG_COMMENT_DIALOG = "comment";
        public const string TAG_COMMENT_REJECT_DIALOG = "comment_reject";
        public const string TAG_CONFIRM_APPROVE = "confirm_approve";
        public const string TAG_CONFIRM_REJECT = "confirm_reject";
    }
}
