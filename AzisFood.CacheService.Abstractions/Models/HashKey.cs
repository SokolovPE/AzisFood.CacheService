using System;

namespace AzisFood.CacheService.Abstractions.Models
{
    /// <summary>
    /// Key of HashSet
    /// </summary>
    public class HashKey : Attribute
    {
        /// <summary>
        /// Key of HashSet
        /// </summary>
        public string Key { get; set; }
    }
}