using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ModularMVC.ExamplePlugin.Controllers;
[Route("Example")]
public class ExampleController : Controller
{
    [Route("Index")]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// This action is used to check whether the views are being generated from the plugin.
    /// </summary>
    /// <returns></returns>
    [Route("Types")]
    public IActionResult Types()
    {
        return Ok(GetType().Assembly.GetTypes().Select(x => x.FullName).ToArray());
    }
}
