using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public IFormsAuthenticationService FormsService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }

            base.Initialize(requestContext);
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Search(string p)
        {
            if(p.Length < 2 || p == string.Empty)
            {
                TempData["Error"] = "Please try again";
                return View("Index");
            }

            ProductModel model = new ProductModel();
            List<ProductModel> products = model.GetProductsByName(p);
            if(products.Count < 1)
            {
                TempData["Error"] = "Could not find " + p + ".";
                return View("Index");
            }
            ViewBag.Products = products;

            return View("Index");
        }

        [HttpPost]
        public ActionResult LogIn()
        {
            UserModel user = new UserModel();

            if(user.Login(Request["txtUser"].ToString(), Request["txtPass"].ToString()))
            {
                FormsService.SignIn(Request["txtUser"].ToString(), true);
                return RedirectToAction("Panel");
            }

            TempData["Error"] = "Failed to login!";

            return View("Index");
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            FormsService.SignOut();

            return RedirectToAction("Index");
        }

        public ActionResult Panel()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }

            ProductModel model = new ProductModel();
            ViewBag.Products = model.GetAllProducts();

            return View();
        }

        public ActionResult Product(string id)
        {
            ProductModel model = new ProductModel();
            ViewBag.Product = model.GetProductByName(id);
            if (ViewBag.Product == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddProduct()
        {
            string productName = Request["txtPrdName"];
            string productDesc = Request["txtPrdDesc"];
            string ProductImage = Request["txtPrdImage"];

            if(productDesc.Length < 0 || productName.Length < 0 || ProductImage.Length < 0)
            {
                TempData["Error"] = "Please check all fields.";
                return View("Panel");
            }

            ProductModel model = new ProductModel();
            if(model.CheckIfProductNameExists(productName))
            {
                TempData["Error"] = "Name is already taken!";
                return View("Panel");
            }
            if(!model.AddNewProduct(productName, productDesc, ProductImage))
            {
                TempData["Error"] = "Something is wrong please try later.";
                return View("Panel");
            }

            TempData["Error"] = "Product has been added!";

            return RedirectToAction("Panel");
        }

        public ActionResult Edit(string id)
        {
            if(!Request.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }

            ProductModel product = new ProductModel();
            ViewBag.Product = product.GetProductByName(id);

            return View();
        }

        public ActionResult DeleteProduct(string id)
        {
            ProductModel model = new ProductModel();
            model.DeleteProductByName(id);

            TempData["notify"] = "Product '" + id + "' has been delete!";

            return RedirectToAction("Panel");
        }

        [HttpPost]
        public ActionResult SaveProduct()
        {
            string name = Request["txtName"];
            string desc = Request["txtDesc"];
            string image = Request["txtImage"];
            string originalName = Request["hiddenName"];

            ProductModel model = new ProductModel();
            model.UpdateProduct(originalName, name, desc, image);

            TempData["notify"] = "Product has beed saved!";

            return RedirectToAction("Panel");
        }

        public ActionResult DeleteImage(string id)
        {
            ProductModel model = new ProductModel();
            model.DeleteImage(id);

            List<string> t = new List<string>();

            return RedirectToAction("Panel");
        }

    }
}
