using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmbientCollage.Models;
using AmbientCollage.Abstractions;

namespace AmbientCollage.Controllers
{
    public class AdminController : UserAwareController
    {
        DataAccessLayer dal = new DataAccessLayer(new MongoDataAccessLayer(new SimpleSecurity()));
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }
    }
}
