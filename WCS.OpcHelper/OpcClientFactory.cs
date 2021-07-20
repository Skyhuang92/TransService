using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS.Entity;

namespace WCS.OpcHelper
{
    public class OpcClientFactory
    {
        public static IOpcHelper Create()
        {
            IOpcHelper opcHelper = null;
            switch (McConfig.Instance.OpcType)
            {
                case "UA":
                    opcHelper = new OpcUaHelper();
                    break;
                case "DA":
                    opcHelper = new OpcDaHelper();
                    break;
                default:
                    break;
            }
            return opcHelper;
        }
    }
}
