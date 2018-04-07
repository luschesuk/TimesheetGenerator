using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace TimesheetGeneratorAspNetCoreRazorPages.Pages
{
    public class IndexModel : PageModel
    {
        /// <summary>
        /// This method is used for GET http requests.
        /// </summary>
        public void OnGet()
        {
            // Set default form field values.
            CandidateName = string.Empty;
            ClientName = string.Empty;
            JobTitle = string.Empty;
            PlacementStartDate = DateTime.Today;
            PlacementEndDate = PlacementStartDate.AddDays(6);
            PlacementType = "Weekly";

            // Set default validation values.
            Validated = false;
            PlacementStartDateErrorMessage = string.Empty;
            PlacementEndDateErrorMessage = string.Empty;
        }

        /// <summary>
        /// This method is used for http POST requests when button [Generate] is pressed.
        /// </summary>
        public void OnPostGenerate()
        {
            if (ModelState.IsValid)
            {
                // Trim data type text input values.
                CandidateName = CandidateName.Trim();
                ClientName = ClientName.Trim();
                JobTitle = JobTitle.Trim();

                // Validate data type date values here on server side, because client side validation only validates empty fields.
                DateTime startDate;
                if (DateTime.TryParseExact(PlacementStartDate.ToShortDateString(), new string[] { "dd/MM/yyyy" },
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out startDate))
                {
                    DateTime endDate;
                    if (DateTime.TryParseExact(PlacementEndDate.ToShortDateString(), new string[] { "dd/MM/yyyy" },
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out endDate))
                    {
                        // Check if end date is greater than start date.
                        int cmp = PlacementEndDate.CompareTo(PlacementStartDate);
                        if (cmp > 0)
                        {
                            // Trim data type text values.
                            CandidateName = CandidateName.Trim();
                            ClientName = ClientName.Trim();
                            JobTitle = JobTitle.Trim();

                            // Set validation flag.
                            Validated = true;
                        }
                        else if (cmp < 0)
                        {
                            PlacementEndDateErrorMessage = "The Placement End Date is in the past.";
                            // Set validation flag.
                            Validated = false;
                        }
                    }
                    else
                    {
                        PlacementEndDateErrorMessage = "The Placement End Date is invalid. A valid format is dd/MM/yyyy";
                        // Set validation flag.
                        Validated = false;
                    }
                }
                else
                {
                    PlacementStartDateErrorMessage = "The Placement Start Date is invalid. A valid format is dd/MM/yyyy";
                    // Set validation flag.
                    Validated = false;
                }
            }
        }

        #region Model Properties

        [BindProperty, Required, DataType(DataType.Text), StringLength(50, MinimumLength = 3)]
        [Display(Name = "Candidate Name")]
        public string CandidateName { get; set; }

        [BindProperty, Required, DataType(DataType.Text), StringLength(50, MinimumLength = 3)]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [BindProperty, Required, DataType(DataType.Text), StringLength(50, MinimumLength = 3)]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [BindProperty, DataType(DataType.Date), StringLength(10, MinimumLength = 8)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Placement Start Date")]
        public DateTime PlacementStartDate { get; set; }

        [BindProperty, DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Placement End Date")]
        public DateTime PlacementEndDate { get; set; }

        [BindProperty, Required, DataType(DataType.Text)]
        [Display(Name = "Placement Type")]
        public string PlacementType { get; set; }

        // Validate date properties.
        public string PlacementStartDateErrorMessage { get; set; }
        public string PlacementEndDateErrorMessage { get; set; }

        #endregion

        public bool Validated { get; set; }

        #region Methods

        /// <summary>
        /// This method is used to get the header row for http POST requests when button [Create] is pressed.
        /// </summary>
        public string GetHeaderRow()
        {
            StringBuilder result = new StringBuilder("");

            if (Validated)
            {
                result.AppendLine(string.Format(
                    "Candidate Name: {0}<br />Client Name: {1}<br />Job Title: {2}<br />Placement Start Date: {3}<br />PlacementEnd Date: {4}<br />", CandidateName, ClientName, JobTitle, PlacementStartDate, PlacementEndDate));
            }
            return result.ToString();
        }

        /// <summary>
        /// This method is used to get the date range row(s) for http POST requests when button [Create] is pressed.
        /// </summary>
        public string GetDateRangeRow()
        {
            StringBuilder result = new StringBuilder("");

            if (Validated)
            {
                // Get range
                DateTime startDate;

                if (DateTime.TryParseExact(PlacementStartDate.ToShortDateString(), new string[] { "dd/MM/yyyy" },
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out startDate))
                {
                    string[] dates;

                    while (startDate <= PlacementEndDate)
                    {
                        dates = GetRange(PlacementType, startDate);
                        result.AppendLine(string.Format("{0} - {1}<br />", dates[0], dates[1]));
                        startDate = DateTime.Parse(dates[1]).AddDays(1);
                    }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// This method is called by GetDateRanges() to get a date range by frequency.
        /// </summary>
        public string[] GetRange(string frequency, DateTime startDate)
        {
            string[] result = new string[2];
            DateTime dateRangeBegin = startDate;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0); // One day
            DateTime dateRangeEnd = startDate.Add(duration);

            switch (frequency)
            {
                case "Weekly":
                    dateRangeBegin = startDate;
                    dateRangeEnd = startDate.AddDays(7 - (int)startDate.DayOfWeek);
                    break;

                case "Monthly":
                    duration = new TimeSpan(DateTime.DaysInMonth(startDate.Year, startDate.Month) -1, 0, 0, 0);
                    dateRangeBegin = startDate.AddDays((-1) * startDate.Day + 1);
                    dateRangeEnd = dateRangeBegin.Add(duration);
                    break;
            }

            // Adjust the start date
            if (dateRangeBegin < PlacementStartDate)
            {
                dateRangeBegin = PlacementStartDate;
            }

            // Adjust the end date
            if (dateRangeEnd > PlacementEndDate)
            {
                dateRangeEnd = PlacementEndDate;
            }

            result[0] = dateRangeBegin.ToShortDateString();
            result[1] = dateRangeEnd.ToShortDateString();
            return result;
        }

        #endregion

    }
}
