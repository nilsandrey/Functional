﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functional
{
	public static partial class OptionQuerySyntax
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IOptionEnumerable<TValue> Where<TValue>(this IEnumerable<TValue> source, Func<TValue, Option<Unit>> failurePredicate)
			where TValue : notnull
			=> source
				.Select(success => failurePredicate
					.Invoke(success)
					.Map(_ => success)
				)
				.AsOptionEnumerable();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IAsyncOptionEnumerable<TValue> Where<TValue>(this IEnumerable<TValue> source, Func<TValue, Task<Option<Unit>>> failurePredicate)
			where TValue : notnull
			=> source
				.SelectAsync(success => failurePredicate
					.Invoke(success)
					.Map(_ => success)
				)
				.AsAsyncOptionEnumerable();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IAsyncOptionEnumerable<TValue> Where<TValue>(this IAsyncEnumerable<TValue> source, Func<TValue, Option<Unit>> failurePredicate)
			where TValue : notnull
			=> source
				.Select(success => failurePredicate
					.Invoke(success)
					.Map(_ => success)
				)
				.AsAsyncOptionEnumerable();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IAsyncOptionEnumerable<TValue> Where<TValue>(this IAsyncEnumerable<TValue> source, Func<TValue, Task<Option<Unit>>> failurePredicate)
			where TValue : notnull
			=> source
				.SelectAsync(success => failurePredicate
					.Invoke(success)
					.Map(_ => success)
				)
				.AsAsyncOptionEnumerable();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IOptionEnumerable<TValue> Where<TValue>(this IOptionEnumerable<TValue> source, Func<TValue, Option<Unit>> failurePredicate)
			where TValue : notnull
			=> source
				.Select(value => value
					.Bind(success => failurePredicate
						.Invoke(success)
						.Map(_ => success)
					)
				)
				.AsOptionEnumerable();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IAsyncOptionEnumerable<TValue> Where<TValue>(this IOptionEnumerable<TValue> source, Func<TValue, Task<Option<Unit>>> failurePredicate)
			where TValue : notnull
			=> source
				.SelectAsync(value => value
					.BindAsync(success => failurePredicate
						.Invoke(success)
						.Map(_ => success)
					)
				)
				.AsAsyncOptionEnumerable();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IAsyncOptionEnumerable<TValue> Where<TValue>(this IAsyncOptionEnumerable<TValue> source, Func<TValue, Option<Unit>> failurePredicate)
			where TValue : notnull
			=> source
				.Select(value => value
					.Bind(success => failurePredicate
						.Invoke(success)
						.Map(_ => success)
					)
				)
				.AsAsyncOptionEnumerable();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IAsyncOptionEnumerable<TValue> Where<TValue>(this IAsyncOptionEnumerable<TValue> source, Func<TValue, Task<Option<Unit>>> failurePredicate)
			where TValue : notnull
			=> source
				.SelectAsync(value => value
					.BindAsync(success => failurePredicate
						.Invoke(success)
						.Map(_ => success)
					)
				)
				.AsAsyncOptionEnumerable();
	}
}
