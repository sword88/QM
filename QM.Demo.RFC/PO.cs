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

namespace QM.Demo.RFC
{
    public partial class PO : Form
    {
        private readonly static ILog log = LogManager.GetLogger(typeof(PO));
        private static string dbcon = ConfigurationManager.ConnectionStrings["database"].ConnectionString;
        public static QMDBHelper db = new QMDBHelper(dbcon);

        public PO()
        {
            InitializeComponent();
        }

        public RfcDestination GetDestination()
        {
            RfcDestination dest = RfcDestinationManager.GetDestination("PRD");

            return dest;
        }

        private void ClosePO(string date)
        {
            try
            {
                RfcDestination dest = GetDestination();
                RfcRepository repository = dest.Repository;
                IRfcFunction func = repository.CreateFunction("BAPI_PRODORD_CLOSE");

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
                sb.Append("AND A.ERDAT = '" + date + "' ");

                log.Debug(sb.ToString());
                DataSet ds = db.ExecuteDataset(sb.ToString(), null);
                DataTable dt = null;

                IRfcStructure rfcStruct = null;
                IRfcTable rfcTable = func.GetTable("ORDERS");

                if (ds != null && ds.Tables[0] != null)
                {
                    dt = ds.Tables[0];
                    int count = dt.Rows.Count;
                    log.Debug(string.Format("Total Row Count:{0}", count));

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            log.Debug(dt.Rows[i][0].ToString());
                            //set parm
                            rfcStruct = repository.GetStructureMetadata("BAPI_ORDER_KEY").CreateStructure();
                            rfcStruct.SetValue("ORDER_NUMBER", dt.Rows[i][0].ToString());
                            rfcTable.Insert(rfcStruct);
                        }

                        //call function
                        log.Debug("BAPI_PRODORD_CLOSE => Start");
                        func.Invoke(dest);
                        log.Debug("BAPI_PRODORD_CLOSE => Done");

                        //log result
                        IRfcTable result = func.GetTable("DETAIL_RETURN");
                        for (int i = 0; i < result.RowCount; i++)
                        {
                            IRfcStructure stru = result[i];
                            log.Debug(string.Format("SYSTEM:{0},PPNUMBER:{1},Type:{2},Message:{3}", stru.GetValue("SYSTEM").ToString(),
                                stru.GetValue("ORDER_NUMBER").ToString(), stru.GetValue("TYPE").ToString(), stru.GetValue("MESSAGE").ToString())
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Fatal(date + "  " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string date = "";
            for (int i = 0; i <= (edate.Value - sdate.Value).Days; i++)
            {
                date = sdate.Value.AddDays(i).ToString("yyyyMMdd");
                log.Debug(date);
                ClosePO(date);
            }
        }
    }
}
