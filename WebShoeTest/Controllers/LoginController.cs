﻿using Microsoft.AspNetCore.Mvc;

namespace WebShoeTest.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
