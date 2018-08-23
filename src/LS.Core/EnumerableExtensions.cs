using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LS.Core
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> Yield<T> (this T item)
		{
			yield return item;
		}

		public static T WithIDOrNull<T> (this IEnumerable<T> array, long id) where T : IIdentifiable
		{
			return array.FirstOrDefault (x => x.ID == id);
		}

		public static T WithID<T> (this IEnumerable<T> array, long id) where T : IIdentifiable
		{
			return array.First (x => x.ID == id);
		}

		public static ImmutableArray<T> ReplaceWithID<T>(this ImmutableArray<T> array, T item) where T : IIdentifiable
		{
			return array.Replace (array.WithID (item.ID), item);
		}
	}
}
