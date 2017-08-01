using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Core.Common
{
    public class QMDebug
    {
        public static int columnDisplayCnt = 25;
        public static void DebugTable(DataTable table)
        {

            Debug.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");

            String str = MakeRightBlankString_Bytes("ROWNUM", 7);
            Debug.Write(str);

            foreach (DataColumn column in table.Columns)
            {
                str = column.ToString();

                if (str.IndexOf("Column") == 0)
                {
                    String sIndex = str.Replace("Column", "");
                    int index = int.Parse(sIndex) - 1;
                    str = "Column" + index.ToString();

                }
                str = MakeRightBlankString_Bytes(str, columnDisplayCnt);
                Debug.Write(str);
            }
            Debug.WriteLine("");
            Debug.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");

            uint i = 0;
            foreach (DataRow row in table.Rows)
            {
                str = MakeRightBlankString_Bytes(i.ToString(), 7);
                i++;
                Debug.Write(str);
                foreach (Object column in row.ItemArray)
                {
                    str = MakeRightBlankString_Bytes(column.ToString().Replace("\t", ""), columnDisplayCnt);

                    Debug.Write(str);
                }
                Debug.WriteLine("");
            }

            return;
        }

        private static String MakeRightBlankString_Bytes(String str, int wishedBytes)
        {
            String result = "";

            int f = 0;
            char[] a = str.ToCharArray();

            int byteCount = 0;
            for (int i = 0; i < a.Length; i++)
            {
                int charBytes = 0;
                f = (int)a[i];

                if (byteCount >= wishedBytes)
                    break;


                if ((0x0000ff00 & f) == 0)
                {
                    charBytes = 1;
                }
                else
                {
                    charBytes = 2;
                }

                if (byteCount + charBytes > wishedBytes)
                {
                    charBytes--;
                }

                if (charBytes == 1)
                {
                    f = f & 0x000000ff;
                    result += Char.ConvertFromUtf32(f);
                }
                else if (charBytes == 2)
                {
                    result += a[i];
                }

                byteCount += charBytes;

            }
            if (wishedBytes - byteCount > 0)
            {
                for (int u = 0; u < wishedBytes - byteCount; u++)
                    result += " ";
            }


            return result;
        }
    }


}
