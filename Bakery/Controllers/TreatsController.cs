using Bakery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Bakery.Controllers
{
  [Authorize]
  public class TreatsController : Controller
  {
    private readonly BakeryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TreatsController(UserManager<ApplicationUser> userManager, BakeryContext db)
    {
       _userManager = userManager;
      _db = db;
    }

    [AllowAnonymous]
    public ActionResult Index()
    {
      return View(_db.Treats.ToList());
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public ActionResult Create(Treat treat)
    {
      ViewBag.ErrorMessage = "";
      bool isUnique = true;
      List<Treat> treatList = _db.Treats.ToList();
      foreach(Treat iteration in treatList)
      {
        if (treat.TreatName == iteration.TreatName) 
        {
          isUnique = false;
          ModelState.AddModelError("DuplicateTreat", iteration.TreatName + " already exists");
          return View();
        }
      }
      if (isUnique)
      {
        _db.Treats.Add(treat);
        _db.SaveChanges();
      }
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      Treat thisTreat = _db.Treats
          .Include(treat => treat.FlavorTreatJoinEntity)
          .ThenInclude(join => join.Flavor)
          .FirstOrDefault(treat => treat.TreatId == id);
      return View(thisTreat);
    }

    public ActionResult Edit(int id)
    {
      Treat thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "FlavorName");
      return View(thisTreat);
    }

    [HttpPost]
    public ActionResult Edit(Treat treat, int FlavorId)
    {
      if (FlavorId != 0)
      {
        _db.FlavorTreat.Add(new FlavorTreat() { FlavorId = FlavorId, TreatId = treat.TreatId });
      }
      _db.Entry(treat).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddFlavor(int id)
    {
      Treat thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "FlavorName");
      return View(thisTreat);
    }

    [HttpPost]
    public ActionResult AddFlavor(Treat treat, int FlavorId)
    {
      if (FlavorId != 0)
      {
        ViewBag.ErrorMessage = "";
        bool isUnique = true;
        List<FlavorTreat> flavorTreatList = _db.FlavorTreat.ToList();
        foreach(FlavorTreat iteration in flavorTreatList)
        {
          if (treat.TreatId == iteration.TreatId && FlavorId == iteration.FlavorId) 
          {
            isUnique = false;
            Flavor thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == FlavorId);
            ModelState.AddModelError("DuplicateFlavor", "This treat is already licensed to repair " + thisFlavor.FlavorName);
            Treat thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == FlavorId);
            ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "FlavorName");
            return View(thisTreat);
          }
        }
        if (isUnique)
        {
          _db.FlavorTreat.Add(new FlavorTreat() { FlavorId = FlavorId, TreatId = treat.TreatId });
        }
        _db.SaveChanges();
      }
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      Treat thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      return View(thisTreat);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      Treat thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      _db.Treats.Remove(thisTreat);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteFlavor(int joinId)
    {
      FlavorTreat joinEntry = _db.FlavorTreat.FirstOrDefault(entry => entry.FlavorTreatId == joinId);
      _db.FlavorTreat.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}