using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelBookingSystem.Controllers
{
    public class HotelViewController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public HotelViewController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // GET: HotelView
        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7256/api/Hotels");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var hotels = JsonConvert.DeserializeObject<IEnumerable<Hotel>>(data);
                return View(hotels);
            }
            return View(new List<Hotel>());
        }

        // GET: HotelView/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HotelView/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                var client = _clientFactory.CreateClient();
                var content = new StringContent(JsonConvert.SerializeObject(hotel), System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:7256/api/Hotels", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "服务器错误，请联系管理员。");
            }
            return View(hotel);
        }

        // 其他操作（编辑、删除等）类似
    }
}
