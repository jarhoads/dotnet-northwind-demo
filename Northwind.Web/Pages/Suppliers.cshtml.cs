using Microsoft.AspNetCore.Mvc.RazorPages; // PageModel
using Northwind.Shared; // NorthwindContext
using Microsoft.AspNetCore.Mvc; // [BindProperty], IActionResult

namespace Northwind.Web.Pages;

public class SuppliersModel : PageModel
{


    //public IEnumerable<string>? Suppliers { get; set; }
    public IEnumerable<Supplier>? Suppliers { get; set; }
    
    private NorthwindContext db;
    public SuppliersModel(NorthwindContext injectedContext)
    {
        db = injectedContext;
    }

    public void OnGet()
    {
        ViewData["Title"] = "Northwind B2B - Suppliers";
        Suppliers = db.Suppliers.OrderBy(c => c.Country)
                                .ThenBy(c => c.CompanyName);
        //Suppliers = new[]
        //{
        //    "Alpha Co", "Beta Limited", "Gamma Corp"
        //};
    }

    [BindProperty]
    public Supplier? Supplier { get; set; }

    public IActionResult OnPost()
    {
        if ((Supplier is not null) && ModelState.IsValid)
        {
            db.Suppliers.Add(Supplier);
            db.SaveChanges();
            return RedirectToPage("/suppliers");
        }
        else
        {
            return Page(); // return to original page
        }
    }
}
