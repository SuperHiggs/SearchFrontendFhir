using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SearchFrontendFhir.Models;

namespace SearchFrontendFhir.Controllers
{
    [Route("api")]
    [ApiController]
    public class AutoCompleteController : ControllerBase
    {

        [HttpGet]
        [Route("AutoComplete")]
        public async Task<List<AutoCompleteModel>> AutoCompleteAsync([FromQuery] AutoCompleteInputModel model)
        {
            var FhirPath = "http://sqlonfhir-stu3.azurewebsites.net/fhir";
            if (string.IsNullOrEmpty(model.Value))
            {
                return new List<AutoCompleteModel>();
            }

            var fhirclient = new Hl7.Fhir.Rest.FhirClient(FhirPath);
            //fhirclient.OnBeforeRequest += (sender, args) =>
            //{
            //    args.RawRequest.Headers.Add(HttpRequestHeader.Authorization,
            //        $"Bearer {_tokenManagerService.GetAccessToken()}");
            //};

            var resultBundle = await fhirclient.SearchAsync<Practitioner>(
                new[] { $"name={model.Value}", "active=true" },null,10
            );

            var results = new List<AutoCompleteModel>();

            var practitioners = resultBundle.Entry.ByResourceType<Practitioner>();
            foreach (var prac in practitioners)
            {
                var ac = new AutoCompleteModel();

                var name = prac.Name.FirstOrDefault();

                if (name?.Given?.Any() ?? false)
                {
                    ac.Name += string.Join(" ", name.Given);
                }

                ac.Name += " ";

                if (name?.Family?.Any() ?? false)
                {
                    ac.Name += string.Join(" ", name.Family);
                }

                results.Add(ac);
            }

            //resultBundle = await fhirclient.SearchAsync<Location>(
            //    new[] { $"name={model.Value}", "active=true" }
            //);

            //var locations = resultBundle.Entry.ByResourceType<Location>();
            //foreach (var loc in locations)
            //{
            //    var ac = new AutoCompleteModel
            //    {
            //        Name = loc.Name
            //    };
            //    results.Add(ac);
            //}

            //resultBundle = await fhirclient.SearchAsync<Location>(
            //    new[] { $"address-city={model.Value}", "active=true" }
            //);

            //locations = resultBundle.Entry.ByResourceType<Location>();
            //foreach (var loc in locations)
            //{
            //    var ac = new AutoCompleteModel
            //    {
            //        Name = loc.Name
            //    };
            //    results.Add(ac);
            //}

            //resultBundle = await fhirclient.SearchAsync<Location>(
            //    new[] { $"address-postalcode={model.Value}", "active=true" }
            //);

            //locations = resultBundle.Entry.ByResourceType<Location>();
            //foreach (var loc in locations)
            //{
            //    var ac = new AutoCompleteModel
            //    {
            //        Name = loc.Name
            //    };
            //    results.Add(ac);
            //}


            return results;

        }

    }
}