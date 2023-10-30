using Microsoft.AspNetCore.Mvc;
using VShoesEShop.Data;
using VShoesEShop.Infrastructure;
using VShoesEShop.Models;

namespace VShoesEShop.Components
{
    public class CartWidget:ViewComponent
    {
        
        public IViewComponentResult Invoke()
        {
            return View(HttpContext.Session.GetJson<Cart>("cart"));
        }
    }
}
