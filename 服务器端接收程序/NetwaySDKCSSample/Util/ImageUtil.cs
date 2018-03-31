using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace 服务器端接收程序.Util
{
    public class ImageUtil
    {
        public static bool SaveFormBytes(byte[] bytes, int len)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream("../../"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg", FileMode.Create, FileAccess.Write);
                fs.Write(bytes, 0, len);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }
    }
}
