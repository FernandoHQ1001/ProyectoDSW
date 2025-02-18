using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bodega.SolProyectoWeb.Entidades.Base
{
    public class BaseLN
    {
        protected log4net.ILog Log
        {
            get
            {
                return
                log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());
            }
        }
    }
}
