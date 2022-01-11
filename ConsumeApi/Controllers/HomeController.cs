using ConsumeApi.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsumeApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response= await client.GetAsync("http://localhost:5000/api/products");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var result= JsonConvert.DeserializeObject<List<ProductResponseModel>>(jsonData);
                return View(result);

            }
            else
            {
                ViewBag.ResponseMessage = "Başarısız";
            }
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductResponseModel productResponseModel)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(productResponseModel);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            var response= await client.PostAsync("http://localhost:5000/api/products",content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["errorMessage"] = $"Bir hata ile karşılaştık. Hata kodu {(int)response.StatusCode}";
                return View(productResponseModel);
            }

            
        }

        public async Task<IActionResult> Update(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"http://localhost:5000/api/products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ProductResponseModel>(jsonData);
                return View(data);
            }
            else
            {
                return View(null);
            }

            
        }
        [HttpPost]
        public async Task<IActionResult> Update(ProductResponseModel productResponseModel)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(productResponseModel);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("http://localhost:5000/api/products", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["errorMessage"] = $"Bir hata ile karşılaştık. Hata kodu {(int)response.StatusCode}";
                return View(productResponseModel);
            }
            return View();
        }
        public async Task<IActionResult> Remove(int id)
        {
            var client = _httpClientFactory.CreateClient();
            await client.DeleteAsync($"http://localhost:5000/api/products/{id}");
            return RedirectToAction("Index");
        }
        public IActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile formFile)
        {
            var client = _httpClientFactory.CreateClient();

            var stream = new MemoryStream();
            await formFile.CopyToAsync(stream);
            var bytes = stream.ToArray();

            ByteArrayContent content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(formFile.ContentType);
            MultipartFormDataContent formData =new MultipartFormDataContent();
            formData.Add(content,"formFile",formFile.FileName);

            await client.PostAsync("http://localhost:5000/api/products/upload",formData);
            return RedirectToAction("Index");
        }
    }
}
