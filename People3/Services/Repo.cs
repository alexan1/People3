using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using People3.Models;
using People3.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;

namespace People3.Services
{
    public class Repo : IRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public Repo(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        public async Task<int> AddAsync(Person item)
        {
            try
            {
                await _context.AddAsync(item);
                var res = await _context.SaveChangesAsync();
                return res;
            }
            catch
            {
                return 1;
            }
        }

        public async Task<PersonViewModel> DetailAsync(int key)
        {
            const string imageBase = "http://commons.wikimedia.org/wiki/Special:FilePath/";
            const string linkBase = "https://en.wikipedia.org/wiki/";
            const string urlBase = "https://www.wikidata.org/w/api.php?action=wbgetentities&ids=";
            const string urlOptions = "&format=json&languages=en&props=labels|descriptions|claims";
            var itemID = "Q" + key.ToString();
            var url = urlBase + itemID + urlOptions;
            var client = new HttpClient();
            var json = await client.GetStringAsync(url);
            var results = JObject.Parse(json);
            var entities = results["entities"]?[itemID];            
            var name = entities["labels"]["en"]["value"].ToString();

            var description1 = String.Empty;
            var description = entities["descriptions"];
            if (description != null)
            {
                description1 = description["en"]?["value"]?.ToString();
            }

            DateTime birthday1 = DateTime.MinValue;
            DateTime? birthday2 = null;
            var birthday = entities["claims"]["P569"];
            if (birthday != null)
            {
                var birth = birthday[0]?["mainsnak"]?["datavalue"]?["value"]?["time"];
                if (birth != null)
                {                    
                    birthday2 = DateTime.TryParse(birth.ToString().Substring(1), out birthday1) ? birthday1 : birthday2;
                }
            }

            DateTime death1 = DateTime.MinValue;
            DateTime? death2 = null;
            var death = entities["claims"]["P570"];
            if (death != null)
            {
                death2 = DateTime.TryParse(death[0]["mainsnak"]["datavalue"]["value"]["time"].ToString().Substring(1), out death1) ? death1 : DateTime.MinValue;
            }

            var link1 = entities["claims"]?["P373"]?[0]["mainsnak"]?["datavalue"]?["value"];
            var link = String.Empty;
            if (link1 != null)
            {
                link = System.Net.WebUtility.HtmlEncode(link1.ToString().Replace(" ", "_"));
            }
            var image1 = String.Empty;
            var imageLink = String.Empty;
            var image = entities["claims"]["P18"];
            if (image != null)
            {
                image1 = image[0]["mainsnak"]["datavalue"]["value"].ToString();
                imageLink = imageBase + image1;
            }

            var rating = await getRatingAsync(key);

            var person = new PersonViewModel()
            {
                ID = key,
                Name = name,
                Description = description1,
                Birthday = birthday2,
                Death = death2,
                Image = imageLink,
                Link = linkBase + link,
                Rating = rating
            };
            return person;
        }

        private async Task<decimal> getRatingAsync(int key)
        {
            //var person = await _context.FindAsync(typeof(Person), key);
            //var average = person
            //    .i
            //return ((Person)person).Rate.Average<Rating>;

            try
            {
                var RatingAverage = await _context.Rating.Where(r => r.PersonID == key)?.AverageAsync(r => r.Rate);
                var result = Convert.ToDecimal(RatingAverage);
                return result;
            }
            catch
            {
                return 0.0M;
            }
        }

        public async Task<IEnumerable<PersonViewModel>> FindAsync(string name)
        {            
            var urlBase = "https://query.wikidata.org/sparql";
            var query = "SELECT distinct (SAMPLE(?image)as ?image) ?item ?itemLabel ?itemDescription" +
                " (SAMPLE(?DR) as ?DR)(SAMPLE(?RIP) as ?RIP)(SAMPLE(?article) as ?article) " +
                "WHERE {?item wdt:P31 wd:Q5. ?item ?label '" + name + "'@en. OPTIONAL{?item wdt:P569 ?DR .}" +
                " ?article schema:about ?item . ?article schema:inLanguage 'en'. ?article schema:isPartOf <https://en.wikipedia.org/>. " +
                "OPTIONAL{?item wdt:P570 ?RIP .} " +
                "OPTIONAL{?item wdt:P18 ?image .} " +
                "SERVICE wikibase:label { bd:serviceParam wikibase:language 'en'. }} " +
                "GROUP BY ?item ?itemLabel ?itemDescription";
            var url = urlBase + "?query=" + query + "&format=json";
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36");
            var json = await client.GetStringAsync(url);
            //var jsonStr = json.ToString();
            var results = JObject.Parse(json);
            var entities = results["results"]["bindings"];
            //var count = entities.Count();
            var FoundPersons = new List<PersonViewModel>();
            foreach (JToken item in entities)
            {

                int item1;
                item1 = int.TryParse(item["item"]["value"].ToString().Substring(32), out item1) ? item1 : 0; 
                var itemname = item["itemLabel"]["value"].ToString();

                var description1 = String.Empty;
                var description = item["itemDescription"];
                if (description != null)
                {
                    description1 = description["value"].ToString();
                }

                DateTime birthday1 = DateTime.MinValue;
                DateTime? birthday2 = null;
                var birthday = item["DR"];
                if (birthday != null)
                {
                    birthday2 = DateTime.TryParse(birthday["value"].ToString(), out birthday1) ? birthday1 : birthday2;
                }

                DateTime death1 = DateTime.MinValue;
                DateTime? death2 = null;
                var death = item["RIP"];
                if (death != null)
                {
                    death2 = DateTime.TryParse(death["value"].ToString(), out death1) ? death1 : death2; 
                }

                var image1 = string.Empty;
                var image = item["image"];
                if (image != null)
                {
                    image1 = image["value"].ToString();
                }

                var link = item["article"]["value"].ToString();
                var rating = 10;

                var person = new PersonViewModel
                {
                    ID = item1,
                    Name = itemname,
                    Description = description1,
                    Birthday = birthday2,
                    Death = death2,
                    Image = image1,
                    Link = link,
                    Rating = rating
                };
                FoundPersons.Add(person);
            }            
            
            return FoundPersons;
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _context.Person.Include(person => person.Rate).ToListAsync();
        }

        public async Task<int> RateAsync(Person item, string userID, int rate)
        {                   
            var rating = new Rating {
                Rate = rate,
                UserID = userID,
                PersonID = item.ID
            };
            try
            {
                var res1 = await _context.Rating.AddAsync(rating);
                var res = await _context.SaveChangesAsync();
                return res;
            }
            catch (DbUpdateException)
            {
                var res1 = _context.Rating.Update(rating);
                var res = await _context.SaveChangesAsync();
                return res;
            }
            catch
            {
                return -1;
            }
        }       
    }
}
