using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;

namespace InsideWordMVCWeb.Controllers
{
    // This is so dumb... please delete me when MVC3 fixes the optional routing bug.
    public partial class BugFixController : Controller
    {
        public BugFixController()
        {
            if (System.Web.HttpContext.Current.CurrentHandler is MvcHandler)
            {
	            RouteValueDictionary rvd = ((MvcHandler)System.Web.HttpContext.Current.CurrentHandler).RequestContext.RouteData.Values;
                string[] matchingKeys = rvd.Where(entry => entry.Value == BugFixUrlParameter.Optional)
                                           .Select(entry => entry.Key)
                                           .ToArray();

	            foreach (string key in matchingKeys)
	            {
	                rvd.Remove(key);
	            }
            }
        }
    }

    // This is so dumb... please delete me when MVC3 fixes the optional routing bug.
    public class BugFixUrlParameter
    {
        public static readonly BugFixUrlParameter Optional = new BugFixUrlParameter();

        private BugFixUrlParameter()
        {

        }
    }
}
