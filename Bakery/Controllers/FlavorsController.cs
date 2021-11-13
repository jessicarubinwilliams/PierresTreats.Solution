using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Bakery.Models;

namespace Bakery.Controllers
{
  public class FlavorsController : Controller
  {
    private readonly BakeryContext _db;

    public FlavorsController(BakeryContext db)
    {
      _db = db;
    }

    public ActionResult Index()
    {
      List<Flavor> model = _db.Flavors.ToList();
      return View(model);
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public ActionResult Create(Flavor flavor)
    {
      ViewBag.ErrorMessage = "";
      bool isUnique = true;
      List<Flavor> flavorList = _db.Flavors.ToList();
      foreach(Flavor iteration in flavorList)
      {
        if (flavor.FlavorName == iteration.FlavorName) 
        {
          isUnique = false;
          ModelState.AddModelError("DuplicateName", iteration.FlavorName + " already exists");
          return View();
        }
      }
      if (isUnique)
      {
        _db.Flavors.Add(flavor);
        _db.SaveChanges();
      }
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisFlavor = _db.Flavors
        .Include(flavor => flavor.FlavorTreatJoinEntity)
        .ThenInclude(join => join.Treat)
        .FirstOrDefault(flavor => flavor.FlavorId == id);
      return View(thisFlavor);
    }

    public ActionResult Edit(int id)
    {
      var thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == id);
      return View(thisFlavor);
    }

    [HttpPost]
    public ActionResult Edit(Flavor flavor)
    {
      _db.Entry(flavor).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == id);
      return View(thisFlavor);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == id);
      _db.Flavors.Remove(thisFlavor);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddTreat(int id)
    {
      Flavor thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == id);
      ViewBag.TreatId = new SelectList(_db.Treats, "TreatId", "TreatName");
      return View(thisFlavor);
    }

    [HttpPost]
    public ActionResult AddTreat(Flavor flavor, int TreatId)
    {
      if (TreatId != 0)
      {
        ViewBag.ErrorMessage = "";
        bool isUnique = true;
        List<FlavorTreat> flavorTreatList = _db.FlavorTreat.ToList();
        foreach(FlavorTreat iteration in flavorTreatList)
        {
          if (flavor.FlavorId == iteration.FlavorId && TreatId == iteration.TreatId) 
          {
            isUnique = false;
            Treat thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == TreatId);
            ModelState.AddModelError("DuplicateTreat", thisTreat.TreatName + " is already licensed to repair this flavor.");
            Flavor thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == TreatId);
            ViewBag.TreatId = new SelectList(_db.Treats, "TreatId", "TreatName");
            return View(thisFlavor);
          }
        }
        if (isUnique)
        {
          _db.FlavorTreat.Add(new FlavorTreat() { TreatId = TreatId, FlavorId = flavor.FlavorId });
        }
        _db.SaveChanges();
      }
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteTreat(int joinId)
    {
        var joinEntry = _db.FlavorTreat.FirstOrDefault(entry => entry.FlavorTreatId == joinId);
        _db.FlavorTreat.Remove(joinEntry);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
  }
}