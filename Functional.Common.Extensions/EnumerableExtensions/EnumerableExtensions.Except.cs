namespace Functional;

public static partial class EnumerableExtensions
{
	public static async Task<IEnumerable<TSource>> Except<TSource>(this IEnumerable<TSource> first, Task<IEnumerable<TSource>> second)
		=> first.Except(await second);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this IEnumerable<TSource> first, Task<IEnumerable<TSource>> second, IEqualityComparer<TSource>? comparer)
		=> first.Except(await second, comparer);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this IEnumerable<TSource> first, Task<IOrderedEnumerable<TSource>> second)
		=> first.Except(await second);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this IEnumerable<TSource> first, Task<IOrderedEnumerable<TSource>> second, IEqualityComparer<TSource>? comparer)
		=> first.Except(await second, comparer);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this IEnumerable<TSource> first, IAsyncEnumerable<TSource> second)
		=> first.Except(await second.AsEnumerable());

	public static async Task<IEnumerable<TSource>> Except<TSource>(this IEnumerable<TSource> first, IAsyncEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
		=> first.Except(await second.AsEnumerable(), comparer);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this Task<IEnumerable<TSource>> first, IEnumerable<TSource> second)
		=> (await first).Except(second);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this Task<IEnumerable<TSource>> first, IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
		=> (await first).Except(second, comparer);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this Task<IEnumerable<TSource>> first, Task<IEnumerable<TSource>> second)
		=> (await first).Except(await second);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this Task<IEnumerable<TSource>> first, Task<IEnumerable<TSource>> second, IEqualityComparer<TSource>? comparer)
		=> (await first).Except(await second, comparer);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this Task<IEnumerable<TSource>> first, Task<IOrderedEnumerable<TSource>> second)
		=> (await first).Except(await second);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this Task<IEnumerable<TSource>> first, Task<IOrderedEnumerable<TSource>> second, IEqualityComparer<TSource>? comparer)
		=> (await first).Except(await second, comparer);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this Task<IEnumerable<TSource>> first, IAsyncEnumerable<TSource> second)
		=> (await first).Except(await second.AsEnumerable());

	public static async Task<IEnumerable<TSource>> Except<TSource>(this Task<IEnumerable<TSource>> first, IAsyncEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
		=> (await first).Except(await second.AsEnumerable(), comparer);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this Task<IOrderedEnumerable<TSource>> first, IEnumerable<TSource> second)
		=> (await first).Except(second);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this Task<IOrderedEnumerable<TSource>> first, IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
		=> (await first).Except(second, comparer);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this Task<IOrderedEnumerable<TSource>> first, Task<IEnumerable<TSource>> second)
		=> (await first).Except(await second);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this Task<IOrderedEnumerable<TSource>> first, Task<IEnumerable<TSource>> second, IEqualityComparer<TSource>? comparer)
		=> (await first).Except(await second, comparer);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this Task<IOrderedEnumerable<TSource>> first, Task<IOrderedEnumerable<TSource>> second)
		=> (await first).Except(await second);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this Task<IOrderedEnumerable<TSource>> first, Task<IOrderedEnumerable<TSource>> second, IEqualityComparer<TSource>? comparer)
		=> (await first).Except(await second, comparer);

	public static async Task<IEnumerable<TSource>> Except<TSource>(this Task<IOrderedEnumerable<TSource>> first, IAsyncEnumerable<TSource> second)
		=> (await first).Except(await second.AsEnumerable());

	public static async Task<IEnumerable<TSource>> Except<TSource>(this Task<IOrderedEnumerable<TSource>> first, IAsyncEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
		=> (await first).Except(await second.AsEnumerable(), comparer);

	public static IAsyncEnumerable<TSource> Except<TSource>(this IAsyncEnumerable<TSource> first, IEnumerable<TSource> second)
		=> first.Except(second, null);

	public static async IAsyncEnumerable<TSource> Except<TSource>(this IAsyncEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
	{
		HashSet<TSource>? set = null;

		await foreach (TSource item in first)
		{
			if ((set ??= second.ToHashSet(comparer)).Add(item))
				yield return item;
		}
	}

	public static IAsyncEnumerable<TSource> Except<TSource>(this IAsyncEnumerable<TSource> first, Task<IEnumerable<TSource>> second)
		=> first.Except(second, null);

	public static async IAsyncEnumerable<TSource> Except<TSource>(this IAsyncEnumerable<TSource> first, Task<IEnumerable<TSource>> second, IEqualityComparer<TSource>? comparer)
	{
		HashSet<TSource>? set = null;

		await foreach (TSource item in first)
		{
			if ((set ??= await second.ToHashSet(comparer)).Add(item))
				yield return item;
		}
	}

	public static IAsyncEnumerable<TSource> Except<TSource>(this IAsyncEnumerable<TSource> first, Task<IOrderedEnumerable<TSource>> second)
		=> first.Except(second.AsEnumerable(), null);

	public static IAsyncEnumerable<TSource> Except<TSource>(this IAsyncEnumerable<TSource> first, Task<IOrderedEnumerable<TSource>> second, IEqualityComparer<TSource>? comparer)
		=> first.Except(second.AsEnumerable(), comparer);

	public static IAsyncEnumerable<TSource> Except<TSource>(this IAsyncEnumerable<TSource> first, IAsyncEnumerable<TSource> second)
		=> first.Except(second, null);

	public static async IAsyncEnumerable<TSource> Except<TSource>(this IAsyncEnumerable<TSource> first, IAsyncEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
	{
		HashSet<TSource>? set = null;

		await foreach (TSource item in first)
		{
			if ((set ??= await second.ToHashSet(comparer)).Add(item))
				yield return item;
		}
	}
}
