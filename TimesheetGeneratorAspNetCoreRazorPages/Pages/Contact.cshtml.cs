using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TimesheetGeneratorAspNetCoreRazorPages.Pages
{
    public class ContactModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            Message = "How to get in touch?";
        }
    }
}
