using Microsoft.AspNet.SignalR;
using SignalR_POC.Edmx;
using SignalR_POC.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SignalR_POC.Component
{
    public class NotificationComp
    {
        public void Register(DateTime date)
        {
            var connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var sqlCmdStr = @"Select [PostId], [LikeDislike], [User], [Updated] FROM dbo.UserFBLike Where Updated > @date";
            using (var conn = new SqlConnection(connStr))
            {
                var cmd = new SqlCommand(sqlCmdStr, conn);
                cmd.Parameters.AddWithValue("@date", date);
                if(conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }
                cmd.Notification = null;
                var sqlDependency = new SqlDependency(cmd);
                sqlDependency.OnChange += SqlDependency_OnChange;
                using(var reader = cmd.ExecuteReader())
                {

                }
            }
        }

        private void SqlDependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if(e.Type == SqlNotificationType.Change)
            {
                var sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= SqlDependency_OnChange;
                var notificationHub = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                notificationHub.Clients.All.notify("added");
                Register(DateTime.Now);
            }
        }

        public List<UserFBLike> GetLikes(DateTime now)
        {
            using (var db = new PushNotificationsEntities())
            {
                return db.UserFBLikes
                    .Where(x => x.Updated > now)
                    .OrderByDescending(x => x.Updated)
                    .ToList();
            }
        }
    }
}