using RAP.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAP.Controllers
{
    public class BaseController
    {
        protected ERDAdapter adapter;
        public BaseController()
        {
            adapter = ERDAdapter.getInstance();
        }
    }
}
