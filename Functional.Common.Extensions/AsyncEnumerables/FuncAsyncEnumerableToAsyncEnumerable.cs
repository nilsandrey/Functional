﻿namespace Functional;

internal static class FuncAsyncEnumerableToAsyncEnumerable
{
	public static IAsyncEnumerable<TSource> Create<TValue, TSource>(TValue value, Func<TValue, IAsyncEnumerable<TSource>> sourceFactory)
		=> new FuncAsyncEnumerableToAsyncEnumerable<TValue, TSource>(value, sourceFactory);
}

internal class FuncAsyncEnumerableToAsyncEnumerable<TValue, TSource> : IAsyncEnumerable<TSource>
{
	private readonly TValue _value;
	private readonly Func<TValue, IAsyncEnumerable<TSource>> _source;

	public FuncAsyncEnumerableToAsyncEnumerable(TValue value, Func<TValue, IAsyncEnumerable<TSource>> source)
	{
		_value = value;
		_source = source;
	}

	public IAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		=> _source.Invoke(_value).GetAsyncEnumerator(cancellationToken);
}
