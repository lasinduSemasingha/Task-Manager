using Microsoft.AspNetCore.Mvc;

namespace Task_Manager.Controllers;
public class ErrorController : Controller
{
    [Route("/error")]
    public IActionResult Error()
    {
        return View("Error");
    }

    [Route("/error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        switch (statusCode)
        {
            case 404:
                ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found.";
                break;
            default:
                ViewBag.ErrorMessage = "Sorry, something went wrong.";
                break;
        }
        return View("NotFound");
    }
}
