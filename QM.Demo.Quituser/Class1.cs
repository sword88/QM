using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.Core.Environments;
using QM.Core.Model;
using QM.Core.Exception;
using QM.Core.Log;
using QM.Core.Data;
using Oracle.ManagedDataAccess.Client;

namespace QM.Demo.Quituser
{
    public class Class1 : DllTask
    {
        private static string dbcon = "DATA SOURCE=10.68.10.18:1521/whdb.asewh.com;PASSWORD=sfmg1018;USER ID=sfmg";
        private static ILogger  log = QMStarter.CreateQMLogger(typeof(Class1));
        public override void Run()
        {
            try
            {
                QMDBHelper db = new QMDBHelper(dbcon);
                string sql = "SELECT EMPNO,REF_EMPNO,TAGID,NAME_CN FROM USER_LIST WHERE QUIT_CHECK IS NULL AND STATUS='N'";
                DataSet ds = db.ExecuteDataset(sql,null);
                
                MsgHandlerService srv = new MsgHandlerService();

                DataTable dtInLog = null;
                if (ds != null && ds.Tables[0] != null)
                {
                    dtInLog = ds.Tables[0];
                    int count = ds.Tables[0].Rows.Count;
                    for (int i = 0; i < count; i++)
                    {

                        string empno = dtInLog.Rows[i][0].ToString();
                        UserInfo[] userInfo = new UserInfo[1];
                        UserInfo obj = new UserInfo();
                        obj.CardNo = dtInLog.Rows[i][2].ToString();
                        obj.Id = dtInLog.Rows[i][1].ToString();
                        obj.Name = dtInLog.Rows[i][3].ToString();
                        obj.Enabled = true;
                        obj.Privilege = 1;
                        userInfo[0] = obj;
                        //log.Debug(i.ToString());
                        ResultSet rtn = srv.RequestGateWay("QuitUser", "+", userInfo, true);
                        srv.Timeout = 30 * 1000000;
                        if (!rtn.SeccessFlag)
                        {
                            //log.Fatal("delete faild userid: " + obj.Id + "." + rtn.Description);
                        }
                        else
                        {
                            string sqlupdate = "UPDATE USER_LIST SET QUIT_CHECK='Y' WHERE EMPNO ='" + empno + "'";
                            db.ExecuteNonQuery(sqlupdate,CommandType.Text,null);
                        }

                        //log.Info("delete empno " + empno + " finished.");
                    }

                }
            }
            catch (QMException ex)
            {
                throw ex;
            }
        }
    }
}
