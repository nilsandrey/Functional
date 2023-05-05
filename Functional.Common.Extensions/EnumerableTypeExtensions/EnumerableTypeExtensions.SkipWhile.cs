using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Functional;

public static partial class EnumerableTypeExtensions
{
	public static async Task<IEnumerable<TSource>> SkipWhile<TSource>(this Task<IEnumerable<TSource>> source, Func<TSource, bool> predicate)
		=> (await source).SkipWhile(predicate);

	public static async Task<IEnumerable<TSource>> SkipWhile<TSource>(this Task<IEnumerable<TSource>> source, Func<TSource, int, bool> predicate)
		=> (await source).SkipWhile(predicate);

	public static async Task<IEnumerable<TSource>> SkipWhile<TSource>(this Task<IOrderedEnumerable<TSource>> source, Func<TSource, bool> predicate)
		=> (await source).SkipWhile(predicate);

	public static async Task<IEnumerable<TSource>> SkipWhile<TSource>(this Task<IOrderedEnumerable<TSource>> source, Func<TSource, int, bool> predicate)
		=> (await source).SkipWhile(predicate);

	public static IAsyncEnumerable<TSource> SkipWhile<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		if (predicate == null)
			throw new ArgumentNullException(nameof(predicate));

		return AsyncIteratorEnumerable.Create((source, predicate), static (o, t) => BasicAsyncIterator.Create(o.source, o, BasicAsyncIterator.State.Pending, static (s, _, context) => context.predicate.Invoke(s) ? (BasicAsyncIterator.ContinuationType.Take, s) : (BasicAsyncIterator.ContinuationType.Start, s), t));
	}

	public static IAsyncEnumerable<TSource> SkipWhile<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate)
	{
		if (predicate == null)
			throw new ArgumentNullException(nameof(predicate));

		return AsyncIteratorEnumerable.Create((source, predicate), static (o, t) => BasicAsyncIterator.Create(o.source, o, BasicAsyncIterator.State.Pending, static (s, i, context) => context.predicate.Invoke(s, i) ? (BasicAsyncIterator.ContinuationType.Take, s) : (BasicAsyncIterator.ContinuationType.Start, s), t));
	}
}
