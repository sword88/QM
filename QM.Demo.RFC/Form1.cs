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

namespace QM.Demo.RFC
{
    public partial class Form1 : Form
    {
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
            RfcConfigParameters configParams = GetConfigParams();
            RfcDestination dest = RfcDestinationManager.GetDestination(configParams);

            return dest;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RfcDestination dest = GetDestination();
            RfcRepository repository = dest.Repository;
            IRfcFunction func = repository.CreateFunction("ZPP_RFC_PRODORD_CREATE");

            //set parm
            func.SetValue("MATNR", "TI1018ESTRLPBF00");
            func.SetValue("PSMNG", 10);
            func.SetValue("GSTRP", DateTime.Now);
            func.SetValue("KDAUF", "0000001917");
            func.SetValue("KDPOS", "000010");
            //func.SetValue("MATNR", "T9882124400");
            //func.SetValue("PSMNG", 10);
            //func.SetValue("GSTRP", DateTime.Now);
            //func.SetValue("KDAUF", "0000073112");
            //func.SetValue("KDPOS", "000010");

            //call function
            func.Invoke(dest);

            //get result
            string po = func.GetString("AUFNR");
            string ok = func.GetString("ZRESULT");
            string msg = func.GetString("ERMSG");

            textBox1.Text =  po  + " \n " 
                          +  ok  + " \n " 
                          +  msg + " \n " 
                          +  textBox1.Text;            

        }
    }
}
