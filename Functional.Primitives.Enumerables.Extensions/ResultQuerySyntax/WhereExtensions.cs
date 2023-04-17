﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functional
{
	public static partial class ResultQuerySyntax
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IResultEnumerable<TSuccess, TFailure> Where<TSuccess, TFailure>(this IEnumerable<TSuccess> source, Func<TSuccess, Result<Unit, TFailure>> failurePredicate)
			where TSuccess : notnull
			where TFailure : notnull
			=> source
				.Select(success => failurePredicate
					.Invoke(success)
					.Map(_ => success)
				)
				.AsResultEnumerable();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IAsyncResultEnumerable<TSuccess, TFailure> Where<TSuccess, TFailure>(this IEnumerable<TSuccess> source, Func<TSuccess, Task<Result<Unit, TFailure>>> failurePredicate)
			where TSuccess : notnull
			where TFailure : notnull
			=> source
				.SelectAsync(success => failurePredicate
					.Invoke(success)
					.Map(_ => success)
				)
				.AsAsyncResultEnumerable();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IAsyncResultEnumerable<TSuccess, TFailure> Where<TSuccess, TFailure>(this IAsyncEnumerable<TSuccess> source, Func<TSuccess, Result<Unit, TFailure>> failurePredicate)
			where TSuccess : notnull
			where TFailure : notnull
			=> source
				.Select(success => failurePredicate
					.Invoke(success)
					.Map(_ => success)
				)
				.AsAsyncResultEnumerable();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IAsyncResultEnumerable<TSuccess, TFailure> Where<TSuccess, TFailure>(this IAsyncEnumerable<TSuccess> source, Func<TSuccess, Task<Result<Unit, TFailure>>> failurePredicate)
			where TSuccess : notnull
			where TFailure : notnull
			=> source
				.SelectAsync(success => failurePredicate
					.Invoke(success)
					.Map(_ => success)
				)
				.AsAsyncResultEnumerable();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IResultEnumerable<TSuccess, TFailure> Where<TSuccess, TFailure>(this IResultEnumerable<TSuccess, TFailure> source, Func<TSuccess, Result<Unit, TFailure>> failurePredicate)
			where TSuccess : notnull
			where TFailure : notnull
			=> source
				.Select(value => value
					.Bind(success => failurePredicate
						.Invoke(success)
						.Map(_ => success)
					)
				)
				.AsResultEnumerable();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IAsyncResultEnumerable<TSuccess, TFailure> Where<TSuccess, TFailure>(this IResultEnumerable<TSuccess, TFailure> source, Func<TSuccess, Task<Result<Unit, TFailure>>> failurePredicate)
			where TSuccess : notnull
			where TFailure : notnull
			=> source
				.SelectAsync(value => value
					.BindAsync(success => failurePredicate
						.Invoke(success)
						.Map(_ => success)
					)
				)
				.AsAsyncResultEnumerable();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IAsyncResultEnumerable<TSuccess, TFailure> Where<TSuccess, TFailure>(this IAsyncResultEnumerable<TSuccess, TFailure> source, Func<TSuccess, Result<Unit, TFailure>> failurePredicate)
			where TSuccess : notnull
			where TFailure : notnull
			=> source
				.Select(value => value
					.Bind(success => failurePredicate
						.Invoke(success)
						.Map(_ => success)
					)
				)
				.AsAsyncResultEnumerable();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IAsyncResultEnumerable<TSuccess, TFailure> Where<TSuccess, TFailure>(this IAsyncResultEnumerable<TSuccess, TFailure> source, Func<TSuccess, Task<Result<Unit, TFailure>>> failurePredicate)
			where TSuccess : notnull
			where TFailure : notnull
			=> source
				.SelectAsync(value => value
					.BindAsync(success => failurePredicate
						.Invoke(success)
						.Map(_ => success)
					)
				)
				.AsAsyncResultEnumerable();
	}
}
