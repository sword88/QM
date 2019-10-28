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
    public partial class Form1 : Form
    {
        private readonly static ILog log = LogManager.GetLogger(typeof(Form1));

        public Form1()
        {
            InitializeComponent();

        }

        public RfcConfigParameters GetConfigParams()
        {
            RfcConfigParameters configParams = new RfcConfigParameters();

            // Name property is neccessary, otherwise, NonInvalidParameterException will be thrown
            configParams.Add(RfcConfigParameters.Name, "DEV");
            //configParams.Add(RfcConfigParameters.Name, "NPR");
            configParams.Add(RfcConfigParameters.AppServerHost, "10.68.10.63");
            //configParams.Add(RfcConfigParameters.AppServerHost, "10.68.10.118");
            configParams.Add(RfcConfigParameters.SystemNumber, "00"); // instance number
            configParams.Add(RfcConfigParameters.SystemID, "DEV");
            //configParams.Add(RfcConfigParameters.SystemID, "NPR");

            configParams.Add(RfcConfigParameters.User, "104218");
            configParams.Add(RfcConfigParameters.Password, "qwer123");
            configParams.Add(RfcConfigParameters.Client, "320");
            configParams.Add(RfcConfigParameters.Language, "EN");
            configParams.Add(RfcConfigParameters.PoolSize, "5");
            //configParams.Add(RfcConfigParameters.MaxPoolSize, "10");
            //configParams.Add(RfcConfigParameters.IdleTimeout, "30");

            return configParams;
        }

        public RfcDestination GetDestination()
        {
            //RfcConfigParameters configParams = GetConfigParams();
            //RfcDestination dest = RfcDestinationManager.GetDestination(configParams);

            RfcDestination dest = RfcDestinationManager.GetDestination("DEV");

            return dest;
        }

        public async Task<string> ZPP_RFC_PRODORD_CREATE(RfcRepository repository, RfcDestination dest)
        {
            IRfcFunction func = repository.CreateFunction("ZPP_RFC_PRODORD_CREATE");

            //set parm
            func.SetValue("MATNR", "TSTL24NM60NV00");
            func.SetValue("PSMNG", 2500);
            func.SetValue("GSTRP", DateTime.Now);
            func.SetValue("KDAUF", "0000263074");
            func.SetValue("KDPOS", "000010");

            //call function
            func.Invoke(dest);

            string result = func.GetString("AUFNR") + func.GetString("ZRESULT") + func.GetString("ERMSG");
            log.Debug(result);

            return result;
        }

        public async Task<String> ZMM_RFC_GOODSMVT_CREATE(RfcRepository repository, RfcDestination dest)
        {
            IRfcFunction func = repository.CreateFunction("ZMM_RFC_GOODSMVT_CREATE");

            //set parm
            func.SetValue("MATNR", "17WHP03058A007");
            func.SetValue("WERKS", "1016");
            func.SetValue("LGORT_S", "2100");
            func.SetValue("LGORT_T", "5340");
            func.SetValue("GSTRP", "20190521");
            func.SetValue("BWART", "311");
            func.SetValue("SGTXT", "IT TEST");
            func.SetValue("BRGEW", "10");
            func.SetValue("MEINS", "KPC");
            func.SetValue("CHARG", "0000013755");
            //func.SetValue("EBELN", "4500000068");
            //func.SetValue("EBELP", "00010");
            //func.SetValue("MVT_IND", "B");
            func.SetValue("CODE", "04");            


            //call function
            func.Invoke(dest);

            string result = func.GetString("ZRESULT") + func.GetString("ZERMSG") + func.GetString("MBLNR");
            log.Debug(result);

            return result;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            RfcDestination dest = GetDestination();
            RfcRepository repository = dest.Repository;
            for (int i = 0; i < 1; i++)
            {
                log.Debug(i);
                //var result = ZPP_RFC_PRODORD_CREATE(repository,dest);
                var result = ZMM_RFC_GOODSMVT_CREATE(repository, dest);
                result.Wait();
                log.Debug(i);
                textBox1.Text += result.Result;
            }
      
            textBox1.Text +=  "done";
            log.Debug("done");

        }
    }
}
