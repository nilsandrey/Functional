namespace Functional;

public static partial class EnumerableExtensions
{
	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this IEnumerable<TSource> first, Task<IEnumerable<TKey>> second, Func<TSource, TKey> keySelector)
		=> first.IntersectBy(await second, keySelector);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this IEnumerable<TSource> first, Task<IEnumerable<TKey>> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
		=> first.IntersectBy(await second, keySelector, comparer);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this IEnumerable<TSource> first, Task<IOrderedEnumerable<TKey>> second, Func<TSource, TKey> keySelector)
		=> first.IntersectBy(await second, keySelector);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this IEnumerable<TSource> first, Task<IOrderedEnumerable<TKey>> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
		=> first.IntersectBy(await second, keySelector, comparer);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this IEnumerable<TSource> first, IAsyncEnumerable<TKey> second, Func<TSource, TKey> keySelector)
		=> first.IntersectBy(await second.AsEnumerable(), keySelector);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this IEnumerable<TSource> first, IAsyncEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
		=> first.IntersectBy(await second.AsEnumerable(), keySelector, comparer);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this Task<IEnumerable<TSource>> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector)
		=> (await first).IntersectBy(second, keySelector);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this Task<IEnumerable<TSource>> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
		=> (await first).IntersectBy(second, keySelector, comparer);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this Task<IEnumerable<TSource>> first, Task<IEnumerable<TKey>> second, Func<TSource, TKey> keySelector)
		=> (await first).IntersectBy(await second, keySelector);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this Task<IEnumerable<TSource>> first, Task<IEnumerable<TKey>> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
		=> (await first).IntersectBy(await second, keySelector, comparer);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this Task<IEnumerable<TSource>> first, Task<IOrderedEnumerable<TKey>> second, Func<TSource, TKey> keySelector)
		=> (await first).IntersectBy(await second, keySelector);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this Task<IEnumerable<TSource>> first, Task<IOrderedEnumerable<TKey>> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
		=> (await first).IntersectBy(await second, keySelector, comparer);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this Task<IEnumerable<TSource>> first, IAsyncEnumerable<TKey> second, Func<TSource, TKey> keySelector)
		=> (await first).IntersectBy(await second.AsEnumerable(), keySelector);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this Task<IEnumerable<TSource>> first, IAsyncEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
		=> (await first).IntersectBy(await second.AsEnumerable(), keySelector, comparer);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this Task<IOrderedEnumerable<TSource>> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector)
		=> (await first).IntersectBy(second, keySelector);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this Task<IOrderedEnumerable<TSource>> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
		=> (await first).IntersectBy(second, keySelector, comparer);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this Task<IOrderedEnumerable<TSource>> first, Task<IEnumerable<TKey>> second, Func<TSource, TKey> keySelector)
		=> (await first).IntersectBy(await second, keySelector);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this Task<IOrderedEnumerable<TSource>> first, Task<IEnumerable<TKey>> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
		=> (await first).IntersectBy(await second, keySelector, comparer);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this Task<IOrderedEnumerable<TSource>> first, Task<IOrderedEnumerable<TKey>> second, Func<TSource, TKey> keySelector)
		=> (await first).IntersectBy(await second, keySelector);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this Task<IOrderedEnumerable<TSource>> first, Task<IOrderedEnumerable<TKey>> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
		=> (await first).IntersectBy(await second, keySelector, comparer);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this Task<IOrderedEnumerable<TSource>> first, IAsyncEnumerable<TKey> second, Func<TSource, TKey> keySelector)
		=> (await first).IntersectBy(await second.AsEnumerable(), keySelector);

	public static async Task<IEnumerable<TSource>> IntersectBy<TSource, TKey>(this Task<IOrderedEnumerable<TSource>> first, IAsyncEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
		=> (await first).IntersectBy(await second.AsEnumerable(), keySelector, comparer);

	public static IAsyncEnumerable<TSource> IntersectBy<TSource, TKey>(this IAsyncEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector)
		=> first.IntersectBy(second, keySelector, null);

	public static async IAsyncEnumerable<TSource> IntersectBy<TSource, TKey>(this IAsyncEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
	{
		HashSet<TKey>? set = null;

		await foreach (TSource item in first)
		{
			if ((set ??= second.ToHashSet(comparer)).Remove(keySelector.Invoke(item)))
				yield return item;
		}
	}

	public static IAsyncEnumerable<TSource> IntersectBy<TSource, TKey>(this IAsyncEnumerable<TSource> first, Task<IEnumerable<TKey>> second, Func<TSource, TKey> keySelector)
		=> first.IntersectBy(second, keySelector, null);

	public static async IAsyncEnumerable<TSource> IntersectBy<TSource, TKey>(this IAsyncEnumerable<TSource> first, Task<IEnumerable<TKey>> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
	{
		HashSet<TKey>? set = null;

		await foreach (TSource item in first)
		{
			if ((set ??= await second.ToHashSet(comparer)).Remove(keySelector.Invoke(item)))
				yield return item;
		}
	}

	public static IAsyncEnumerable<TSource> IntersectBy<TSource, TKey>(this IAsyncEnumerable<TSource> first, Task<IOrderedEnumerable<TKey>> second, Func<TSource, TKey> keySelector)
		=> first.IntersectBy(second.AsEnumerable(), keySelector, null);

	public static IAsyncEnumerable<TSource> IntersectBy<TSource, TKey>(this IAsyncEnumerable<TSource> first, Task<IOrderedEnumerable<TKey>> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
		=> first.IntersectBy(second.AsEnumerable(), keySelector, comparer);

	public static IAsyncEnumerable<TSource> IntersectBy<TSource, TKey>(this IAsyncEnumerable<TSource> first, IAsyncEnumerable<TKey> second, Func<TSource, TKey> keySelector)
		=> first.IntersectBy(second, keySelector, null);

	public static async IAsyncEnumerable<TSource> IntersectBy<TSource, TKey>(this IAsyncEnumerable<TSource> first, IAsyncEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
	{
		HashSet<TKey>? set = null;

		await foreach (TSource item in first)
		{
			if ((set ??= await second.ToHashSet(comparer)).Remove(keySelector.Invoke(item)))
				yield return item;
		}
	}
}