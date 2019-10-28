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

namespace QM.Demo.RFC
{
    public partial class BATCH : Form
    {
        private readonly static ILog log = LogManager.GetLogger(typeof(PO));
        public BATCH()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IList<GOODSMVT_ITEM> dt = new List<GOODSMVT_ITEM>();
            GOODSMVT_ITEM item = new GOODSMVT_ITEM();
            item.material = "17WHP03016A004";
            item.move_type = "292";
            item.plant = "1016";
            item.stge_loc = "5320";
            item.entry_qnt = 1200;
            item.entry_uom = "EA";
            item.batch = "R9391032";
            item.cost_obj = "ASEWH";
            dt.Add(item);
            BAPI_GOODSMVT_CREATE(dt);
        }

        public RfcDestination GetDestination()
        {
            //RfcDestination dest = RfcDestinationManager.GetDestination("PRD");
            RfcDestination dest = RfcDestinationManager.GetDestination("QAS");

            return dest;
        }

        public async Task BAPI_GOODSMVT_CREATE(IList<GOODSMVT_ITEM> dt)
        {
            try
            {
                RfcDestination dest = GetDestination();
                RfcRepository repository = dest.Repository;
                RfcSessionManager.BeginContext(dest);
                IRfcFunction func = repository.CreateFunction("BAPI_GOODSMVT_CREATE");
                string message = null;

                IRfcStructure rfcStructGMItem = null;
                IRfcTable rfcTableGMItem = func.GetTable("GOODSMVT_ITEM");

                IRfcStructure stru;
                IRfcTable result;

                //set parm
                rfcStructGMItem = repository.GetStructureMetadata("BAPI2017_GM_ITEM_CREATE").CreateStructure();
                foreach (var item in dt)
                {
                    rfcStructGMItem.SetValue("MATERIAL", item.material);
                    rfcStructGMItem.SetValue("PLANT", item.plant);
                    rfcStructGMItem.SetValue("STGE_LOC", item.stge_loc);
                    rfcStructGMItem.SetValue("BATCH", item.batch);
                    rfcStructGMItem.SetValue("MOVE_TYPE", item.move_type);
                    rfcStructGMItem.SetValue("ENTRY_QNT", item.entry_qnt);
                    rfcStructGMItem.SetValue("COST_OBJ", item.cost_obj);
                    rfcStructGMItem.SetValue("ENTRY_UOM", item.entry_uom);
                }
                rfcTableGMItem.Insert(rfcStructGMItem);

                IRfcStructure rfcStructGMCode = null;
                rfcStructGMCode = func.GetStructure("GOODSMVT_CODE");

                if (dt.Count > 0)
                {
                    if (dt[0].move_type == "291")
                    {
                        rfcStructGMCode.SetValue("GM_CODE", "03");
                    }
                    else if(dt[0].move_type == "292")
                    {
                        rfcStructGMCode.SetValue("GM_CODE", "06");
                    }
                }                

                IRfcStructure rfcStructGMHeader = null;
                rfcStructGMHeader = func.GetStructure("GOODSMVT_HEADER");
                rfcStructGMHeader.SetValue("PSTNG_DATE", "20191028");
                rfcStructGMHeader.SetValue("DOC_DATE", "20191028");



                //call function
                log.Debug("BAPI_GOODSMVT_CREATE => Start");
                func.Invoke(dest);
                log.Debug("BAPI_GOODSMVT_CREATE => Done");

                //log result
                result = func.GetTable("RETURN");

                for (int k = 0; k < result.RowCount; k++)
                {
                    stru = result[k];
                    message = stru.GetValue("MESSAGE").ToString();
                    log.Debug(string.Format("MESSAGE:{0}",  message));
                }

                if (result.RowCount == 0)
                {
                    string doc = func.GetValue("MATERIALDOCUMENT").ToString();
                    string year = func.GetValue("MATDOCUMENTYEAR").ToString();

                    func = repository.CreateFunction("BAPI_TRANSACTION_COMMIT");
                    func.SetValue("WAIT", "X");
                    func.Invoke(dest);

                    stru = func.GetStructure("RETURN");
                    MessageBox.Show(doc);
                }
                else
                {
                    func = repository.CreateFunction("BAPI_TRANSACTION_ROLLBACK");
                    func.Invoke(dest);
                    MessageBox.Show(message);
                }

                RfcSessionManager.EndContext(dest);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

    public class GOODSMVT_ITEM
    {
        public string material { get; set; }
        public string plant { get; set; }
        public string stge_loc { get; set; }
        public string batch { get; set; }
        public string move_type { get; set; }    
        public long entry_qnt { get; set; }
        public string cost_obj { get; set; }
        public string entry_uom { get; set; }
    }

    public class GOODSMVT_HEADER
    { 
        public DateTime pstng_date { get; set; }
        public DateTime doc_date { get; set; }
    }

    public class GOODSMVT_CODE
    { 
        public string gm_code { get; set; }
    }

    public class GOODSMVT_RETURN
    { 
        public string message { get; set; }
    }
}
