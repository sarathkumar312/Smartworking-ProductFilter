using Newtonsoft.Json;
using ProductFilter.Entities;
using ProductFilter.Interfaces;
using System.Text.RegularExpressions;

namespace ProductFilter.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private const string DataUrl = "https://run.mocky.io/v3/cc147902-4a5a-4b1a-bc00-2220bafb49fd";
        private readonly ILogger<ProductService> _logger;

        public ProductService(HttpClient httpClient, ILogger<ProductService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<FilterResponse> GetFilteredProducts(decimal? minPrice, decimal? maxPrice, string? size, string[]? highlight)
        {
            var products = await FetchProducts();
            var allSizes = products.Select(p => p.Size).Distinct().ToList();
            var minPriceValue = products.Min(p => p.Price);
            var maxPriceValue = products.Max(p => p.Price);

            if (minPrice.HasValue) products = products.Where(p => p.Price >= minPrice.Value).ToList();
            if (maxPrice.HasValue) products = products.Where(p => p.Price <= maxPrice.Value).ToList();
            if (!string.IsNullOrWhiteSpace(size)) products = products.Where(p => p.Size.Equals(size, StringComparison.OrdinalIgnoreCase)).ToList();
            if (highlight != null) HighlightWords(products, highlight);

            var topWords = GetTopWords(products.Select(p => p.Description), 10);

            return new FilterResponse
            {
                Products = products,
                Filter = new FilterInfo
                {
                    MinPrice = minPriceValue,
                    MaxPrice = maxPriceValue,
                    Sizes = allSizes,
                    TopWords = topWords
                }
            };
        }

        private async Task<List<Product>> FetchProducts()
        {
            var response = await _httpClient.GetStringAsync(DataUrl);
            _logger.LogInformation("Fetched products: {Response}", response);
            return JsonConvert.DeserializeObject<List<Product>>(response);
        }

        private void HighlightWords(IEnumerable<Product> products, string[] words)
        {
            foreach (var product in products)
            {
                foreach (var word in words)
                {
                    product.Description = Regex.Replace(product.Description, $@"\b{Regex.Escape(word)}\b",
                        match => $"<em>{match.Value}</em>", RegexOptions.IgnoreCase);
                }
            }
        }

        private IEnumerable<string> GetTopWords(IEnumerable<string> descriptions, int count)
        {
            var stopWords = new HashSet<string> { "the", "is", "in", "and", "of" };
            var wordFrequency = descriptions
                .SelectMany(d => Regex.Split(d, @"\W+"))
                .Where(w => w.Length > 0 && !stopWords.Contains(w.ToLower()))
                .GroupBy(w => w.ToLower())
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Select(g => g.Key);

            return wordFrequency;
        }
    }
}
