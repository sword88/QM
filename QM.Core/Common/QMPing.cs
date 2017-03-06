using System;
using System.Text;
using System.Net.NetworkInformation;

namespace QM.Core.Common
{
    public class QMPing : IDisposable
    {
        private bool m_disposed;
        private Ping m_ping;
        private PingOptions m_pingOpt;
        private PingReply m_pingRep;
        private byte[] m_buf;
        public QMPing()
        {
            this.m_ping = new Ping();
            this.m_pingOpt = new PingOptions();
            this.m_pingOpt.DontFragment = true;
            this.m_buf = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        }

        ~QMPing()
        {
            this.myDispose(false);
        }
        protected void myDispose(bool disposing)
        {
            if (!this.m_disposed && disposing)
            {
                if (this.m_ping != null)
                {
                    this.m_ping.Dispose();
                    this.m_ping = null;
                }

                this.m_pingOpt = null;
                this.m_pingRep = null;
            }
            this.m_disposed = true;
        }

        /// <summary>
        /// disponse action
        /// </summary>
        public void Dispose()
        {
            this.myDispose(true);
        }

        /// <summary>
        /// ping发送
        /// </summary>
        /// <param name="address">ipaddress</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool send(string address, int timeout)
        {
            try
            {
                this.m_pingRep = this.m_ping.Send(address, timeout, this.m_buf, this.m_pingOpt);
                bool result;
                if (this.m_pingRep.Status == IPStatus.Success)
                {
                    result = true;
                    return result;
                }
                result = false;
                return result;
            }
            catch (QMException fex)
            {
                throw fex;
            }
            return false;
        }
    }
}
