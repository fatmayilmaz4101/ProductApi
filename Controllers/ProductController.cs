using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace ProductApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string barkod)
        {
            var url = $"https://www.a101.com.tr/arama?k={barkod}";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Mobile Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Referer", "https://www.a101.com.tr/");
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

            string html;
            try
            {
                html = await httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException e)
            {
                return BadRequest($"HTTP Hatası: {e.Message}");
            }

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var productName = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'product-container')]//h3");
            var productPrice = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'product-container')]//section//span[contains(@class, 'text-base')]");

            if (productName is not null && productPrice is not null)
            {
                var product = new
                {
                    Name = productName.InnerText.Trim(),
                    Price = productPrice.InnerText.Trim()
                };

                return Ok(product);
            }
            else
            {
                return Ok("Ürün bulunamadı.");
            }
        }
    }
}
