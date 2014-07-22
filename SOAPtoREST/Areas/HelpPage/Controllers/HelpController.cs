using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Linq;
using SoapToRest.Areas.HelpPage.Models;
using System.Collections.ObjectModel;
using System.Web.Http.Description;

namespace SoapToRest.Areas.HelpPage.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public partial class HelpController : Controller
    {
        public HelpController()
            : this(GlobalConfiguration.Configuration)
        {
        }

        public HelpController(HttpConfiguration config)
        {
            Configuration = config;
        }

        public HttpConfiguration Configuration { get; private set; }

        public ActionResult Index()
        {
            ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();
            var apid = Configuration.Services.GetApiExplorer().ApiDescriptions.Where(a => !String.Equals(a.Route.Defaults["controller"] as string, "ManageMap")).ToList();
            return View(new Collection<ApiDescription>(apid));
        }

        public ActionResult Api(string apiId)
        {
            if (!String.IsNullOrEmpty(apiId))
            {
                HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
                if (apiModel != null)
                {
                    return View(apiModel);
                }
            }

            return View("Error");
        }
    }
}