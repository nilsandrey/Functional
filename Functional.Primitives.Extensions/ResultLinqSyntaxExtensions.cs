﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Functional
{
    public static class ResultLinqSyntaxExtensions
    {
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Result<TResult, TFailure> SelectMany<TSuccess, TFailure, TResult>(this Result<TSuccess, TFailure> result, Func<TSuccess, Result<TResult, TFailure>> bind)
			=> result.Match(bind, failure => Result.Failure<TResult, TFailure>(failure));

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static async Task<Result<TResult, TFailure>> SelectMany<TSuccess, TFailure, TResult>(this Task<Result<TSuccess, TFailure>> result, Func<TSuccess, Result<TResult, TFailure>> bind)
			=> (await result).SelectMany(bind);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Result<TResult, TFailure> SelectMany<TSuccess, TFailure, TBind, TResult>(this Result<TSuccess, TFailure> result, Func<TSuccess, Result<TBind, TFailure>> bind, Func<TSuccess, TBind, TResult> resultSelector)
		{
			if (bind == null)
				throw new ArgumentNullException(nameof(bind));

			if (resultSelector == null)
				throw new ArgumentNullException(nameof(resultSelector));

			return result.Bind(value => bind
				.Invoke(value)
				.Select(obj => resultSelector.Invoke(value, obj))
			);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static async Task<Result<TResult, TFailure>> SelectMany<TSuccess, TFailure, TBind, TResult>(this Task<Result<TSuccess, TFailure>> result, Func<TSuccess, Result<TBind, TFailure>> bind, Func<TSuccess, TBind, TResult> resultSelector)
			=> (await result).SelectMany(bind, resultSelector);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Task<Result<TResult, TFailure>> SelectMany<TSuccess, TFailure, TBind, TResult>(this Result<TSuccess, TFailure> result, Func<TSuccess, Task<Result<TBind, TFailure>>> bind, Func<TSuccess, TBind, TResult> resultSelector)
		{
			if (bind == null)
				throw new ArgumentNullException(nameof(bind));

			if (resultSelector == null)
				throw new ArgumentNullException(nameof(resultSelector));

			return result.BindAsync(value => bind
				.Invoke(value)
				.Select(obj => resultSelector.Invoke(value, obj))
			);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static async Task<Result<TResult, TFailure>> SelectMany<TSuccess, TFailure, TBind, TResult>(this Task<Result<TSuccess, TFailure>> result, Func<TSuccess, Task<Result<TBind, TFailure>>> bind, Func<TSuccess, TBind, TResult> resultSelector)
			=> await (await result).SelectMany(bind, resultSelector);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Task<Result<TResult, TFailure>> SelectMany<TSuccess, TFailure, TResult>(this Result<TSuccess, TFailure> result, Func<TSuccess, Task<Result<TResult, TFailure>>> bind)
			=> result.MatchAsync(bind, failure => Task.FromResult(Result.Failure<TResult, TFailure>(failure)));

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static async Task<Result<TResult, TFailure>> SelectMany<TSuccess, TFailure, TResult>(this Task<Result<TSuccess, TFailure>> result, Func<TSuccess, Task<Result<TResult, TFailure>>> bind)
			=> await (await result).SelectMany(bind);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Task<Result<TResult, TFailure>> SelectMany<TSuccess, TFailure, TBind, TResult>(this Result<TSuccess, TFailure> result, Func<TSuccess, Result<TBind, TFailure>> bind, Func<TSuccess, TBind, Task<TResult>> resultSelector)
		{
			if (bind == null)
				throw new ArgumentNullException(nameof(bind));

			if (resultSelector == null)
				throw new ArgumentNullException(nameof(resultSelector));

			return result.BindAsync(value => bind
				.Invoke(value)
				.SelectAsync(obj => resultSelector.Invoke(value, obj))
			);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static async Task<Result<TResult, TFailure>> SelectMany<TSuccess, TFailure, TBind, TResult>(this Task<Result<TSuccess, TFailure>> result, Func<TSuccess, Result<TBind, TFailure>> bind, Func<TSuccess, TBind, Task<TResult>> resultSelector)
			=> await (await result).SelectMany(bind, resultSelector);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Task<Result<TResult, TFailure>> SelectMany<TSuccess, TFailure, TBind, TResult>(this Result<TSuccess, TFailure> result, Func<TSuccess, Task<Result<TBind, TFailure>>> bind, Func<TSuccess, TBind, Task<TResult>> resultSelector)
		{
			if (bind == null)
				throw new ArgumentNullException(nameof(bind));

			if (resultSelector == null)
				throw new ArgumentNullException(nameof(resultSelector));

			return result.BindAsync(value => bind
				.Invoke(value)
				.SelectAsync(obj => resultSelector.Invoke(value, obj))
			);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static async Task<Result<TResult, TFailure>> SelectMany<TSuccess, TFailure, TBind, TResult>(this Task<Result<TSuccess, TFailure>> result, Func<TSuccess, Task<Result<TBind, TFailure>>> bind, Func<TSuccess, TBind, Task<TResult>> resultSelector)
			=> await (await result).SelectMany(bind, resultSelector);
	}
}
