using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using SoapToRest.Areas.HelpPage.ModelDescriptions;
using SoapToRest.Areas.HelpPage.Models;
using System.Collections.ObjectModel;
using System.Web.Http.Description;

namespace SoapToRest.Areas.HelpPage.Controllers
{
	public partial class HelpController
	{
        public ActionResult Wadl(string controllerDescriptor)
        {
            var apiDescriptions = Configuration.Services.GetApiExplorer().ApiDescriptions;
            var apid = new Collection<ApiDescription>(apiDescriptions.Where(a => !String.Equals(a.Route.Defaults["controller"] as string, "ManageMap")).ToList());
            var apisWithHelp = apid.Select(api => Configuration.GetHelpPageApiModel(api.GetFriendlyId()));

            return View(apisWithHelp);
        }

	}
}