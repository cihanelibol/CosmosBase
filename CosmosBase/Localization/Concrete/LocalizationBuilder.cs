using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Resources;

namespace CosmosBase
{
    public class LocalizationBuilder : IStringLocalizer
    {
        private readonly IMemoryCache memoryCache;
        private readonly ResourceManager resourceManager;

        public LocalizationBuilder(IMemoryCache memoryCache, ResourceManager resourceManager)
        {
            this.memoryCache = memoryCache;
            this.resourceManager = resourceManager;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        private string GetString(string key)
        {
            var culture = CultureInfo.CurrentUICulture;
            var cacheKey = $"{key}_{culture.Name}";
            if (!memoryCache.TryGetValue(cacheKey, out string value))
            {
                value = resourceManager.GetString(key, culture);
                memoryCache.Set(cacheKey, value); 
            }
            return value;

        }
    }
}
