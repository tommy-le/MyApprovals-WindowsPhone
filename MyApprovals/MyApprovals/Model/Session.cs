using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApprovals.Util;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;

namespace MyApprovals.Model
{
    [DataContract]
    public class Session
    {
        [DataMember]
        public bool LoggedIn { get; set; }
        [DataMember]
        public bool WrongLogin { get; set; }
         [DataMember]
        public string Sid { get; set; }
         [DataMember]
        public string StoreURL { get; set; }
         [DataMember]
        public string UserId { get; set; }

        public static Session GetSavedSession()
        {
            Session session = new Session();
            Settings settings = new Settings();
            session.LoggedIn = settings.GetValueOrDefault<bool>(Constant.KEY_LOGGED_IN, false);
            session.Sid = settings.GetValueOrDefault<string>(Constant.KEY_SID, null);
            session.StoreURL = settings.GetValueOrDefault<string>(Constant.KEY_STORE_URL, null);
            session.WrongLogin = settings.GetValueOrDefault<bool>(Constant.KEY_WRONG_LOGIN, true);
            session.UserId = settings.GetValueOrDefault<string>(Constant.KEY_USER_ID, null);
           
            //session.StoreURL = Constant.URL_DEFAULT;
		    return session;

        }
        
        public void Save()
        {
            Settings settings = new Settings();
            settings.AddOrUpdateValue(Constant.KEY_LOGGED_IN, this.LoggedIn);
            settings.AddOrUpdateValue(Constant.KEY_SID, this.Sid);
            settings.AddOrUpdateValue(Constant.KEY_STORE_URL, this.StoreURL);
            settings.AddOrUpdateValue(Constant.KEY_WRONG_LOGIN, this.WrongLogin);
            settings.AddOrUpdateValue(Constant.KEY_USER_ID, this.UserId);
            settings.Save();

        }

        public void Logout()
        {
          
            this.LoggedIn = false;
            this.Sid = null;
            //this.UserId = null;
            this.WrongLogin = false;
            this.Save();
        }

        public void Expire()
        {
            this.LoggedIn = false;
            this.Sid = null;
            this.Save();
        }
    }
}
