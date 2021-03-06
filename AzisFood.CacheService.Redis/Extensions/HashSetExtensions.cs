using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AzisFood.CacheService.Abstractions.Models;
using MessagePack;
using StackExchange.Redis;

namespace AzisFood.CacheService.Redis.Extensions
{
    /// <summary>
    /// Extensions to convert entity to HashEntry and back
    /// </summary>
    public static class HashSetExtensions
    {
        private const string DefaultHashEntryKeyMember = "Id";
        
        /// <summary>
        /// Returns value of hash entry key
        /// </summary>
        /// <param name="entry">Entity entry</param>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <returns>Value of hash key</returns>
        /// <exception cref="ArgumentException">Entities without HashEntryKey attribute are not supported</exception>
        public static string GetHashEntryKey<T>(this T entry)
        {
            var typeMembers = typeof(T).GetMembers();
            var hashEntryKeyMember = typeMembers
                .FirstOrDefault(m => m.GetCustomAttributes(typeof(HashEntryKey), false).Any());

            if (hashEntryKeyMember != default) 
                return hashEntryKeyMember.GetValue(entry).ToString();
            
            // If HashEntryKeyMember not set - find id member
            hashEntryKeyMember = typeMembers.FirstOrDefault(m => m.Name == DefaultHashEntryKeyMember);
            if (hashEntryKeyMember == default)
            {
                throw new ArgumentException(
                    $"Entity {nameof(T)} must contain {nameof(HashEntryKey)} attribute " +
                    $"or <{DefaultHashEntryKeyMember}> member");
            }

            return hashEntryKeyMember.GetValue(entry).ToString();
        }

        /// <summary>
        /// Get key for entity HashSet
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <returns>Key for entity HashSet</returns>
        public static string GetHashKey<T>()
        {
            var hashEntryKeyAttr = typeof(T).GetCustomAttributes(typeof(HashKey), false).FirstOrDefault();
            
            if (hashEntryKeyAttr is null)
            {
                return $"h_{typeof(T).Name}";
            }
            
            var hashEntryKeyMember = (HashKey)(hashEntryKeyAttr);
            return hashEntryKeyMember.Key;
        }
        
        /// <summary>
        /// Converts instance of an object to hash entry list
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="instance">The instance</param>
        /// <returns>Collection of HashEntries</returns>
        /// <exception cref="ArgumentException">Entities without HashEntryKey attribute are not supported</exception>
        public static IEnumerable<HashEntry> ConvertToHashEntryList<T>(this IEnumerable<T> instance)
        {
            var result = instance.Select(entry =>
            {
                var key = entry.GetHashEntryKey();
                var value = MessagePackSerializer.Serialize(entry);
                return new HashEntry(key, value);
            });
            
            return result;
        }

        /// <summary>
        /// Convert collection of HashEntry into collection of entity
        /// </summary>
        /// <param name="instance">Collection of HashEntry to convert</param>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <returns>Collection of entity</returns>
        public static IEnumerable<T> ConvertToCollection<T>(this IEnumerable<HashEntry> instance)
        {
            // TODO: validation + exceptions
            return instance.Select(entry => MessagePackSerializer.Deserialize<T>(entry.Value));
        }

        /// <summary>
        /// Get value of member info
        /// </summary>
        /// <param name="memberInfo">Info of required member</param>
        /// <param name="forObject">Object to get value from</param>
        /// <returns>Value of property</returns>
        /// <exception cref="NotImplementedException">Only Property and Field are supported</exception>
        private static object GetValue(this MemberInfo memberInfo, object forObject)
        {
            return memberInfo.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo) memberInfo).GetValue(forObject),
                MemberTypes.Property => ((PropertyInfo) memberInfo).GetValue(forObject),
                _ => throw new NotImplementedException()
            };
        }
        
        // /// <summary>
        // /// Converts instance of an object to hash entry list
        // /// </summary>
        // /// <typeparam name="T"></typeparam>
        // /// <param name="instance">The instance</param>
        // /// <returns></returns>
        // public static IEnumerable<HashEntry> ToHashEntryList<T>(this T instance) where T: IEnumerable
        // {
        //     var accessor = TypeAccessor.Create(typeof(T));
        //     var members = accessor.GetMembers();
        //     foreach (var member in members)
        //     {
        //         if (member.IsDefined(typeof(IgnoreDataMemberAttribute)))
        //         {
        //             continue;
        //         }
        //
        //         var type = member.Type;
        //         if (!type.IsValueType && type != typeof(string))
        //         {
        //             continue;
        //         }
        //         
        //         var underlyingType = Nullable.GetUnderlyingType(type);
        //         var effectiveType = underlyingType ?? type;
        //         var val = accessor[instance, member.Name];
        //         if (val == null) continue;
        //         if (effectiveType == typeof(DateTime))
        //         {
        //             var date = (DateTime) val;
        //             if (date.Kind == DateTimeKind.Utc)
        //             {
        //
        //                 yield return new HashEntry(member.Name, $"{date.Ticks}|UTC");
        //             }
        //             else
        //             {
        //                 yield return new HashEntry(member.Name, $"{date.Ticks}|LOC");
        //             }
        //         }
        //         else
        //         {
        //             yield return new HashEntry(member.Name, val.ToString());
        //         }
        //     }
        // }
        
        // /// <summary>
        // /// Converts from hash entry list and create instance of type T
        // /// </summary>
        // /// <typeparam name="T"></typeparam>
        // /// <param name="entries">The entries returned from StackExchange.redis</param>
        // /// <returns>Instance of Type T </returns>
        // public static T ToEnumerable<T>(this IEnumerable<HashEntry> entries) where T: new()
        // {
        //     var accessor = TypeAccessor.Create(typeof(T));
        //     var instance = new T();
        //     var hashEntries = entries as HashEntry[] ?? entries.ToArray();
        //     var members = accessor.GetMembers();
        //     foreach (var member in members)
        //     {
        //         if (member.IsDefined(typeof(IgnoreDataMemberAttribute)))
        //         {
        //             continue;
        //         }
        //
        //         var type = member.Type;
        //
        //         if (!type.IsValueType && type != typeof(string))
        //         {
        //             continue;
        //         }
        //
        //         var underlyingType = Nullable.GetUnderlyingType(type);
        //         var effectiveType = underlyingType ?? type;
        //
        //         var entry = hashEntries.FirstOrDefault(e => e.Name.ToString().Equals(member.Name));
        //
        //         if (entry.Equals(new HashEntry()))
        //         {
        //             continue;
        //         }
        //
        //         var value = entry.Value.ToString();
        //
        //         if (string.IsNullOrEmpty(value))
        //         {
        //             continue;
        //         }
        //
        //         if (effectiveType == typeof(DateTime))
        //         {
        //             if (value.EndsWith("|UTC"))
        //             {
        //                 value = value.TrimEnd("|UTC".ToCharArray());
        //
        //                 if (!long.TryParse(value, out var ticks)) continue;
        //                 var date = new DateTime(ticks);
        //                 accessor[instance, member.Name] = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        //             }
        //             else
        //             {
        //                 value = value.TrimEnd("|LOC".ToCharArray());
        //                 if (!long.TryParse(value, out var ticks)) continue;
        //                 var date = new DateTime(ticks);
        //                 accessor[instance, member.Name] = DateTime.SpecifyKind(date, DateTimeKind.Local);
        //             }
        //         }
        //         else
        //         {
        //             accessor[instance, member.Name] = Convert.ChangeType(entry.Value.ToString(), member.Type);
        //         }
        //     }
        //     return instance;
        // }
    }
}