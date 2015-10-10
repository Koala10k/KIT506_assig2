using RAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAP.Controllers
{
    class PublicationsController : BaseController
    {
        public PublicationsController()
            : base()
        {

        }

        public Publication GetDetail(Publication publication)
        {
            return publication;// ?? new Publication();
        }
    }
}
