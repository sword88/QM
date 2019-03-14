using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAP.Middleware.Connector;
using log4net;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace QM.Demo.RFC
{
    public partial class PO : Form
    {
        private readonly static ILog log = LogManager.GetLogger(typeof(PO));
        private static string dbcon = ConfigurationManager.ConnectionStrings["database"].ConnectionString;
        private static string dbcon2 = ConfigurationManager.ConnectionStrings["whdb"].ConnectionString;
        public static QMDBHelper db = new QMDBHelper(dbcon);
        public static QMDBHelper db2 = new QMDBHelper(dbcon2);

        public PO()
        {
            InitializeComponent();
        }

        public RfcDestination GetDestination()
        {
            //RfcDestination dest = RfcDestinationManager.GetDestination("PRD");
            RfcDestination dest = RfcDestinationManager.GetDestination("QAS");

            return dest;
        }

        private void ClosePO(string date)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT A.AUFNR ");
                sb.Append("FROM SAPSR3.AUFK A, SAPSR3.JEST B ");
                sb.Append("WHERE A.OBJNR = B.OBJNR ");
                sb.Append("and A.MANDT = '100' ");
                sb.Append("AND A.AUART = 'PP08' ");
                sb.Append("AND A.LOEKZ = ' ' ");
                sb.Append("AND A.AUTYP = '10' ");
                sb.Append("AND A.WERKS = '1016' ");
                sb.Append("AND B.STAT = 'I0045' ");
                sb.Append("and B.INACT = ' ' ");
                sb.Append("and B.MANDT = '100' ");
                sb.Append("AND A.ERNAM = 'MESINT' ");
                sb.Append("AND A.ERDAT < '" + date + "' ");

                log.Debug(sb.ToString());
                DataSet ds = db.ExecuteDataset(sb.ToString(), null);
                DataTable dt = null;
                IList<string> vs = new List<string>();



                if (ds != null && ds.Tables[0] != null)
                {
                    dt = ds.Tables[0];
                    int count = dt.Rows.Count;
                    log.Debug(string.Format("Total Row Count:{0}", count));

                    if (count > 0)
                    {
                        int j = 0;
                        for (int i = 0; i < count; i++)
                        {
                            log.Debug(dt.Rows[i][0].ToString());
                            vs.Add(dt.Rows[i][0].ToString());

                            j++;
                            if (j == 1000)
                            {
                                var result = BAPI_PRODORD_CLOSE(vs);
                                result.Wait();
                                j = 0;
                            }
                        }

                        if (j > 0)
                        {
                            var result = BAPI_PRODORD_CLOSE(vs);
                            result.Wait();
                            j = 0;
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                OracleParameter[] param = null;
                string sql = string.Format("insert into po_errlog values ('{0}','{1}')", date, ex.Message);
                db2.ExecuteNonQuery(sql, CommandType.Text , param);
                log.Fatal(date + "  " + ex.Message);
            }
        }


        public async Task BAPI_PRODORD_CLOSE(IList<string> dt)
        {
            RfcDestination dest = GetDestination();
            RfcRepository repository = dest.Repository;
            IRfcFunction func = repository.CreateFunction("BAPI_PRODORD_CLOSE");

            IRfcStructure rfcStruct = null;
            IRfcTable rfcTable = func.GetTable("ORDERS");

            IRfcStructure stru;
            IRfcTable result;

            //set parm
            rfcStruct = repository.GetStructureMetadata("BAPI_ORDER_KEY").CreateStructure();
            foreach (var item in dt)
            {
                rfcStruct.SetValue("ORDER_NUMBER", item);
            }            
            rfcTable.Insert(rfcStruct);

            //call function
            log.Debug("BAPI_PRODORD_CLOSE => Start");
            func.Invoke(dest);
            log.Debug("BAPI_PRODORD_CLOSE => Done");

            //log result
            result = func.GetTable("DETAIL_RETURN");
            for (int k = 0; k < result.RowCount; k++)
            {
                stru = result[k];
                log.Debug(string.Format("SYSTEM:{0},PPNUMBER:{1},Type:{2},Message:{3}", stru.GetValue("SYSTEM").ToString(),
                    stru.GetValue("ORDER_NUMBER").ToString(), stru.GetValue("TYPE").ToString(), stru.GetValue("MESSAGE").ToString())
                );
            }
            rfcTable.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string date = "";
            //for (int i = 0; i <= (edate.Value - sdate.Value).Days; i++)
            //{
            //    date = sdate.Value.AddDays(i).ToString("yyyyMMdd");
            //    log.Debug(date);
            //    ClosePO(date);
            //}

            ClosePO(sdate.Value.ToString("yyyyMMdd"));
        }
    }
}
