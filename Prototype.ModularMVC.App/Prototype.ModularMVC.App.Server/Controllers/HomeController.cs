using Microsoft.AspNetCore.Mvc;
using Prototype.ModularMVC.App.Server.Models;
using Prototype.ModularMVC.Hooks;
using Prototype.ModularMVC.Hooks.Args;
using Prototype.ModularMVC.PluginBase;
using System.Diagnostics;

namespace Prototype.ModularMVC.App.Server.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IServiceProvider _serviceProvider;

    public HomeController(ILogger<HomeController> logger, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Types()
    {
        return Ok(GetType().Assembly.GetTypes().Select(x => x.FullName).ToArray());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
