﻿namespace Functional;

[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class ResultExtensions
{
	private static class DelegateCache<T>
	{
		public static readonly Func<T, bool> True = _ => true;
		public static readonly Func<T, bool> False = _ => false;
		public static readonly Func<T, T> Passthrough = _ => _;
	}

#pragma warning disable CS8603 // Possible null reference return.
	private static class DelegateCache<TIn, TOut>
	{
		public static readonly Func<TIn, TOut> Default = _ => default;
	}
#pragma warning restore CS8603 // Possible null reference return.

	public static bool TryGetValue<TSuccess, TFailure>(this Result<TSuccess, TFailure> result, [MaybeNullWhen(false)] out TSuccess success, [MaybeNullWhen(true)] out TFailure failure)
		where TSuccess : notnull
		where TFailure : notnull
	{
		success = result.Match(DelegateCache<TSuccess>.Passthrough, DelegateCache<TFailure, TSuccess>.Default);

		failure = result.Match(DelegateCache<TSuccess, TFailure>.Default, DelegateCache<TFailure>.Passthrough);

		return result.Match(DelegateCache<TSuccess>.True, DelegateCache<TFailure>.False);
	}

	public static bool IsSuccess<TSuccess, TFailure>(this Result<TSuccess, TFailure> result)
		where TSuccess : notnull
		where TFailure : notnull
		=> result.TryGetValue(out var success, out var failure) ? true : false;

	public static async Task<bool> IsSuccess<TSuccess, TFailure>(this Task<Result<TSuccess, TFailure>> result)
		where TSuccess : notnull
		where TFailure : notnull
		=> (await result).IsSuccess();

	public static Option<TSuccess> Success<TSuccess, TFailure>(this Result<TSuccess, TFailure> result)
		where TSuccess : notnull
		where TFailure : notnull
		=> result.TryGetValue(out var success, out var failure) ? Option.Some(success) : Option.None<TSuccess>();

	public static async Task<Option<TSuccess>> Success<TSuccess, TFailure>(this Task<Result<TSuccess, TFailure>> result)
		where TSuccess : notnull
		where TFailure : notnull
		=> (await result).Success();

	public static Option<TFailure> Failure<TSuccess, TFailure>(this Result<TSuccess, TFailure> result)
		where TSuccess : notnull
		where TFailure : notnull
		=> result.TryGetValue(out var success, out var failure) ? Option.None<TFailure>() : Option.Some(failure);

	public static async Task<Option<TFailure>> Failure<TSuccess, TFailure>(this Task<Result<TSuccess, TFailure>> result)
		where TSuccess : notnull
		where TFailure : notnull
		=> (await result).Failure();

	public static Result<TResult, TFailure> Map<TSuccess, TFailure, TResult>(this Result<TSuccess, TFailure> result, Func<TSuccess, TResult> map)
		where TSuccess : notnull
		where TFailure : notnull
		where TResult : notnull
	{
		if (map == null)
			throw new ArgumentNullException(nameof(map));

		if (result.TryGetValue(out var success, out var failure))
			return Result.Success<TResult, TFailure>(map.Invoke(success));

		return Result.Failure<TResult, TFailure>(failure);
	}

	public static async Task<Result<TResult, TFailure>> Map<TSuccess, TFailure, TResult>(this Task<Result<TSuccess, TFailure>> result, Func<TSuccess, TResult> map)
		where TSuccess : notnull
		where TFailure : notnull
		where TResult : notnull
		=> (await result).Map(map);

	public static Result<TSuccess, TFailure> Where<TSuccess, TFailure>(this Result<TSuccess, TFailure> result, Func<TSuccess, bool> predicate, Func<TSuccess, TFailure> failureFactory)
		where TSuccess : notnull
		where TFailure : notnull
	{
		if (predicate == null)
			throw new ArgumentNullException(nameof(predicate));

		if (failureFactory == null)
			throw new ArgumentNullException(nameof(failureFactory));

		if (result.TryGetValue(out var success, out var failure))
			return predicate.Invoke(success) ? Result.Success<TSuccess, TFailure>(success) : Result.Failure<TSuccess, TFailure>(failureFactory.Invoke(success));

		return Result.Failure<TSuccess, TFailure>(failure);
	}

	public static async Task<Result<TSuccess, TFailure>> Where<TSuccess, TFailure>(this Task<Result<TSuccess, TFailure>> result, Func<TSuccess, bool> predicate, Func<TSuccess, TFailure> failureFactory)
		where TSuccess : notnull
		where TFailure : notnull
		=> (await result).Where(predicate, failureFactory);

	public static Result<TSuccess, TResult> MapOnFailure<TSuccess, TFailure, TResult>(this Result<TSuccess, TFailure> result, Func<TFailure, TResult> mapFailure)
		where TSuccess : notnull
		where TFailure : notnull
		where TResult : notnull
	{
		if (mapFailure == null)
			throw new ArgumentNullException(nameof(mapFailure));

		if (result.TryGetValue(out var success, out var failure))
			return Result.Success<TSuccess, TResult>(success);

		return Result.Failure<TSuccess, TResult>(mapFailure.Invoke(failure));
	}

	public static async Task<Result<TSuccess, TResult>> MapOnFailure<TSuccess, TFailure, TResult>(this Task<Result<TSuccess, TFailure>> result, Func<TFailure, TResult> mapFailure)
		where TSuccess : notnull
		where TFailure : notnull
		where TResult : notnull
		=> (await result).MapOnFailure(mapFailure);

	public static Result<TResult, TFailure> Bind<TSuccess, TFailure, TResult>(this Result<TSuccess, TFailure> result, Func<TSuccess, Result<TResult, TFailure>> bind)
		where TSuccess : notnull
		where TFailure : notnull
		where TResult : notnull
	{
		if (bind == null)
			throw new ArgumentNullException(nameof(bind));

		if (result.TryGetValue(out var success, out var failure))
			return bind.Invoke(success);
		
		return Result.Failure<TResult, TFailure>(failure);
	}

	public static async Task<Result<TResult, TFailure>> Bind<TSuccess, TFailure, TResult>(this Task<Result<TSuccess, TFailure>> result, Func<TSuccess, Result<TResult, TFailure>> bind)
		where TSuccess : notnull
		where TFailure : notnull
		where TResult : notnull
		=> (await result).Bind(bind);

	public static Result<TSuccess, TResult> BindOnFailure<TSuccess, TFailure, TResult>(this Result<TSuccess, TFailure> result, Func<TFailure, Result<TSuccess, TResult>> bind)
		where TSuccess : notnull
		where TFailure : notnull
		where TResult : notnull
	{
		if (bind == null) throw new ArgumentNullException(nameof(bind));

		return !result.TryGetValue(out var success, out var failure)
			? bind.Invoke(failure)
			: Result.Success<TSuccess, TResult>(success);
	}

	public static async Task<Result<TSuccess, TResult>> BindOnFailure<TSuccess, TFailure, TResult>(this Task<Result<TSuccess, TFailure>> result, Func<TFailure, Result<TSuccess, TResult>> bind)
		where TSuccess : notnull
		where TFailure : notnull
		where TResult : notnull
		=> (await result).BindOnFailure(bind);

	public static Result<TSuccess, TFailure> Do<TSuccess, TFailure>(this Result<TSuccess, TFailure> result, Action<TSuccess> onSuccess, Action<TFailure> onFailure)
		where TSuccess : notnull
		where TFailure : notnull
	{
		if (onSuccess == null)
			throw new ArgumentNullException(nameof(onSuccess));

		if (onFailure == null)
			throw new ArgumentNullException(nameof(onFailure));

		if (result.TryGetValue(out var success, out var failure))
			onSuccess.Invoke(success);
		else
			onFailure.Invoke(failure);

		return result;
	}

	public static async Task<Result<TSuccess, TFailure>> Do<TSuccess, TFailure>(this Task<Result<TSuccess, TFailure>> result, Action<TSuccess> success, Action<TFailure> failure)
		where TSuccess : notnull
		where TFailure : notnull
		=> (await result).Do(success, failure);

	public static Result<TSuccess, TFailure> Do<TSuccess, TFailure>(this Result<TSuccess, TFailure> result, Action<TSuccess> onSuccess)
		where TSuccess : notnull
		where TFailure : notnull
		=> result.Do(onSuccess, static _ => { });

	public static async Task<Result<TSuccess, TFailure>> Do<TSuccess, TFailure>(this Task<Result<TSuccess, TFailure>> result, Action<TSuccess> onSuccess)
		where TSuccess : notnull
		where TFailure : notnull
		=> (await result).Do(onSuccess);

	public static Result<TSuccess, TFailure> DoOnFailure<TSuccess, TFailure>(this Result<TSuccess, TFailure> result, Action<TFailure> onFailure)
		where TSuccess : notnull
		where TFailure : notnull
		=> result.Do(static _ => { }, onFailure);

	public static async Task<Result<TSuccess, TFailure>> DoOnFailure<TSuccess, TFailure>(this Task<Result<TSuccess, TFailure>> result, Action<TFailure> onFailure)
		where TSuccess : notnull
		where TFailure : notnull
		=> (await result).DoOnFailure(onFailure);

	public static void Apply<TSuccess, TFailure>(this Result<TSuccess, TFailure> result, Action<TSuccess> onSuccess, Action<TFailure> onFailure)
		where TSuccess : notnull
		where TFailure : notnull
		=> result.Do(onSuccess, onFailure);

	public static Task Apply<TSuccess, TFailure>(this Task<Result<TSuccess, TFailure>> result, Action<TSuccess> onSuccess, Action<TFailure> onFailure)
		where TSuccess : notnull
		where TFailure : notnull
		=> result.Do(onSuccess, onFailure);

	public static Result<TResult, TFailure> TryMap<TSuccess, TResult, TFailure>(this Result<TSuccess, TFailure> result, Func<TSuccess, TResult> successFactory, Func<Exception, TFailure> failureFactory)
		where TSuccess : notnull
		where TFailure : notnull
		where TResult : notnull
	{
		if (successFactory == null)
			throw new ArgumentNullException(nameof(successFactory));

		if(failureFactory == null)
			throw new ArgumentNullException(nameof(failureFactory));

		if (result.TryGetValue(out var success, out var failure))
		{
			try
			{
				return Result.Success<TResult, TFailure>(successFactory.Invoke(success));
			}
			catch (Exception ex)
			{
				return Result.Failure<TResult, TFailure>(failureFactory.Invoke(ex));
			}
		}

		return Result.Failure<TResult, TFailure>(failure);
	}

	public static Result<TResult, Exception> TryMap<TSuccess, TResult>(this Result<TSuccess, Exception> result, Func<TSuccess, TResult> successFactory)
		where TSuccess : notnull
		where TResult : notnull
		=> TryMap(result, successFactory, static _ => _);

	public static async Task<Result<TResult, TFailure>> TryMap<TSuccess, TResult, TFailure>(this Task<Result<TSuccess, TFailure>> result, Func<TSuccess, TResult> successFactory, Func<Exception, TFailure> failureFactory)
		where TSuccess : notnull
		where TFailure : notnull
		where TResult : notnull
		=> (await result).TryMap(successFactory, failureFactory);

	public static async Task<Result<TResult, Exception>> TryMap<TSuccess, TResult>(this Task<Result<TSuccess, Exception>> result, Func<TSuccess, TResult> successFactory)
		where TSuccess : notnull
		where TResult : notnull
		=> (await result).TryMap(successFactory, static _ => _);

	public static Result<T2, T1> Transpose<T1, T2>(this Result<T1, T2> source)
		where T1 : notnull
		where T2 : notnull
		=> source.TryGetValue(out var success, out var failure) ? Result.Failure<T2, T1>(success) : Result.Success<T2, T1>(failure);

	public static async Task<Result<T2, T1>> Transpose<T1, T2>(this Task<Result<T1, T2>> source)
		where T1 : notnull
		where T2 : notnull
		=> (await source).Transpose();

	public static TSuccess ThrowOnFailure<TSuccess, TFailure>(this Result<TSuccess, TFailure> result, Func<TFailure, Exception> exceptionFactory)
		where TSuccess : notnull
		where TFailure : notnull
		=> result.TryGetValue(out var success, out var failure) ? success : throw exceptionFactory.Invoke(failure);

	public static async Task<TSuccess> ThrowOnFailure<TSuccess, TFailure>(this Task<Result<TSuccess, TFailure>> result, Func<TFailure, Exception> exceptionFactory)
		where TSuccess : notnull
		where TFailure : notnull
		=> (await result).ThrowOnFailure(exceptionFactory);
}
