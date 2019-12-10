using Allied.RealTime.Example.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Allied.RealTime.Example.Web.Controllers
{
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private static readonly Random random = new Random(DateTime.Now.Millisecond);
        private static readonly IList<string> names = new List<string>
        {
            "Liam", "Emma", "Noah", "Olivia", "William", "Ava", "James", "Isabella", "Oliver", "Sophia",
            "Benjamin", "Charlotte", "Elijah", "Mia", "Lucas", "Amelia", "Mason", "Harper", "Logan",
            "Evelyn", "Alexander", "Abigail", "Ethan", "Emily", "Jacob", "Elizabeth", "Michael", "Mila",
            "Daniel", "Ella", "Henry", "Avery", "Jackson", "Sofia", "Sebastian", "Camila", "Aiden", "Aria",
            "Matthew", "Scarlett", "Samuel", "Victoria", "David", "Madison", "Joseph", "Luna", "Carter",
            "Grace", "Owen", "Chloe",
        };
        private static readonly IList<string> streets = new List<string>
        {
            "Brookside Drive", "Cemetery Road", "Garden Street", "Woodland Road", "Windsor Court", "Mechanic Street",
            "Walnut Street", "Schoolhouse Lane", "Washington Avenue", "9th Street West", "Park Street", "Laurel Street",
            "1st Avenue", "Riverside Drive", "5th Street South", "Cypress Court", "Route 11", "Rosewood Drive", "5th Street West",
            "Elmwood Avenue", "Brookside Drive", "Belmont Avenue", "Arlington Avenue", "Green Street", "Middle Street",
            "Pennsylvania Avenue", "Rosewood Drive", "Virginia Street", "2nd Street North", "Parker Street", "4th Avenue",
            "South Street", "Fairview Avenue", "White Street", "Cambridge Court", "Forest Avenue", "Route 10", "Cemetery Road",
            "Riverside Drive", "Windsor Court", "New Street", "Rose Street", "Bayberry Drive", "Hilltop Road", "Broad Street West",
            "Woodland Road", "Forest Street", "Water Street", "Strawberry Lane",
        };
        private static readonly IList<string> cities = new List<string>
        {
            "Warner", "East Natchitoches", "Lyon", "Willow Run", "Conyersville", "Mount Baker", "Farmington Lake", "Martins Corner", "Pickerel Narrows",
            "Willaha", "Center", "Spring City", "Mittenlane", "East Waterford", "Coltman", "Scottsville", "Hebron", "Longview", "Emerson", "North Knoxville",
            "Momford Landing", "Ipswich", "Storms", "Kalauao", "Farwell", "Brentwood Village", "Wilhelm Park", "Bannister Acres", "Bent Pine", "Mitchell",
            "Social Circle", "Kreutzberg", "Cimarron", "Brookhaven", "Montverde Junction", "Midland City", "Sacramento", "Del Rey Oaks", "Coal Creek",
            "Rabbitown", "Fairland", "Gaskil", "Morgan Mill", "Merrimac South", "Stanardsville", "Two Brooks", "Curriers", "Skookumchuck", "Edgerton", "Slater",
        };
        private static readonly IList<string> states = new List<string>
        {
            "AK", "AL", "AR", "AZ", "CA", "CO", "CT", "DE", "FL", "GA",
            "HI", "IA", "ID", "IL", "IN", "KS", "KY", "LA", "MA", "MD",
            "ME", "MI", "MN", "MO", "MS", "MT", "NC", "ND", "NE", "NH",
            "NJ", "NM", "NV", "NY", "OH", "OK", "OR", "PA", "RI", "SC",
            "SD", "TN", "TX", "UT", "VA", "VT", "WA", "WI", "WV", "WY",
        };

        // GET: api/<controller>
        [HttpPost]
        public ActionResult<ProfileResponse> Profile([FromBody] ProfileRequest profileRequest)
        {
            // TODO: query the core for address info. Here, we'll just simulate
            // the address and name.

            ProfileResponse profileResponse = new ProfileResponse
            {
                success = true,
                firstName = names[random.Next(50)],
                lastName = names[random.Next(50)],
                address1 = $"{random.Next(100, 5000)} {streets[random.Next(50)]}",
                city = cities[random.Next(50)],
                state = states[random.Next(50)],
                zip = $"{random.Next(10000, 90000)}",
            };

            return profileResponse;
        }
    }
}