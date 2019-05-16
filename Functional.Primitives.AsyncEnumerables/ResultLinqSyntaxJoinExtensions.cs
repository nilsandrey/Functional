﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functional
{
	public static class ResultLinqSyntaxJoinExtensions
	{
		private static class ResultJoinEnumerable
		{
			public static IResultEnumerable<TResult, TFailure> Create<TOuter, TInner, TKey, TResult, TFailure>(IResultEnumerable<TOuter, TFailure> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
				=> new ResultGroupJoinEnumerable<TOuter, TInner, TKey, TFailure>(outer, inner, outerKeySelector, innerKeySelector)
					.SelectMany(result => result.Match(success => success.inner.Select(value => resultSelector.Invoke(success.outer, value)).Select(value => Result.Success<TResult, TFailure>(value)), failure => new[] { Result.Failure<TResult, TFailure>(failure) }))
					.AsResultEnumerable();

			public static async Task<IResultEnumerable<TResult, TFailure>> Create<TOuter, TInner, TKey, TResult, TFailure>(IResultEnumerable<TOuter, TFailure> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
				=> new ResultGroupJoinEnumerable<TOuter, TInner, TKey, TFailure>(outer, await inner, outerKeySelector, innerKeySelector)
					.SelectMany(result => result.Match(success => success.inner.Select(value => resultSelector.Invoke(success.outer, value)).Select(value => Result.Success<TResult, TFailure>(value)), failure => new[] { Result.Failure<TResult, TFailure>(failure) }))
					.AsResultEnumerable();
		}

		private static class ResultGroupJoinEnumerable
		{
			public static IResultEnumerable<TResult, TFailure> Create<TOuter, TInner, TKey, TResult, TFailure>(IResultEnumerable<TOuter, TFailure> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
				=> new ResultGroupJoinEnumerable<TOuter, TInner, TKey, TFailure>(outer, inner, outerKeySelector, innerKeySelector)
					.Select(result => result.Match(success => Result.Success<TResult, TFailure>(resultSelector.Invoke(success.outer, success.inner)), Result.Failure<TResult, TFailure>))
					.AsResultEnumerable();

			public static async Task<IResultEnumerable<TResult, TFailure>> Create<TOuter, TInner, TKey, TResult, TFailure>(IResultEnumerable<TOuter, TFailure> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
				=> new ResultGroupJoinEnumerable<TOuter, TInner, TKey, TFailure>(outer, await inner, outerKeySelector, innerKeySelector)
					.Select(result => result.Match(success => Result.Success<TResult, TFailure>(resultSelector.Invoke(success.outer, success.inner)), Result.Failure<TResult, TFailure>))
					.AsResultEnumerable();
		}

		private class ResultGroupJoinEnumerable<TOuter, TInner, TKey, TFailure> : IResultEnumerable<(TOuter outer, IEnumerable<TInner> inner), TFailure>
		{
			public readonly IResultEnumerable<TOuter, TFailure> _outer;
			public readonly IEnumerable<TInner> _inner;
			public readonly Func<TOuter, TKey> _outerKeySelector;
			public readonly Func<TInner, TKey> _innerKeySelector;

			public ResultGroupJoinEnumerable(IResultEnumerable<TOuter, TFailure> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector)
			{
				_outer = outer ?? throw new ArgumentNullException(nameof(outer));
				_inner = inner ?? throw new ArgumentNullException(nameof(inner));
				_outerKeySelector = outerKeySelector ?? throw new ArgumentNullException(nameof(outerKeySelector));
				_innerKeySelector = innerKeySelector ?? throw new ArgumentNullException(nameof(innerKeySelector));
			}

			public IEnumerator<Result<(TOuter outer, IEnumerable<TInner> inner), TFailure>> GetEnumerator()
				=> new ResultGroupJoinEnumerator<TOuter, TInner, TKey, TFailure>(_outer.GetEnumerator(), _inner, _outerKeySelector, _innerKeySelector);

			IEnumerator IEnumerable.GetEnumerator()
				=> GetEnumerator();
		}

		private class ResultGroupJoinEnumerator<TOuter, TInner, TKey, TFailure> : IEnumerator<Result<(TOuter outer, IEnumerable<TInner> inner), TFailure>>
		{
			private readonly IEnumerator<Result<TOuter, TFailure>> _outer;
			private readonly IEnumerable<TInner> _inner;
			private readonly Func<TOuter, TKey> _outerKeySelector;
			private readonly Func<TInner, TKey> _innerKeySelector;

			public Result<(TOuter outer, IEnumerable<TInner> inner), TFailure> Current { get; private set; }

			object IEnumerator.Current => Current;

			private ILookup<TKey, TInner> _lookup;
			private ILookup<TKey, TInner> GetLookup() 
				=> _lookup ?? (_lookup = _inner.ToLookup(_innerKeySelector));

			public ResultGroupJoinEnumerator(IEnumerator<Result<TOuter, TFailure>> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector)
			{
				_outer = outer ?? throw new ArgumentNullException(nameof(outer));
				_inner = inner ?? throw new ArgumentNullException(nameof(inner));
				_outerKeySelector = outerKeySelector ?? throw new ArgumentNullException(nameof(outerKeySelector));
				_innerKeySelector = innerKeySelector ?? throw new ArgumentNullException(nameof(innerKeySelector));
			}

			public void Dispose()
				=> _outer.Dispose();

			public bool MoveNext()
			{
				if (_outer.MoveNext())
				{
					Current = _outer
					   .Current
					   .Select(outerValue => (outerValue, GetLookup()[_outerKeySelector.Invoke(outerValue)]));

					return true;
				}
				else
				{
					Current = default;
					return false;
				}
			}

			public void Reset()
			{
				_lookup = null;
				_outer.Reset();
			}
		}

		private static class AsyncResultJoinEnumerable
		{
			public static IAsyncResultEnumerable<TResult, TFailure> Create<TOuter, TInner, TKey, TResult, TFailure>(IAsyncResultEnumerable<TOuter, TFailure> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
				=> new AsyncResultGroupJoinEnumerable<TOuter, TInner, TKey, TFailure>(outer, Task.FromResult(inner), outerKeySelector, innerKeySelector)
					.SelectMany(result => result.Match(success => success.inner.Select(value => resultSelector.Invoke(success.outer, value)).Select(value => Result.Success<TResult, TFailure>(value)), failure => new[] { Result.Failure<TResult, TFailure>(failure) }))
					.AsAsyncResultEnumerable();

			public static IAsyncResultEnumerable<TResult, TFailure> Create<TOuter, TInner, TKey, TResult, TFailure>(IAsyncResultEnumerable<TOuter, TFailure> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
				=> new AsyncResultGroupJoinEnumerable<TOuter, TInner, TKey, TFailure>(outer, inner, outerKeySelector, innerKeySelector)
					.SelectMany(result => result.Match(success => success.inner.Select(value => resultSelector.Invoke(success.outer, value)).Select(value => Result.Success<TResult, TFailure>(value)), failure => new[] { Result.Failure<TResult, TFailure>(failure) }))
					.AsAsyncResultEnumerable();

			public static IAsyncResultEnumerable<TResult, TFailure> Create<TOuter, TInner, TKey, TResult, TFailure>(IAsyncResultEnumerable<TOuter, TFailure> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
				=> new AsyncResultGroupJoinEnumerable<TOuter, TInner, TKey, TFailure>(outer, inner.AsEnumerable(), outerKeySelector, innerKeySelector)
					.SelectMany(result => result.Match(success => success.inner.Select(value => resultSelector.Invoke(success.outer, value)).Select(value => Result.Success<TResult, TFailure>(value)), failure => new[] { Result.Failure<TResult, TFailure>(failure) }))
					.AsAsyncResultEnumerable();
		}

		private static class AsyncResultGroupJoinEnumerable
		{
			public static IAsyncResultEnumerable<TResult, TFailure> Create<TOuter, TInner, TKey, TResult, TFailure>(IAsyncResultEnumerable<TOuter, TFailure> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
				=> new AsyncResultGroupJoinEnumerable<TOuter, TInner, TKey, TFailure>(outer, Task.FromResult(inner), outerKeySelector, innerKeySelector)
					.Select(result => result.Match(success => Result.Success<TResult, TFailure>(resultSelector.Invoke(success.outer, success.inner)), Result.Failure<TResult, TFailure>))
					.AsAsyncResultEnumerable();

			public static IAsyncResultEnumerable<TResult, TFailure> Create<TOuter, TInner, TKey, TResult, TFailure>(IAsyncResultEnumerable<TOuter, TFailure> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
				=> new AsyncResultGroupJoinEnumerable<TOuter, TInner, TKey, TFailure>(outer, inner, outerKeySelector, innerKeySelector)
					.Select(result => result.Match(success => Result.Success<TResult, TFailure>(resultSelector.Invoke(success.outer, success.inner)), Result.Failure<TResult, TFailure>))
					.AsAsyncResultEnumerable();

			public static IAsyncResultEnumerable<TResult, TFailure> Create<TOuter, TInner, TKey, TResult, TFailure>(IAsyncResultEnumerable<TOuter, TFailure> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
				=> new AsyncResultGroupJoinEnumerable<TOuter, TInner, TKey, TFailure>(outer, inner.AsEnumerable(), outerKeySelector, innerKeySelector)
					.Select(result => result.Match(success => Result.Success<TResult, TFailure>(resultSelector.Invoke(success.outer, success.inner)), Result.Failure<TResult, TFailure>))
					.AsAsyncResultEnumerable();
		}

		private class AsyncResultGroupJoinEnumerable<TOuter, TInner, TKey, TFailure> : IAsyncResultEnumerable<(TOuter outer, IEnumerable<TInner> inner), TFailure>
		{
			public readonly IAsyncResultEnumerable<TOuter, TFailure> _outer;
			public readonly Task<IEnumerable<TInner>> _inner;
			public readonly Func<TOuter, TKey> _outerKeySelector;
			public readonly Func<TInner, TKey> _innerKeySelector;

			public AsyncResultGroupJoinEnumerable(IAsyncResultEnumerable<TOuter, TFailure> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector)
			{
				_outer = outer ?? throw new ArgumentNullException(nameof(outer));
				_inner = inner ?? throw new ArgumentNullException(nameof(inner));
				_outerKeySelector = outerKeySelector ?? throw new ArgumentNullException(nameof(outerKeySelector));
				_innerKeySelector = innerKeySelector ?? throw new ArgumentNullException(nameof(innerKeySelector));
			}

			public IAsyncEnumerator<Result<(TOuter outer, IEnumerable<TInner> inner), TFailure>> GetEnumerator()
				=> new AsyncResultGroupJoinEnumerator<TOuter, TInner, TKey, TFailure>(_outer.GetEnumerator(), _inner, _outerKeySelector, _innerKeySelector);
		}

		private class AsyncResultGroupJoinEnumerator<TOuter, TInner, TKey, TFailure> : IAsyncEnumerator<Result<(TOuter outer, IEnumerable<TInner> inner), TFailure>>
		{
			private readonly IAsyncEnumerator<Result<TOuter, TFailure>> _outer;
			private readonly Task<IEnumerable<TInner>> _inner;
			private readonly Func<TOuter, TKey> _outerKeySelector;
			private readonly Func<TInner, TKey> _innerKeySelector;

			public Result<(TOuter outer, IEnumerable<TInner> inner), TFailure> Current { get; private set; }

			private ILookup<TKey, TInner> _lookup;
			private async Task<ILookup<TKey, TInner>> GetLookup() 
				=> _lookup ?? (_lookup = (await _inner).ToLookup(_innerKeySelector));

			public AsyncResultGroupJoinEnumerator(IAsyncEnumerator<Result<TOuter, TFailure>> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector)
			{
				_outer = outer ?? throw new ArgumentNullException(nameof(outer));
				_inner = inner ?? throw new ArgumentNullException(nameof(inner));
				_outerKeySelector = outerKeySelector ?? throw new ArgumentNullException(nameof(outerKeySelector));
				_innerKeySelector = innerKeySelector ?? throw new ArgumentNullException(nameof(innerKeySelector));
			}

			public async Task<bool> MoveNext()
			{
				if (await _outer.MoveNext())
				{
					Current = await _outer
					   .Current
					   .SelectAsync(async outerValue => (outerValue, (await GetLookup())[_outerKeySelector.Invoke(outerValue)]));

					return true;
				}
				else
				{
					Current = default;
					return false;
				}
			}
		}

		private static class AsyncJoinEnumerable
		{
			public static IAsyncEnumerable<TResult> Create<TOuter, TInner, TKey, TResult>(IAsyncEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
				=> new AsyncGroupJoinEnumerable<TOuter, TInner, TKey>(outer, Task.FromResult(inner), outerKeySelector, innerKeySelector)
					.SelectMany(result => result.inner.Select(value => resultSelector.Invoke(result.outer, value)));

			public static IAsyncEnumerable<TResult> Create<TOuter, TInner, TKey, TResult>(IAsyncEnumerable<TOuter> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
				=> new AsyncGroupJoinEnumerable<TOuter, TInner, TKey>(outer, inner, outerKeySelector, innerKeySelector)
					.SelectMany(result => result.inner.Select(value => resultSelector.Invoke(result.outer, value)));

			public static IAsyncEnumerable<TResult> Create<TOuter, TInner, TKey, TResult>(IAsyncEnumerable<TOuter> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
				=> new AsyncGroupJoinEnumerable<TOuter, TInner, TKey>(outer, inner.AsEnumerable(), outerKeySelector, innerKeySelector)
					.SelectMany(result => result.inner.Select(value => resultSelector.Invoke(result.outer, value)));
		}

		private static class AsyncGroupJoinEnumerable
		{
			public static IAsyncEnumerable<TResult> Create<TOuter, TInner, TKey, TResult>(IAsyncEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
				=> new AsyncGroupJoinEnumerable<TOuter, TInner, TKey>(outer, Task.FromResult(inner), outerKeySelector, innerKeySelector)
					.Select(result => resultSelector.Invoke(result.outer, result.inner));

			public static IAsyncEnumerable<TResult> Create<TOuter, TInner, TKey, TResult>(IAsyncEnumerable<TOuter> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
				=> new AsyncGroupJoinEnumerable<TOuter, TInner, TKey>(outer, inner, outerKeySelector, innerKeySelector)
					.Select(result => resultSelector.Invoke(result.outer, result.inner));

			public static IAsyncEnumerable<TResult> Create<TOuter, TInner, TKey, TResult>(IAsyncEnumerable<TOuter> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
				=> new AsyncGroupJoinEnumerable<TOuter, TInner, TKey>(outer, inner.AsEnumerable(), outerKeySelector, innerKeySelector)
					.Select(result => resultSelector.Invoke(result.outer, result.inner));
		}

		private class AsyncGroupJoinEnumerable<TOuter, TInner, TKey> : IAsyncEnumerable<(TOuter outer, IEnumerable<TInner> inner)>
		{
			public readonly IAsyncEnumerable<TOuter> _outer;
			public readonly Task<IEnumerable<TInner>> _inner;
			public readonly Func<TOuter, TKey> _outerKeySelector;
			public readonly Func<TInner, TKey> _innerKeySelector;

			public AsyncGroupJoinEnumerable(IAsyncEnumerable<TOuter> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector)
			{
				_outer = outer ?? throw new ArgumentNullException(nameof(outer));
				_inner = inner ?? throw new ArgumentNullException(nameof(inner));
				_outerKeySelector = outerKeySelector ?? throw new ArgumentNullException(nameof(outerKeySelector));
				_innerKeySelector = innerKeySelector ?? throw new ArgumentNullException(nameof(innerKeySelector));
			}

			public IAsyncEnumerator<(TOuter outer, IEnumerable<TInner> inner)> GetEnumerator()
				=> new AsyncGroupJoinEnumerator<TOuter, TInner, TKey>(_outer.GetEnumerator(), _inner, _outerKeySelector, _innerKeySelector);
		}

		private class AsyncGroupJoinEnumerator<TOuter, TInner, TKey> : IAsyncEnumerator<(TOuter outer, IEnumerable<TInner> inner)>
		{
			private readonly IAsyncEnumerator<TOuter> _outer;
			private readonly Task<IEnumerable<TInner>> _inner;
			private readonly Func<TOuter, TKey> _outerKeySelector;
			private readonly Func<TInner, TKey> _innerKeySelector;

			public (TOuter outer, IEnumerable<TInner> inner) Current { get; private set; }

			private ILookup<TKey, TInner> _lookup;
			private async Task<ILookup<TKey, TInner>> GetLookup()
				=> _lookup ?? (_lookup = (await _inner).ToLookup(_innerKeySelector));

			public AsyncGroupJoinEnumerator(IAsyncEnumerator<TOuter> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector)
			{
				_outer = outer ?? throw new ArgumentNullException(nameof(outer));
				_inner = inner ?? throw new ArgumentNullException(nameof(inner));
				_outerKeySelector = outerKeySelector ?? throw new ArgumentNullException(nameof(outerKeySelector));
				_innerKeySelector = innerKeySelector ?? throw new ArgumentNullException(nameof(innerKeySelector));
			}

			public async Task<bool> MoveNext()
			{
				if (await _outer.MoveNext())
				{
					Current = (_outer.Current, (await GetLookup())[_outerKeySelector.Invoke(_outer.Current)]);

					return true;
				}
				else
				{
					Current = default;
					return false;
				}
			}
		}

		// ------------------------- //

		private static async Task<IEnumerable<TResult>> DoJoin<TOuter, TInner, TKey, TResult>(Task<IEnumerable<TOuter>> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> (await outer).Join(await inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> DoJoin(Task.FromResult(outer), inner.AsEnumerable(), outerKeySelector, innerKeySelector, resultSelector).AsAsyncEnumerable();

		public static IAsyncEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this Task<IEnumerable<TOuter>> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> DoJoin(outer, inner.AsEnumerable(), outerKeySelector, innerKeySelector, resultSelector).AsAsyncEnumerable();

		public static IAsyncEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IAsyncEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> AsyncJoinEnumerable.Create(outer, inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IAsyncEnumerable<TOuter> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> AsyncJoinEnumerable.Create(outer, inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IAsyncEnumerable<TOuter> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> AsyncJoinEnumerable.Create(outer, inner, outerKeySelector, innerKeySelector, resultSelector);

		// ------------------------- //

		public static IResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IResultEnumerable<TOuter, TFailure> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> ResultJoinEnumerable.Create(outer, inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IResultEnumerable<TOuter, TFailure> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> ResultJoinEnumerable.Create(outer, inner, outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IResultEnumerable<TOuter, TFailure> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> ResultJoinEnumerable.Create(outer, inner.AsEnumerable(), outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IResultEnumerable<TOuter, TFailure> outer, Result<IEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.Join(success, outerKeySelector, innerKeySelector, resultSelector),
					failure => outer
						.Where(result => result.Match(_ => false, f => true))
						.Select(result => result.Match(_ => default, Result.Failure<TResult, TFailure>))
						.Concat(new[] { Result.Failure<TResult, TFailure>(failure) })
						.AsResultEnumerable()
				);

		private static async Task<IResultEnumerable<TResult, TFailure>> DoJoin<TOuter, TInner, TKey, TResult, TFailure>(IResultEnumerable<TOuter, TFailure> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> outer.Join(await inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IResultEnumerable<TOuter, TFailure> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> DoJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IResultEnumerable<TOuter, TFailure> outer, Result<IAsyncEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> DoJoin(outer, inner.MatchAsync(async success => Result.Success<IEnumerable<TInner>, TFailure>(await success.AsEnumerable()), failure => Task.FromResult(Result.Failure<IEnumerable<TInner>, TFailure>(failure))), outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IResultEnumerable<TOuter, TFailure> outer, Result<TInner[], TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> outer.Join(inner.Select(arr => arr.AsEnumerable()), outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IResultEnumerable<TOuter, TFailure> outer, Task<Result<TInner[], TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> DoJoin(outer, inner.Select(arr => arr.AsEnumerable()), outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IEnumerable<TOuter> outer, Result<IEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.Join(success, outerKeySelector, innerKeySelector, resultSelector).Select(Result.Success<TResult, TFailure>),
					failure => new[] { Result.Failure<TResult, TFailure>(failure) }
				)
				.AsResultEnumerable();

		private static async Task<IResultEnumerable<TResult, TFailure>> DoJoin<TOuter, TInner, TKey, TResult, TFailure>(IEnumerable<TOuter> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> outer.Join(await inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IEnumerable<TOuter> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> DoJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IEnumerable<TOuter> outer, Result<IAsyncEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> DoJoin(outer, inner.MatchAsync(async success => Result.Success<IEnumerable<TInner>, TFailure>(await success.AsEnumerable()), failure => Task.FromResult(Result.Failure<IEnumerable<TInner>, TFailure>(failure))), outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IEnumerable<TOuter> outer, Result<TInner[], TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> outer.Join(inner.Select(arr => arr.AsEnumerable()), outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IEnumerable<TOuter> outer, Task<Result<TInner[], TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> DoJoin(outer, inner.Select(arr => arr.AsEnumerable()), outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		private static Task<IEnumerable<Result<TResult, TFailure>>> DoJoin<TOuter, TInner, TKey, TResult, TFailure>(Task<IEnumerable<TOuter>> outer, Result<IEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> inner
				.MatchAsync
				(
					success => outer.Join(success, outerKeySelector, innerKeySelector, resultSelector).Select(Result.Success<TResult, TFailure>),
					failure => Task.FromResult(new[] { Result.Failure<TResult, TFailure>(failure) }.AsEnumerable())
				);

		private static Task<IEnumerable<Result<TResult, TFailure>>> DoJoin<TOuter, TInner, TKey, TResult, TFailure>(Task<IEnumerable<TOuter>> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> inner
				.MatchAsync
				(
					success => outer.Join(success, outerKeySelector, innerKeySelector, resultSelector).Select(Result.Success<TResult, TFailure>),
					failure => Task.FromResult(new[] { Result.Failure<TResult, TFailure>(failure) }.AsEnumerable())
				);

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this Task<IEnumerable<TOuter>> outer, Result<IEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> DoJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this Task<IEnumerable<TOuter>> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> DoJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this Task<IEnumerable<TOuter>> outer, Result<IAsyncEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> DoJoin(outer, inner.MatchAsync(async success => Result.Success<IEnumerable<TInner>, TFailure>(await success.AsEnumerable()), failure => Task.FromResult(Result.Failure<IEnumerable<TInner>, TFailure>(failure))), outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this Task<IEnumerable<TOuter>> outer, Result<TInner[], TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> DoJoin(outer, inner.Select(arr => arr.AsEnumerable()), outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this Task<IEnumerable<TOuter>> outer, Task<Result<TInner[], TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> DoJoin(outer, inner.Select(arr => arr.AsEnumerable()), outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		// ------------------------- //

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncResultEnumerable<TOuter, TFailure> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> AsyncResultJoinEnumerable.Create(outer, inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncResultEnumerable<TOuter, TFailure> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> AsyncResultJoinEnumerable.Create(outer, inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncResultEnumerable<TOuter, TFailure> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> AsyncResultJoinEnumerable.Create(outer, inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncResultEnumerable<TOuter, TFailure> outer, Result<IEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.Join(success, outerKeySelector, innerKeySelector, resultSelector),
					failure => outer
						.Where(result => result.Match(_ => false, f => true))
						.Select(result => result.Match(_ => default, Result.Failure<TResult, TFailure>))
						.Concat(new[] { Result.Failure<TResult, TFailure>(failure) })
						.AsAsyncResultEnumerable()
				);

		private static async Task<IAsyncResultEnumerable<TResult, TFailure>> DoJoin<TOuter, TInner, TKey, TResult, TFailure>(IAsyncResultEnumerable<TOuter, TFailure> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> outer.Join(await inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncResultEnumerable<TOuter, TFailure> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> DoJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncResultEnumerable<TOuter, TFailure> outer, Result<IAsyncEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.Join(success, outerKeySelector, innerKeySelector, resultSelector),
					failure => outer
						.Where(result => result.Match(_ => false, f => true))
						.Select(result => result.Match(_ => default, Result.Failure<TResult, TFailure>))
						.Concat(new[] { Result.Failure<TResult, TFailure>(failure) })
						.AsAsyncResultEnumerable()
				);

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncResultEnumerable<TOuter, TFailure> outer, Result<TInner[], TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> outer.Join(inner.Select(arr => arr.AsEnumerable()), outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncResultEnumerable<TOuter, TFailure> outer, Task<Result<TInner[], TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> outer.Join(inner.Select(arr => arr.AsEnumerable()), outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncEnumerable<TOuter> outer, Result<IEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.Join(success, outerKeySelector, innerKeySelector, resultSelector).Select(Result.Success<TResult, TFailure>),
					failure => new[] { Result.Failure<TResult, TFailure>(failure) }.AsAsyncEnumerable()
				)
				.AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncEnumerable<TOuter> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.Join(success, outerKeySelector, innerKeySelector, resultSelector).Select(Result.Success<TResult, TFailure>),
					failure => new[] { Result.Failure<TResult, TFailure>(failure) }.AsAsyncEnumerable()
				)
				.AsAsyncResultEnumerable();

	public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncEnumerable<TOuter> outer, Result<IAsyncEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.Join(success, outerKeySelector, innerKeySelector, resultSelector).Select(Result.Success<TResult, TFailure>),
					failure => new[] { Result.Failure<TResult, TFailure>(failure) }.AsAsyncEnumerable()
				)
				.AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncEnumerable<TOuter> outer, Result<TInner[], TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.Join(success, outerKeySelector, innerKeySelector, resultSelector).Select(Result.Success<TResult, TFailure>),
					failure => new[] { Result.Failure<TResult, TFailure>(failure) }.AsAsyncEnumerable()
				)
				.AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> Join<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncEnumerable<TOuter> outer, Task<Result<TInner[], TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.Join(success, outerKeySelector, innerKeySelector, resultSelector).Select(Result.Success<TResult, TFailure>),
					failure => new[] { Result.Failure<TResult, TFailure>(failure) }.AsAsyncEnumerable()
				)
				.AsAsyncResultEnumerable();

		// ------------------------- //

		private static async Task<IEnumerable<TResult>> DoGroupJoin<TOuter, TInner, TKey, TResult>(Task<IEnumerable<TOuter>> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> (await outer).GroupJoin(await inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> DoGroupJoin(Task.FromResult(outer), inner.AsEnumerable(), outerKeySelector, innerKeySelector, resultSelector).AsAsyncEnumerable();

		public static IAsyncEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this Task<IEnumerable<TOuter>> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> DoGroupJoin(outer, inner.AsEnumerable(), outerKeySelector, innerKeySelector, resultSelector).AsAsyncEnumerable();

		public static IAsyncEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IAsyncEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> AsyncGroupJoinEnumerable.Create(outer, inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IAsyncEnumerable<TOuter> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> AsyncGroupJoinEnumerable.Create(outer, inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IAsyncEnumerable<TOuter> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> AsyncGroupJoinEnumerable.Create(outer, inner, outerKeySelector, innerKeySelector, resultSelector);

		// ------------------------- //

		public static IResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IResultEnumerable<TOuter, TFailure> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> ResultGroupJoinEnumerable.Create(outer, inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IResultEnumerable<TOuter, TFailure> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> ResultGroupJoinEnumerable.Create(outer, inner, outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IResultEnumerable<TOuter, TFailure> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> ResultGroupJoinEnumerable.Create(outer, inner.AsEnumerable(), outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IResultEnumerable<TOuter, TFailure> outer, Result<IEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.GroupJoin(success, outerKeySelector, innerKeySelector, resultSelector),
					failure => outer
						.Where(result => result.Match(_ => false, f => true))
						.Select(result => result.Match(_ => default, Result.Failure<TResult, TFailure>))
						.Concat(new[] { Result.Failure<TResult, TFailure>(failure) })
						.AsResultEnumerable()
				);

		private static async Task<IResultEnumerable<TResult, TFailure>> DoGroupJoin<TOuter, TInner, TKey, TResult, TFailure>(IResultEnumerable<TOuter, TFailure> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> outer.GroupJoin(await inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IResultEnumerable<TOuter, TFailure> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> DoGroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IResultEnumerable<TOuter, TFailure> outer, Result<IAsyncEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> DoGroupJoin(outer, inner.MatchAsync(async success => Result.Success<IEnumerable<TInner>, TFailure>(await success.AsEnumerable()), failure => Task.FromResult(Result.Failure<IEnumerable<TInner>, TFailure>(failure))), outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IResultEnumerable<TOuter, TFailure> outer, Result<TInner[], TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> outer.GroupJoin(inner.Select(arr => arr.AsEnumerable()), outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IResultEnumerable<TOuter, TFailure> outer, Task<Result<TInner[], TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> DoGroupJoin(outer, inner.Select(arr => arr.AsEnumerable()), outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IEnumerable<TOuter> outer, Result<IEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.GroupJoin(success, outerKeySelector, innerKeySelector, resultSelector).Select(Result.Success<TResult, TFailure>),
					failure => new[] { Result.Failure<TResult, TFailure>(failure) }
				)
				.AsResultEnumerable();

		private static async Task<IResultEnumerable<TResult, TFailure>> DoGroupJoin<TOuter, TInner, TKey, TResult, TFailure>(IEnumerable<TOuter> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> outer.GroupJoin(await inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IEnumerable<TOuter> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> DoGroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IEnumerable<TOuter> outer, Result<IAsyncEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> DoGroupJoin(outer, inner.MatchAsync(async success => Result.Success<IEnumerable<TInner>, TFailure>(await success.AsEnumerable()), failure => Task.FromResult(Result.Failure<IEnumerable<TInner>, TFailure>(failure))), outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IEnumerable<TOuter> outer, Result<TInner[], TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> outer.GroupJoin(inner.Select(arr => arr.AsEnumerable()), outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IEnumerable<TOuter> outer, Task<Result<TInner[], TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> DoGroupJoin(outer, inner.Select(arr => arr.AsEnumerable()), outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		private static Task<IEnumerable<Result<TResult, TFailure>>> DoGroupJoin<TOuter, TInner, TKey, TResult, TFailure>(Task<IEnumerable<TOuter>> outer, Result<IEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> inner
				.MatchAsync
				(
					success => outer.GroupJoin(success, outerKeySelector, innerKeySelector, resultSelector).Select(Result.Success<TResult, TFailure>),
					failure => Task.FromResult(new[] { Result.Failure<TResult, TFailure>(failure) }.AsEnumerable())
				);

		private static Task<IEnumerable<Result<TResult, TFailure>>> DoGroupJoin<TOuter, TInner, TKey, TResult, TFailure>(Task<IEnumerable<TOuter>> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> inner
				.MatchAsync
				(
					success => outer.GroupJoin(success, outerKeySelector, innerKeySelector, resultSelector).Select(Result.Success<TResult, TFailure>),
					failure => Task.FromResult(new[] { Result.Failure<TResult, TFailure>(failure) }.AsEnumerable())
				);

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this Task<IEnumerable<TOuter>> outer, Result<IEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> DoGroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this Task<IEnumerable<TOuter>> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> DoGroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this Task<IEnumerable<TOuter>> outer, Result<IAsyncEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> DoGroupJoin(outer, inner.MatchAsync(async success => Result.Success<IEnumerable<TInner>, TFailure>(await success.AsEnumerable()), failure => Task.FromResult(Result.Failure<IEnumerable<TInner>, TFailure>(failure))), outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this Task<IEnumerable<TOuter>> outer, Result<TInner[], TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> DoGroupJoin(outer, inner.Select(arr => arr.AsEnumerable()), outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this Task<IEnumerable<TOuter>> outer, Task<Result<TInner[], TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> DoGroupJoin(outer, inner.Select(arr => arr.AsEnumerable()), outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		// ------------------------- //

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncResultEnumerable<TOuter, TFailure> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> AsyncResultGroupJoinEnumerable.Create(outer, inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncResultEnumerable<TOuter, TFailure> outer, Task<IEnumerable<TInner>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> AsyncResultGroupJoinEnumerable.Create(outer, inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncResultEnumerable<TOuter, TFailure> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> AsyncResultGroupJoinEnumerable.Create(outer, inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncResultEnumerable<TOuter, TFailure> outer, Result<IEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.GroupJoin(success, outerKeySelector, innerKeySelector, resultSelector),
					failure => outer
						.Where(result => result.Match(_ => false, f => true))
						.Select(result => result.Match(_ => default, Result.Failure<TResult, TFailure>))
						.Concat(new[] { Result.Failure<TResult, TFailure>(failure) })
						.AsAsyncResultEnumerable()
				);

		private static async Task<IAsyncResultEnumerable<TResult, TFailure>> DoGroupJoin<TOuter, TInner, TKey, TResult, TFailure>(IAsyncResultEnumerable<TOuter, TFailure> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> outer.GroupJoin(await inner, outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncResultEnumerable<TOuter, TFailure> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> DoGroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector).AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncResultEnumerable<TOuter, TFailure> outer, Result<IAsyncEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.GroupJoin(success, outerKeySelector, innerKeySelector, resultSelector),
					failure => outer
						.Where(result => result.Match(_ => false, f => true))
						.Select(result => result.Match(_ => default, Result.Failure<TResult, TFailure>))
						.Concat(new[] { Result.Failure<TResult, TFailure>(failure) })
						.AsAsyncResultEnumerable()
				);

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncResultEnumerable<TOuter, TFailure> outer, Result<TInner[], TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> outer.GroupJoin(inner.Select(arr => arr.AsEnumerable()), outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncResultEnumerable<TOuter, TFailure> outer, Task<Result<TInner[], TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> outer.GroupJoin(inner.Select(arr => arr.AsEnumerable()), outerKeySelector, innerKeySelector, resultSelector);

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncEnumerable<TOuter> outer, Result<IEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.GroupJoin(success, outerKeySelector, innerKeySelector, resultSelector).Select(Result.Success<TResult, TFailure>),
					failure => new[] { Result.Failure<TResult, TFailure>(failure) }.AsAsyncEnumerable()
				)
				.AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncEnumerable<TOuter> outer, Task<Result<IEnumerable<TInner>, TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.GroupJoin(success, outerKeySelector, innerKeySelector, resultSelector).Select(Result.Success<TResult, TFailure>),
					failure => new[] { Result.Failure<TResult, TFailure>(failure) }.AsAsyncEnumerable()
				)
				.AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncEnumerable<TOuter> outer, Result<IAsyncEnumerable<TInner>, TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
				=> inner
					.Match
					(
						success => outer.GroupJoin(success, outerKeySelector, innerKeySelector, resultSelector).Select(Result.Success<TResult, TFailure>),
						failure => new[] { Result.Failure<TResult, TFailure>(failure) }.AsAsyncEnumerable()
					)
					.AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncEnumerable<TOuter> outer, Result<TInner[], TFailure> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.GroupJoin(success, outerKeySelector, innerKeySelector, resultSelector).Select(Result.Success<TResult, TFailure>),
					failure => new[] { Result.Failure<TResult, TFailure>(failure) }.AsAsyncEnumerable()
				)
				.AsAsyncResultEnumerable();

		public static IAsyncResultEnumerable<TResult, TFailure> GroupJoin<TOuter, TInner, TKey, TResult, TFailure>(this IAsyncEnumerable<TOuter> outer, Task<Result<TInner[], TFailure>> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
			=> inner
				.Match
				(
					success => outer.GroupJoin(success, outerKeySelector, innerKeySelector, resultSelector).Select(Result.Success<TResult, TFailure>),
					failure => new[] { Result.Failure<TResult, TFailure>(failure) }.AsAsyncEnumerable()
				)
				.AsAsyncResultEnumerable();
	}
}