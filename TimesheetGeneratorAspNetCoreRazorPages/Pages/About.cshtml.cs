using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TimesheetGeneratorAspNetCoreRazorPages.Pages
{
    public class AboutModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            Message = "What is this application about?";
        }
    }
}
