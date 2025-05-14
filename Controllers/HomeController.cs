using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Agri_Energy.Models;

namespace Agri_Energy.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    //Load Contact index (Microsoft, 2025)
    public IActionResult Index()
    {
        return View();
    }

    //Load About page (Microsoft, 2025)
    public IActionResult About()
    {
        return View();
    }

    //Load Contact page (Microsoft, 2025)
    public IActionResult Contact()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}