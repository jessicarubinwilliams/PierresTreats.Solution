using Bakery.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Bakery.Controllers
{
  public class HomeController : Controller
  {

    private readonly BakeryContext _db;

    public HomeController(BakeryContext db)
    {
      _db = db;
    }

    [HttpGet("/")]
    public ActionResult Index()
    {
      List<Flavor> FlavorList = _db.Flavors.OrderBy(x => x.FlavorName).ToList();
      List<Treat> TreatList = _db.Treats.OrderBy(x => x.TreatName).ToList();

      Dictionary <string, object> model = new Dictionary <string, object>() { {"Flavors", FlavorList}, {"Treats", TreatList} };
      
      return View(model);
    }
  }
}