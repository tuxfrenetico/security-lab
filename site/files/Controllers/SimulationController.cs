using Microsoft.AspNetCore.Mvc;
using Site.Models;
using Site.Services;

namespace Site.Controllers
{
  public class SimulationController : BaseController
  {
    private SimulationService _service;

    public SimulationController(SimulationService service) => _service = service;

    [HttpGet]
    public IActionResult Index() => View(new SimulationModel());

    [HttpPost]
    public IActionResult Add(SimulationModel simulation)
    {

      if (ModelState.IsValid)
      {
        simulation.UserId = UserId;
        simulation.Id = _service.Add(simulation);
        return RedirectToAction("Details", new { id = simulation.Id });
      }
      else
        return View("Index", simulation);
    }

    [HttpPost]
    public IActionResult Approve(long id)
    {
      _service.Approve(id);
      return RedirectToAction("List");
    }

    [HttpGet]
    public IActionResult Details(long id) => View(_service.Get(id, UserId));

    [HttpGet]
    public IActionResult List()
    {
      if (IsApprover)
        return View(_service.GetAll());

      return View(_service.GetByUser(UserId));
    }
  }
}