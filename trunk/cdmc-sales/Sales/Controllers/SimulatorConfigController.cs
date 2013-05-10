using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Entity;
using Telerik.Web.Mvc;
using Utl;

namespace Sales.Controllers
{
    /// <summary>
    /// Simulator controller used to implement simulator configuration function.
    /// </summary>
    [AdministratorRequired]
    public class SimulatorConfigController : Controller
    {
        public ViewResult index()
        {
            return View();
        }

        [GridAction]
        public ActionResult _SelectIndex()
        {
            string loginUserName = Utl.Employee.GetLoginUserName();
            SimulatorConfig simulatorConfig =
                    CH.GetAllData<SimulatorConfig>(s => s.AdminName == loginUserName).SingleOrDefault();
            if (simulatorConfig == null)
            {
                simulatorConfig = new SimulatorConfig();
                simulatorConfig.SimulatorName = loginUserName;
            }
            
            List<SimulatorConfig> gridList = new List<SimulatorConfig>();
            gridList.Add(simulatorConfig);
            return View(new GridModel(gridList));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ViewResult _SaveAjaxEditing(int id)
        {
            var item = CH.GetDataById<SimulatorConfig>(id);
            if (item == null)
            {
                item = new SimulatorConfig();
                item.AdminName = Utl.Employee.GetLoginUserName();
                TryUpdateModel(item);
                CH.Create<SimulatorConfig>(item);
            }
            else
            {
                TryUpdateModel(item);
                CH.Edit<SimulatorConfig>(item);
            }
            return View(new GridModel(CH.GetAllData<SimulatorConfig>()));
        }

        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }
    }
}