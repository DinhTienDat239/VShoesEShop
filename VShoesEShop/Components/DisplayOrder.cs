using Microsoft.AspNetCore.Mvc;
using VShoesEShop.Infrastructure;
using VShoesEShop.Models;

namespace VShoesEShop.Components
{
	public class DisplayOrder:ViewComponent
	{
        public IViewComponentResult Invoke()
        {
            return View(HttpContext.Session.GetJson<Cart>("cart"));
        }
    }
}
