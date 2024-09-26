using SampleMVCTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SampleMVCTest.Controllers
{
    public class ItemsController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            Item item = new Item();
            item.Id = id;
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            Item item = new Item();
            item.Id = id;
            return View(item);
        }
    }
}