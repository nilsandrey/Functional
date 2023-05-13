namespace Functional;

public static partial class EnumerableExtensions
{
	public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(this IEnumerable<TSource> source)
		=> AsyncEnumerable.Create(source);

	public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(this Task<IEnumerable<TSource>> source)
		=> AsyncEnumerable.Create(source);

	public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(this Task<IOrderedEnumerable<TSource>> source)
		=> AsyncEnumerable.Create(source.AsEnumerable());

	public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(this Task<TSource[]> source)
		=> AsyncEnumerable.Create(source.AsEnumerable());

	public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(this IAsyncEnumerable<TSource> source)
		=> source;

	public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(this Task<IAsyncEnumerable<TSource>> source)
		=> AsyncEnumerable.Create(source);
}
