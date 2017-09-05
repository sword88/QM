using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using log4net;

namespace QM.Demo.Quituser
{
    public class Common
    {
        private readonly static ILog log = LogManager.GetLogger(typeof(Common));
        private static string dbcon = "DATA SOURCE=10.68.10.18:1521/whdb.asewh.com;PASSWORD=sfmg1018;USER ID=sfmg";
        private static string mdbcon = "DATA SOURCE=10.68.10.110:1588/mesdb2;PASSWORD=mesbrft00;USER ID=mesbr";
        public static QMDBHelper db = new QMDBHelper(dbcon);
        public static QMDBHelper mdb = new QMDBHelper(mdbcon);
        private static ZKSHandler zks = ZKSHandler.getInstance();

        public static void QuitUser()
        {
            try
            {
                string sql = "SELECT EMPNO,REF_EMPNO,TAGID,NAME_CN FROM USER_LIST WHERE QUIT_CHECK IS NULL AND STATUS='N'";
                DataSet ds = db.ExecuteDataset(sql, null);
                DataTable dt = null;
                if (ds != null && ds.Tables[0] != null)
                {
                    dt = ds.Tables[0];
                    int count = dt.Rows.Count;
                    UserInfo[] userInfo = new UserInfo[count];
                    for (int i = 0; i < count; i++)
                    {
                        string empno = dt.Rows[i]["EMPNO"].ToString();

                        UserInfo user = new UserInfo();
                        user.Id = dt.Rows[i]["EMPNO"].ToString();
                        user.Name = dt.Rows[i]["NAME_CN"].ToString();
                        user.Enabled = true;
                        user.Privilege = 1;
                        userInfo[i] = user;

                        string date = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                        mdb.ExecuteNonQuery("UPDATE FWUSERPROFILE SET EXPIRATIONDATE = '" + date + "' WHERE USERNAME = '" + empno + "'", CommandType.Text, null);
                        mdb.ExecuteNonQuery("UPDATE FWUSERPROFILE@MESDB1 SET EXPIRATIONDATE = '" + date + "' WHERE USERNAME = '" + empno + "'", CommandType.Text, null);
                    }

                    zks.QuitUser(userInfo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }

        public static void QuitUserByDevice()
        {
            try
            {
                int cnt = int.Parse(GetQuitCount().ToString());
                log.Debug(string.Format("离职人员共计{0}",cnt));

                if (cnt > 20)
                {
                    IList<string> lists = Common.GetDeviceList();

                    foreach (var item in lists)
                    {
                        zks.QuitUserByDvc(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获得离职人员所在的设备
        /// </summary>
        /// <param name="empno"></param>
        /// <returns></returns>
        public static IList<string> GetGateListByUser(string empno)
        {
            IList<string> ip = new List<string>();
            string sql = "SELECT DISTINCT A.DEVICE_IP FROM RFDEVICE_TAG_REG A,RFDEVICE_CONFIG B " 
                       + " WHERE A.EMPNO = '" + empno + "' AND A.DEVICE_IP=B.DEVICE_IP and b.status='A'";

            DataSet ds = db.ExecuteDataset(sql, null);

            if (ds != null && ds.Tables[0] != null)
            {
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    ip.Add(item[0].ToString());
                }
            }

            return ip;
        }

        /// <summary>
        /// 获得用户清单
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static IList<string> GetUserByGate(string ip)
        {
            IList<string> user = new List<string>();
            string sql = "SELECT DISTINCT A.EMPNO FROM RFDEVICE_TAG_REG A, RFDEVICE_CONFIG B ,USER_LIST C "
                       + " WHERE A.DEVICE_IP = '" + ip + "' AND A.DEVICE_IP=B.DEVICE_IP and b.status='A' AND C.EMPNO = A.EMPNO AND C.STATUS='N'";

            DataSet ds = db.ExecuteDataset(sql, null);

            if (ds != null && ds.Tables[0] != null)
            {
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    user.Add(item[0].ToString());
                }
            }

            return user;
        }

        /// <summary>
        /// 查询卡机5位工号by工号
        /// </summary>
        /// <param name="empno"></param>
        /// <returns></returns>
        public static object GetRefnoByUser(string empno)
        {
            string sql = "SELECT REF_EMPNO FROM USER_LIST WHERE EMPNO = '" + empno + "'";

            return db.ExecuteScalar(CommandType.Text, sql, null);
        }

        /// <summary>
        /// 查询工号by卡号
        /// </summary>
        /// <param name="tagid"></param>
        /// <returns></returns>
        public static object GetUserByTagid(string tagid)
        {
            string sql = "SELECT EMPNO FROM USER_LIST WHERE TAGID = '" + tagid + "'";

            return db.ExecuteScalar(CommandType.Text, sql, null);
        }


        public static void InsertTagReg(DeviceTagRegData t)
        {
            string sql = "INSERT INTO RFDEVICE_TAG_REG(DEVICE_IP, DEVICE_PORT, EMPNO, REG_DATE, TAGID) "
                        + " VALUES(:device_ip,:device_port,:empno,:reg_date,:tagid) ";

            OracleParameter[] param = new OracleParameter[]
            {
                new OracleParameter(":device_ip",t.device_ip),
                new OracleParameter(":device_port",t.device_port),
                new OracleParameter(":empno",t.empno),
                new OracleParameter(":reg_date",t.reg_date),
                new OracleParameter(":tagid",t.tagid)
            };

            db.ExecuteNonQuery(sql, CommandType.Text, param);
        }

        /// <summary>
        /// 查询工号by卡机5位工号
        /// </summary>
        /// <param name="empno"></param>
        /// <returns></returns>
        public static object GetUserByRefno(string refempno)
        {
            string sql = "SELECT EMPNO FROM USER_LIST WHERE REF_EMPNO = '" + refempno + "'";

            return db.ExecuteScalar(CommandType.Text, sql, null);
        }

        /// <summary>
        /// 获得设备类型
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static object GetDeviceType(string ip)
        {
            string sql = "SELECT UNIQUE USE_KIND FROM RFDEVICE_CONFIG WHERE DEVICE_IP = '" + ip + "'";

            return db.ExecuteScalar(CommandType.Text,sql,null);
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="empno"></param>
        public static int DelDevice(string ip = "", string empno = "")
        {
            string sql = "DELETE FROM RFDEVICE_TAG_REG WHERE DEVICE_IP = '" + ip + "'";
            if (empno != "")
            {
                sql += " AND EMPNO = '" + empno + "'";
            }

            return db.ExecuteNonQuery(sql, CommandType.Text, null);
        }

        /// <summary>
        /// 获得设备清单
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetDeviceList(string type="")
        {
            IList<string> ip = new List<string>();
            string sql = "SELECT A.DEVICE_IP FROM RFDEVICE_CONFIG A WHERE STATUS='A'";
            if (type != "")
            {
                sql += "AND USE_KIND = '" + type + "'";
            }

            DataSet ds = db.ExecuteDataset(sql, null);

            if (ds != null && ds.Tables[0] != null)
            {
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    ip.Add(item[0].ToString());
                }
            }

            return ip;
        }

        /// <summary>
        /// 更新设备状态
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="status"></param>
        public static void UpdateDeviceConfig(string ip, string status)
        {
            string sql = "UPDATE RFDEVICE_CONFIG SET DEVICE_STATUS = '" + status + "' WHERE DEVICE_IP = '" + ip + "'";

            db.ExecuteNonQuery( sql, CommandType.Text, null);
        }


        public static object GetQuitCount()
        {
            string sql = "SELECT  COUNT(*) FROM  USER_LIST WHERE STATUS='N'AND QUIT_CHECK IS NULL";

            return db.ExecuteScalar(CommandType.Text, sql, null);
        }

        public static void UpdateUserStatus(string empno)
        {
            string sql = "UPDATE USER_LIST SET QUIT_CHECK = 'Y' WHERE EMPNO = '" + empno + "'";

            db.ExecuteNonQuery(sql, CommandType.Text, null);
        }
    }

    [Serializable]
    public class UserInfo
    {
        public string Id;
        public string Name;
        public string Password;
        public int Privilege;
        public bool Enabled;
        public string CardNo;

        public string ToLogString()
        {
            return string.Concat("UserInfo Log", Environment.NewLine
                                , "Id         : ", Id.ToString(), Environment.NewLine
                                , "Name       : ", Name, Environment.NewLine
                                , "Password   : ", Password, Environment.NewLine
                                , "Privilege  : ", Privilege.ToString(), Environment.NewLine
                                , "Enabled    : ", Enabled.ToString(), Environment.NewLine
                                , "CardNo     : ", CardNo);
        }
    }
    [Serializable]
    public class ResultSet
    {
        public bool SeccessFlag;
        public string Key;
        public string Result;
        public string Description;

        public string ToLogString()
        {
            return string.Concat("ResultSet Log", Environment.NewLine
                                , "SeccessFlag : ", SeccessFlag.ToString(), Environment.NewLine
                                , "Key         : ", Key, Environment.NewLine
                                , "Result      : ", Result, Environment.NewLine
                                , "Description : ", Description, Environment.NewLine);
        }
    }

    public class DeviceTagRegData
    {
        public string device_ip { get; set; }
        public int device_port { get; set; }
        public string empno { get; set; }
        public DateTime reg_date { get; set; }
        public string tagid { get; set; }
    }
}
