using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using SearchFrontendFhir.Models;

namespace SearchFrontendFhir.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }

        public async Task<IActionResult> Results(string search)
        {
            var FhirPath = "http://sqlonfhir-stu3.azurewebsites.net/fhir";
            if (string.IsNullOrEmpty(search))
            {
                return View();
            }

            var fhirclient = new Hl7.Fhir.Rest.FhirClient(FhirPath);
            //fhirclient.OnBeforeRequest += (sender, args) =>
            //{
            //    args.RawRequest.Headers.Add(HttpRequestHeader.Authorization,
            //        $"Bearer {_tokenManagerService.GetAccessToken()}");
            //};

            var resultBundle = await fhirclient.SearchAsync<Practitioner>(
                new[] { $"name={search}", "active=true" }
            );
            var ListofPractitioners = new List<PractitionerModel>();
            var practitioners = resultBundle.Entry.ByResourceType<Practitioner>();
            foreach (var prac in practitioners)
            {
                var ac = new PractitionerModel();

                var name = prac.Name.FirstOrDefault();

                if (name?.Given?.Any() ?? false)
                {
                    ac.Given += string.Join(" ", name.Given);
                }

                if (name?.Family?.Any() ?? false)
                {
                    ac.Surname += string.Join(" ", name.Family);
                }

                ListofPractitioners.Add(ac);
            }


            return View(ListofPractitioners);
        }

        
    }
}