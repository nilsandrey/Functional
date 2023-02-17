﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functional
{
	public static class Result
	{
		public static PartialResult.Success<TSuccess> Success<TSuccess>(TSuccess success)
			where TSuccess : notnull
			=> new PartialResult.Success<TSuccess>(success);

		/// <summary>
		/// Create a new <see cref="Result{TSuccess,TFailure}"/> holding a success value.
		/// </summary>
		/// <typeparam name="TSuccess">The success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="success">The success value.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static Result<TSuccess, TFailure> Success<TSuccess, TFailure>(TSuccess success)
			where TSuccess : notnull
			where TFailure : notnull
		{
			if (success == null)
				throw new ArgumentNullException(nameof(success));

#pragma warning disable CS8604 // Possible null reference argument.
			return new Result<TSuccess, TFailure>(true, success, default);
#pragma warning restore CS8604 // Possible null reference argument.
		}

		[Obsolete("Please use .SuccessAsync() instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Task<Result<TSuccess, TFailure>> Success<TSuccess, TFailure>(Task<TSuccess> success)
			where TSuccess : notnull
			where TFailure : notnull
			=> SuccessAsync<TSuccess, TFailure>(success);

		/// <summary>
		/// Create a new <see cref="Result{TSuccess,TFailure}"/> holding a success value.
		/// </summary>
		/// <typeparam name="TSuccess">The success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="success">The success value.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static async Task<Result<TSuccess, TFailure>> SuccessAsync<TSuccess, TFailure>(Task<TSuccess> success)
			where TSuccess : notnull
			where TFailure : notnull
			=> Success<TSuccess, TFailure>(await success);

		public static PartialResult.Failure<TFailure> Failure<TFailure>(TFailure failure)
			where TFailure : notnull
			=> new PartialResult.Failure<TFailure>(failure);

		/// <summary>
		/// Create a new <see cref="Result{TSuccess,TFailure}"/> holding a failure value.
		/// </summary>
		/// <typeparam name="TSuccess">The success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="failure">The failure value.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static Result<TSuccess, TFailure> Failure<TSuccess, TFailure>(TFailure failure)
			where TSuccess : notnull
			where TFailure : notnull
		{
			if (failure == null)
				throw new ArgumentNullException(nameof(failure));

#pragma warning disable CS8604 // Possible null reference argument.
			return new Result<TSuccess, TFailure>(false, default, failure);
#pragma warning restore CS8604 // Possible null reference argument.
		}

		[Obsolete("Please use .FailureAsync() instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Task<Result<TSuccess, TFailure>> Failure<TSuccess, TFailure>(Task<TFailure> failure)
			where TSuccess : notnull
			where TFailure : notnull
			=> FailureAsync<TSuccess, TFailure>(failure);

		/// <summary>
		/// Create a new <see cref="Result{TSuccess,TFailure}"/> holding a failure value.
		/// </summary>
		/// <typeparam name="TSuccess">The success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="failure">The failure value.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static async Task<Result<TSuccess, TFailure>> FailureAsync<TSuccess, TFailure>(Task<TFailure> failure)
			where TSuccess : notnull
			where TFailure : notnull
			=> Failure<TSuccess, TFailure>(await failure);

		/// <summary>
		/// Create a new <see cref="Result{TSuccess,TFailure}"/>.
		/// </summary>
		/// <typeparam name="TSuccess">The success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="isSuccess">If <see langword="true"/>, hold <paramref name="success"/>; otherwise, hold <paramref name="failure"/>.</param>
		/// <param name="success">The success value.</param>
		/// <param name="failure">The failure value.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static Result<TSuccess, TFailure> Create<TSuccess, TFailure>(bool isSuccess, TSuccess success, TFailure failure)
			where TSuccess : notnull
			where TFailure : notnull
			=> isSuccess
				? Success<TSuccess, TFailure>(success)
				: Failure<TSuccess, TFailure>(failure);

		/// <summary>
		/// Create a new <see cref="Result{TSuccess,TFailure}"/>.
		/// </summary>
		/// <typeparam name="TSuccess">The success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="isSuccess">If <see langword="true"/>, hold <paramref name="success"/>; otherwise, hold the value produced by <paramref name="failureFactory"/>.</param>
		/// <param name="success">The success value.</param>
		/// <param name="failureFactory">The function producing the failure value.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static Result<TSuccess, TFailure> Create<TSuccess, TFailure>(bool isSuccess, TSuccess success, Func<TFailure> failureFactory)
			where TSuccess : notnull
			where TFailure : notnull
		{
			if (failureFactory == null)
				throw new ArgumentNullException(nameof(failureFactory));

			return isSuccess
				? Success<TSuccess, TFailure>(success)
				: Failure<TSuccess, TFailure>(failureFactory.Invoke());
		}

		/// <summary>
		/// Create a new <see cref="Result{TSuccess,TFailure}"/>.
		/// </summary>
		/// <typeparam name="TSuccess">The success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="isSuccess">If <see langword="true"/>, hold <paramref name="success"/>; otherwise, hold the value produced by <paramref name="failureFactory"/>.</param>
		/// <param name="success">The success value.</param>
		/// <param name="failureFactory">The function producing the failure value.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static async Task<Result<TSuccess, TFailure>> CreateAsync<TSuccess, TFailure>(bool isSuccess, TSuccess success, Func<Task<TFailure>> failureFactory)
			where TSuccess : notnull
			where TFailure : notnull
		{
			if (failureFactory == null)
				throw new ArgumentNullException(nameof(failureFactory));

			return isSuccess
				? Success<TSuccess, TFailure>(success)
				: Failure<TSuccess, TFailure>(await failureFactory.Invoke());
		}

		/// <summary>
		/// Create a new <see cref="Result{TSuccess,TFailure}"/>.
		/// </summary>
		/// <typeparam name="TSuccess">The success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="isSuccess">If <see langword="true"/>, hold the value produced by <paramref name="successFactory"/>; otherwise, hold <paramref name="failure"/>.</param>
		/// <param name="successFactory">The function producing the success value.</param>
		/// <param name="failure">The failure value.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static Result<TSuccess, TFailure> Create<TSuccess, TFailure>(bool isSuccess, Func<TSuccess> successFactory, TFailure failure)
			where TSuccess : notnull
			where TFailure : notnull
		{
			if (successFactory == null)
				throw new ArgumentNullException(nameof(successFactory));

			return isSuccess
				? Success<TSuccess, TFailure>(successFactory.Invoke())
				: Failure<TSuccess, TFailure>(failure);
		}

		/// <summary>
		/// Create a new <see cref="Result{TSuccess,TFailure}"/>.
		/// </summary>
		/// <typeparam name="TSuccess">The success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="isSuccess">If <see langword="true"/>, hold the value produced by <paramref name="successFactory"/>; otherwise, hold the value produced by <paramref name="failureFactory"/>.</param>
		/// <param name="successFactory">The function producing the success value.</param>
		/// <param name="failureFactory">The function producing the failure value.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static Result<TSuccess, TFailure> Create<TSuccess, TFailure>(bool isSuccess, Func<TSuccess> successFactory, Func<TFailure> failureFactory)
			where TSuccess : notnull
			where TFailure : notnull
		{
			if (successFactory == null)
				throw new ArgumentNullException(nameof(successFactory));

			if (failureFactory == null)
				throw new ArgumentNullException(nameof(failureFactory));

			return isSuccess
				? Success<TSuccess, TFailure>(successFactory.Invoke())
				: Failure<TSuccess, TFailure>(failureFactory.Invoke());
		}

		/// <summary>
		/// Create a new <see cref="Result{TSuccess,TFailure}"/>.
		/// </summary>
		/// <typeparam name="TSuccess">The success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="isSuccess">If <see langword="true"/>, hold the value produced by <paramref name="successFactory"/>; otherwise, hold the value produced by <paramref name="failureFactory"/>.</param>
		/// <param name="successFactory">The function producing the success value.</param>
		/// <param name="failureFactory">The function producing the failure value.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static async Task<Result<TSuccess, TFailure>> CreateAsync<TSuccess, TFailure>(bool isSuccess, Func<TSuccess> successFactory, Func<Task<TFailure>> failureFactory)
			where TSuccess : notnull
			where TFailure : notnull
		{
			if (successFactory == null)
				throw new ArgumentNullException(nameof(successFactory));

			if (failureFactory == null)
				throw new ArgumentNullException(nameof(failureFactory));

			return isSuccess
				? Success<TSuccess, TFailure>(successFactory.Invoke())
				: Failure<TSuccess, TFailure>(await failureFactory.Invoke());
		}

		/// <summary>
		/// Create a new <see cref="Result{TSuccess,TFailure}"/>.
		/// </summary>
		/// <typeparam name="TSuccess">The success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="isSuccess">If <see langword="true"/>, hold the value produced by <paramref name="successFactory"/>; otherwise, hold <paramref name="failure"/>.</param>
		/// <param name="successFactory">The function producing the success value.</param>
		/// <param name="failure">The failure value.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static async Task<Result<TSuccess, TFailure>> CreateAsync<TSuccess, TFailure>(bool isSuccess, Func<Task<TSuccess>> successFactory, TFailure failure)
			where TSuccess : notnull
			where TFailure : notnull
		{
			if (successFactory == null)
				throw new ArgumentNullException(nameof(successFactory));

			return isSuccess
				? Success<TSuccess, TFailure>(await successFactory.Invoke())
				: Failure<TSuccess, TFailure>(failure);
		}

		/// <summary>
		/// Create a new <see cref="Result{TSuccess,TFailure}"/>.
		/// </summary>
		/// <typeparam name="TSuccess">The success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="isSuccess">If <see langword="true"/>, hold the value produced by <paramref name="successFactory"/>; otherwise, hold the value produced by <paramref name="failureFactory"/>.</param>
		/// <param name="successFactory">The function producing the success value.</param>
		/// <param name="failureFactory">The function producing the failure value.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static async Task<Result<TSuccess, TFailure>> CreateAsync<TSuccess, TFailure>(bool isSuccess, Func<Task<TSuccess>> successFactory, Func<TFailure> failureFactory)
			where TSuccess : notnull
			where TFailure : notnull
		{
			if (successFactory == null)
				throw new ArgumentNullException(nameof(successFactory));

			if (failureFactory == null)
				throw new ArgumentNullException(nameof(failureFactory));

			return isSuccess
				? Success<TSuccess, TFailure>(await successFactory.Invoke())
				: Failure<TSuccess, TFailure>(failureFactory.Invoke());
		}

		/// <summary>
		/// Create a new <see cref="Result{TSuccess,TFailure}"/>.
		/// </summary>
		/// <typeparam name="TSuccess">The success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="isSuccess">If <see langword="true"/>, hold the value produced by <paramref name="successFactory"/>; otherwise, hold the value produced by <paramref name="failureFactory"/>.</param>
		/// <param name="successFactory">The function producing the success value.</param>
		/// <param name="failureFactory">The function producing the failure value.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static async Task<Result<TSuccess, TFailure>> CreateAsync<TSuccess, TFailure>(bool isSuccess, Func<Task<TSuccess>> successFactory, Func<Task<TFailure>> failureFactory)
			where TSuccess : notnull
			where TFailure : notnull
		{
			if (successFactory == null)
				throw new ArgumentNullException(nameof(successFactory));

			if (failureFactory == null)
				throw new ArgumentNullException(nameof(failureFactory));

			return isSuccess
				? Success<TSuccess, TFailure>(await successFactory.Invoke())
				: Failure<TSuccess, TFailure>(await failureFactory.Invoke());
		}

		[Obsolete("Please use .CreateAsync() instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Task<Result<TSuccess, TFailure>> Create<TSuccess, TFailure>(bool isSuccess, Func<Task<TSuccess>> successFactory, Func<Task<TFailure>> failureFactory)
			where TSuccess : notnull
			where TFailure : notnull
			=> CreateAsync(isSuccess, successFactory, failureFactory);

		public static Result<TSuccess, Exception> Try<TSuccess>(Func<TSuccess> successFactory)
			where TSuccess : notnull
			=> Try<TSuccess, Exception>(successFactory);

		public static Result<Unit, Exception> Try(Action successFactory)
			=> Try<Exception>(successFactory);

		public static Result<TSuccess, TException> Try<TSuccess, TException>(Func<TSuccess> successFactory) where TException : Exception
			where TSuccess : notnull
		{
			if (successFactory == null)
				throw new ArgumentNullException(nameof(successFactory));

			try
			{
				return Success<TSuccess, TException>(successFactory.Invoke());
			}
			catch (TException ex)
			{
				return Failure<TSuccess, TException>(ex);
			}
		}

		public static Result<Unit, TException> Try<TException>(Action successFactory) where TException : Exception
		{
			if (successFactory == null)
				throw new ArgumentNullException(nameof(successFactory));

			try
			{
				successFactory.Invoke();

				return Success<Unit, TException>(Functional.Unit.Value);
			}
			catch (TException ex)
			{
				return Failure<Unit, TException>(ex);
			}
		}

		[Obsolete("Please use .TryAsync() instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Task<Result<TSuccess, Exception>> Try<TSuccess>(Func<Task<TSuccess>> successFactory)
			where TSuccess : notnull
			=> TryAsync(successFactory);

		public static async Task<Result<TSuccess, Exception>> TryAsync<TSuccess>(Func<Task<TSuccess>> successFactory)
			where TSuccess : notnull
		{
			if (successFactory == null)
				throw new ArgumentNullException(nameof(successFactory));

			try
			{
				return Success<TSuccess, Exception>(await successFactory.Invoke());
			}
			catch (Exception ex)
			{
				return Failure<TSuccess, Exception>(ex);
			}
		}

		[Obsolete("Please use .TryAsync() instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Task<Result<Unit, Exception>> Try(Func<Task> successFactory)
			=> TryAsync(successFactory);

		public static async Task<Result<Unit, Exception>> TryAsync(Func<Task> successFactory)
		{
			if (successFactory == null)
				throw new ArgumentNullException(nameof(successFactory));

			try
			{
				await successFactory.Invoke();

				return Success<Unit, Exception>(Functional.Unit.Value);
			}
			catch (Exception ex)
			{
				return Failure<Unit, Exception>(ex);
			}
		}

		public static Result<TSuccess, TFailure> Try<TSuccess, TFailure>(Func<TSuccess> successFactory, Func<Exception, TFailure> @catch)
			where TSuccess : notnull
			where TFailure : notnull
		{
			if (successFactory == null)
				throw new ArgumentNullException(nameof(successFactory));

			if (@catch == null)
				throw new ArgumentNullException(nameof(@catch));

			try
			{
				return Success<TSuccess, TFailure>(successFactory.Invoke());
			}
			catch (Exception ex)
			{
				return Failure<TSuccess, TFailure>(@catch.Invoke(ex));
			}
		}

		public static Result<Unit, TFailure> Try<TFailure>(Action successFactory, Func<Exception, TFailure> @catch)
			where TFailure : notnull
		{
			if (successFactory == null)
				throw new ArgumentNullException(nameof(successFactory));

			if (@catch == null)
				throw new ArgumentNullException(nameof(@catch));

			try
			{
				successFactory.Invoke();

				return Success<Unit, TFailure>(Functional.Unit.Value);
			}
			catch (Exception ex)
			{
				return Failure<Unit, TFailure>(@catch.Invoke(ex));
			}
		}

		[Obsolete("Please use .TryAsync() instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Task<Result<TSuccess, TFailure>> Try<TSuccess, TFailure>(Func<Task<TSuccess>> successFactory, Func<Exception, TFailure> @catch)
			where TSuccess : notnull
			where TFailure : notnull
			=> TryAsync(successFactory, @catch);

		public static async Task<Result<TSuccess, TFailure>> TryAsync<TSuccess, TFailure>(Func<Task<TSuccess>> successFactory, Func<Exception, TFailure> @catch)
			where TSuccess : notnull
			where TFailure : notnull
		{
			if (successFactory == null)
				throw new ArgumentNullException(nameof(successFactory));

			if (@catch == null)
				throw new ArgumentNullException(nameof(@catch));

			try
			{
				return Success<TSuccess, TFailure>(await successFactory.Invoke());
			}
			catch (Exception ex)
			{
				return Failure<TSuccess, TFailure>(@catch.Invoke(ex));
			}
		}

		[Obsolete("Please use .TryAsync() instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Task<Result<Unit, TFailure>> Try<TFailure>(Func<Task> successFactory, Func<Exception, TFailure> @catch)
			where TFailure : notnull
			=> TryAsync(successFactory, @catch);

		public static async Task<Result<Unit, TFailure>> TryAsync<TFailure>(Func<Task> successFactory, Func<Exception, TFailure> @catch)
			where TFailure : notnull
		{
			if (successFactory == null)
				throw new ArgumentNullException(nameof(successFactory));

			if (@catch == null)
				throw new ArgumentNullException(nameof(@catch));

			try
			{
				await successFactory.Invoke();

				return Success<Unit, TFailure>(Functional.Unit.Value);
			}
			catch (Exception ex)
			{
				return Failure<Unit, TFailure>(@catch.Invoke(ex));
			}
		}

		public static Result<Unit, TFailure> Unit<TFailure>()
			where TFailure : notnull
			=> Success<Unit, TFailure>(Functional.Unit.Value);

		public static Result<Unit, TFailure> Where<TFailure>(bool isSuccess, TFailure failure)
			where TFailure : notnull
			=> isSuccess ? Success<Unit, TFailure>(Functional.Unit.Value) : Failure<Unit, TFailure>(failure);

		public static Result<Unit, TFailure> Where<TFailure>(bool isSuccess, Func<TFailure> failureFactory)
			where TFailure : notnull
			=> isSuccess ? Success<Unit, TFailure>(Functional.Unit.Value) : Failure<Unit, TFailure>(failureFactory.Invoke());

		/// <summary>
		/// Given a set of <see cref="Result{TSuccess,TFailure}"/> with identical failure types (<typeparamref name="TFailure"/>), produce
		/// a result holding all success values from the set if all input results hold success; otherwise, produce a result holding all
		/// failure values from the set.
		/// </summary>
		/// <typeparam name="TSuccess1">The first success type.</typeparam>
		/// <typeparam name="TSuccess2">The second success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="r1">The first result.</param>
		/// <param name="r2">The second result.</param>
		/// <returns>
		/// A <see cref="Result{TSuccess,TFailure}"/> holding (<typeparamref name="TSuccess1"/>, <typeparamref name="TSuccess2"/>) if both
		/// <paramref name="r1"/> and <paramref name="r2"/> hold success values; otherwise, a result holding all failures.
		/// </returns>
		[AllowAllocations]
		public static Result<(TSuccess1, TSuccess2), TFailure[]> Zip<TSuccess1, TSuccess2, TFailure>
		(
			Result<TSuccess1, TFailure> r1,
			Result<TSuccess2, TFailure> r2
		)
			where TSuccess1 : notnull
			where TSuccess2 : notnull
			where TFailure : notnull
		{
			if (r1.IsSuccess() && r2.IsSuccess())
				return Success<(TSuccess1, TSuccess2), TFailure[]>((r1.SuccessUnsafe(), r2.SuccessUnsafe()));

			var errorCollection = new List<TFailure>(2);
			if (!r1.IsSuccess())
				errorCollection.Add(r1.FailureUnsafe());
			if (!r2.IsSuccess())
				errorCollection.Add(r2.FailureUnsafe());

			return Failure<(TSuccess1, TSuccess2), TFailure[]>(errorCollection.ToArray());
		}

		/// <summary>
		/// Given a set of <see cref="Result{TSuccess,TFailure}"/> with identical failure types (<typeparamref name="TFailure"/>), produce
		/// a result holding all success values from the set if all input results hold success; otherwise, produce a result holding all
		/// failure values from the set.
		/// </summary>
		/// <typeparam name="TSuccess1">The first success type.</typeparam>
		/// <typeparam name="TSuccess2">The second success type.</typeparam>
		/// <typeparam name="TSuccess3">The third success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="r1">The first result.</param>
		/// <param name="r2">The second result.</param>
		/// <param name="r3">The third result.</param>
		/// <returns>
		/// A <see cref="Result{TSuccess,TFailure}"/> holding (<typeparamref name="TSuccess1"/>, <typeparamref name="TSuccess2"/>, <typeparamref name="TSuccess3"/>)
		/// if all of <paramref name="r1"/>, <paramref name="r2"/>, and <paramref name="r3"/> hold success values; otherwise, a result holding all failures.
		/// </returns>
		[AllowAllocations]
		public static Result<(TSuccess1, TSuccess2, TSuccess3), TFailure[]> Zip<TSuccess1, TSuccess2, TSuccess3, TFailure>
		(
			Result<TSuccess1, TFailure> r1,
			Result<TSuccess2, TFailure> r2,
			Result<TSuccess3, TFailure> r3
		)
			where TSuccess1 : notnull
			where TSuccess2 : notnull
			where TSuccess3 : notnull
			where TFailure : notnull
		{
			if (r1.IsSuccess() && r2.IsSuccess() && r3.IsSuccess())
				return Success<(TSuccess1, TSuccess2, TSuccess3), TFailure[]>((
					r1.SuccessUnsafe(),
					r2.SuccessUnsafe(),
					r3.SuccessUnsafe()));

			var errorCollection = new List<TFailure>(3);
			if (!r1.IsSuccess())
				errorCollection.Add(r1.FailureUnsafe());
			if (!r2.IsSuccess())
				errorCollection.Add(r2.FailureUnsafe());
			if (!r3.IsSuccess())
				errorCollection.Add(r3.FailureUnsafe());

			return Failure<(TSuccess1, TSuccess2, TSuccess3), TFailure[]>(errorCollection.ToArray());
		}

		/// <summary>
		/// Given a set of <see cref="Result{TSuccess,TFailure}"/> with identical failure types (<typeparamref name="TFailure"/>), produce
		/// a result holding all success values from the set if all input results hold success; otherwise, produce a result holding all
		/// failure values from the set.
		/// </summary>
		/// <typeparam name="TSuccess1">The first success type.</typeparam>
		/// <typeparam name="TSuccess2">The second success type.</typeparam>
		/// <typeparam name="TSuccess3">The third success type.</typeparam>
		/// <typeparam name="TSuccess4">The fourth success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="r1">The first result.</param>
		/// <param name="r2">The second result.</param>
		/// <param name="r3">The third result.</param>
		/// <param name="r4">The fourth result.</param>
		/// <returns>
		/// A <see cref="Result{TSuccess,TFailure}"/> holding (<typeparamref name="TSuccess1"/>, <typeparamref name="TSuccess2"/>, <typeparamref name="TSuccess3"/>)
		/// if all of <paramref name="r1"/>, <paramref name="r2"/>, <paramref name="r3"/>, and <paramref name="r4"/> hold success values; otherwise, a result holding all failures.
		/// </returns>
		[AllowAllocations]
		public static Result<(TSuccess1, TSuccess2, TSuccess3, TSuccess4), TFailure[]> Zip<TSuccess1, TSuccess2, TSuccess3, TSuccess4, TFailure>
		(
			Result<TSuccess1, TFailure> r1,
			Result<TSuccess2, TFailure> r2,
			Result<TSuccess3, TFailure> r3,
			Result<TSuccess4, TFailure> r4
		)
			where TSuccess1 : notnull
			where TSuccess2 : notnull
			where TSuccess3 : notnull
			where TSuccess4 : notnull
			where TFailure : notnull
		{
			if (r1.IsSuccess() && r2.IsSuccess() && r3.IsSuccess() && r4.IsSuccess())
				return Success<(TSuccess1, TSuccess2, TSuccess3, TSuccess4), TFailure[]>((
					r1.SuccessUnsafe(),
					r2.SuccessUnsafe(),
					r3.SuccessUnsafe(),
					r4.SuccessUnsafe()));

			var errorCollection = new List<TFailure>(4);
			if (!r1.IsSuccess())
				errorCollection.Add(r1.FailureUnsafe());
			if (!r2.IsSuccess())
				errorCollection.Add(r2.FailureUnsafe());
			if (!r3.IsSuccess())
				errorCollection.Add(r3.FailureUnsafe());
			if (!r4.IsSuccess())
				errorCollection.Add(r4.FailureUnsafe());

			return Failure<(TSuccess1, TSuccess2, TSuccess3, TSuccess4), TFailure[]>(errorCollection.ToArray());
		}

		/// <summary>
		/// Given a set of <see cref="Result{TSuccess,TFailure}"/> with identical failure types (<typeparamref name="TFailure"/>), produce
		/// a result holding all success values from the set if all input results hold success; otherwise, produce a result holding all
		/// failure values from the set.
		/// </summary>
		/// <typeparam name="TSuccess1">The first success type.</typeparam>
		/// <typeparam name="TSuccess2">The second success type.</typeparam>
		/// <typeparam name="TSuccess3">The third success type.</typeparam>
		/// <typeparam name="TSuccess4">The fourth success type.</typeparam>
		/// <typeparam name="TSuccess5">The fifth success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="r1">The first result.</param>
		/// <param name="r2">The second result.</param>
		/// <param name="r3">The third result.</param>
		/// <param name="r4">The fourth result.</param>
		/// <param name="r5">The fifth result.</param>
		/// <returns>
		/// A <see cref="Result{TSuccess,TFailure}"/> holding (<typeparamref name="TSuccess1"/>, <typeparamref name="TSuccess2"/>, <typeparamref name="TSuccess3"/>)
		/// if all of <paramref name="r1"/>, <paramref name="r2"/>, <paramref name="r3"/>, <paramref name="r4"/>, and <paramref name="r5"/> hold success values;
		/// otherwise, a result holding all failures.
		/// </returns>
		[AllowAllocations]
		public static Result<(TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5), TFailure[]> Zip<TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TFailure>
		(
			Result<TSuccess1, TFailure> r1,
			Result<TSuccess2, TFailure> r2,
			Result<TSuccess3, TFailure> r3,
			Result<TSuccess4, TFailure> r4,
			Result<TSuccess5, TFailure> r5
		)
			where TSuccess1 : notnull
			where TSuccess2 : notnull
			where TSuccess3 : notnull
			where TSuccess4 : notnull
			where TSuccess5 : notnull
			where TFailure : notnull
		{
			if (r1.IsSuccess() && r2.IsSuccess() && r3.IsSuccess() && r4.IsSuccess() && r5.IsSuccess())
				return Success<(TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5), TFailure[]>((
					r1.SuccessUnsafe(),
					r2.SuccessUnsafe(),
					r3.SuccessUnsafe(),
					r4.SuccessUnsafe(),
					r5.SuccessUnsafe()));

			var errorCollection = new List<TFailure>(5);
			if (!r1.IsSuccess())
				errorCollection.Add(r1.FailureUnsafe());
			if (!r2.IsSuccess())
				errorCollection.Add(r2.FailureUnsafe());
			if (!r3.IsSuccess())
				errorCollection.Add(r3.FailureUnsafe());
			if (!r4.IsSuccess())
				errorCollection.Add(r4.FailureUnsafe());
			if (!r5.IsSuccess())
				errorCollection.Add(r5.FailureUnsafe());

			return Failure<(TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5), TFailure[]>(errorCollection.ToArray());
		}

		/// <summary>
		/// Given a set of <see cref="Result{TSuccess,TFailure}"/> with identical failure types (<typeparamref name="TFailure"/>), produce
		/// a result holding all success values from the set if all input results hold success; otherwise, produce a result holding all
		/// failure values from the set.
		/// </summary>
		/// <typeparam name="TSuccess1">The first success type.</typeparam>
		/// <typeparam name="TSuccess2">The second success type.</typeparam>
		/// <typeparam name="TSuccess3">The third success type.</typeparam>
		/// <typeparam name="TSuccess4">The fourth success type.</typeparam>
		/// <typeparam name="TSuccess5">The fifth success type.</typeparam>
		/// <typeparam name="TSuccess6">The sixth success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="r1">The first result.</param>
		/// <param name="r2">The second result.</param>
		/// <param name="r3">The third result.</param>
		/// <param name="r4">The fourth result.</param>
		/// <param name="r5">The fifth result.</param>
		/// <param name="r6">The sixth result.</param>
		/// <returns>
		/// A <see cref="Result{TSuccess,TFailure}"/> holding (<typeparamref name="TSuccess1"/>, <typeparamref name="TSuccess2"/>, <typeparamref name="TSuccess3"/>)
		/// if all of <paramref name="r1"/>, <paramref name="r2"/>, <paramref name="r3"/>, <paramref name="r4"/>, <paramref name="r5"/>, and <paramref name="r6"/> hold
		/// success values; otherwise, a result holding all failures.
		/// </returns>
		[AllowAllocations]
		public static Result<(TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TSuccess6), TFailure[]> Zip<TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TSuccess6, TFailure>
		(
			Result<TSuccess1, TFailure> r1,
			Result<TSuccess2, TFailure> r2,
			Result<TSuccess3, TFailure> r3,
			Result<TSuccess4, TFailure> r4,
			Result<TSuccess5, TFailure> r5,
			Result<TSuccess6, TFailure> r6
		)
			where TSuccess1 : notnull
			where TSuccess2 : notnull
			where TSuccess3 : notnull
			where TSuccess4 : notnull
			where TSuccess5 : notnull
			where TSuccess6 : notnull
			where TFailure : notnull
		{
			if (r1.IsSuccess() && r2.IsSuccess() && r3.IsSuccess() && r4.IsSuccess() && r5.IsSuccess() && r6.IsSuccess())
				return Success<(TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TSuccess6), TFailure[]>((
					r1.SuccessUnsafe(),
					r2.SuccessUnsafe(),
					r3.SuccessUnsafe(),
					r4.SuccessUnsafe(),
					r5.SuccessUnsafe(),
					r6.SuccessUnsafe()));

			var errorCollection = new List<TFailure>(6);
			if (!r1.IsSuccess())
				errorCollection.Add(r1.FailureUnsafe());
			if (!r2.IsSuccess())
				errorCollection.Add(r2.FailureUnsafe());
			if (!r3.IsSuccess())
				errorCollection.Add(r3.FailureUnsafe());
			if (!r4.IsSuccess())
				errorCollection.Add(r4.FailureUnsafe());
			if (!r5.IsSuccess())
				errorCollection.Add(r5.FailureUnsafe());
			if (!r6.IsSuccess())
				errorCollection.Add(r6.FailureUnsafe());

			return Failure<(TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TSuccess6), TFailure[]>(errorCollection.ToArray());
		}

		/// <summary>
		/// Given a set of <see cref="Result{TSuccess,TFailure}"/> with identical failure types (<typeparamref name="TFailure"/>), produce
		/// a result holding all success values from the set if all input results hold success; otherwise, produce a result holding all
		/// failure values from the set.
		/// </summary>
		/// <typeparam name="TSuccess1">The first success type.</typeparam>
		/// <typeparam name="TSuccess2">The second success type.</typeparam>
		/// <typeparam name="TSuccess3">The third success type.</typeparam>
		/// <typeparam name="TSuccess4">The fourth success type.</typeparam>
		/// <typeparam name="TSuccess5">The fifth success type.</typeparam>
		/// <typeparam name="TSuccess6">The sixth success type.</typeparam>
		/// <typeparam name="TSuccess7">The seventh success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="r1">The first result.</param>
		/// <param name="r2">The second result.</param>
		/// <param name="r3">The third result.</param>
		/// <param name="r4">The fourth result.</param>
		/// <param name="r5">The fifth result.</param>
		/// <param name="r6">The sixth result.</param>
		/// <param name="r7">The seventh result.</param>
		/// <returns>
		/// A <see cref="Result{TSuccess,TFailure}"/> holding (<typeparamref name="TSuccess1"/>, <typeparamref name="TSuccess2"/>, <typeparamref name="TSuccess3"/>)
		/// if all of <paramref name="r1"/>, <paramref name="r2"/>, <paramref name="r3"/>, <paramref name="r4"/>, <paramref name="r5"/>, <paramref name="r6"/>, and
		/// <paramref name="r7"/> hold success values; otherwise, a result holding all failures.
		/// </returns>
		[AllowAllocations]
		public static Result<(TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TSuccess6, TSuccess7), TFailure[]> Zip<TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TSuccess6, TSuccess7, TFailure>
		(
			Result<TSuccess1, TFailure> r1,
			Result<TSuccess2, TFailure> r2,
			Result<TSuccess3, TFailure> r3,
			Result<TSuccess4, TFailure> r4,
			Result<TSuccess5, TFailure> r5,
			Result<TSuccess6, TFailure> r6,
			Result<TSuccess7, TFailure> r7
		)
			where TSuccess1 : notnull
			where TSuccess2 : notnull
			where TSuccess3 : notnull
			where TSuccess4 : notnull
			where TSuccess5 : notnull
			where TSuccess6 : notnull
			where TSuccess7 : notnull
			where TFailure : notnull
		{
			if (r1.IsSuccess() && r2.IsSuccess() && r3.IsSuccess() && r4.IsSuccess() && r5.IsSuccess() && r6.IsSuccess() && r7.IsSuccess())
				return Success<(TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TSuccess6, TSuccess7), TFailure[]>((
					r1.SuccessUnsafe(),
					r2.SuccessUnsafe(),
					r3.SuccessUnsafe(),
					r4.SuccessUnsafe(),
					r5.SuccessUnsafe(),
					r6.SuccessUnsafe(),
					r7.SuccessUnsafe()));

			var errorCollection = new List<TFailure>(7);
			if (!r1.IsSuccess())
				errorCollection.Add(r1.FailureUnsafe());
			if (!r2.IsSuccess())
				errorCollection.Add(r2.FailureUnsafe());
			if (!r3.IsSuccess())
				errorCollection.Add(r3.FailureUnsafe());
			if (!r4.IsSuccess())
				errorCollection.Add(r4.FailureUnsafe());
			if (!r5.IsSuccess())
				errorCollection.Add(r5.FailureUnsafe());
			if (!r6.IsSuccess())
				errorCollection.Add(r6.FailureUnsafe());
			if (!r7.IsSuccess())
				errorCollection.Add(r7.FailureUnsafe());

			return Failure<(TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TSuccess6, TSuccess7), TFailure[]>(errorCollection.ToArray());
		}

		/// <summary>
		/// Given a set of <see cref="Result{TSuccess,TFailure}"/> with identical failure types (<typeparamref name="TFailure"/>), produce
		/// a result holding all success values from the set if all input results hold success; otherwise, produce a result holding all
		/// failure values from the set.
		/// </summary>
		/// <typeparam name="TSuccess1">The first success type.</typeparam>
		/// <typeparam name="TSuccess2">The second success type.</typeparam>
		/// <typeparam name="TSuccess3">The third success type.</typeparam>
		/// <typeparam name="TSuccess4">The fourth success type.</typeparam>
		/// <typeparam name="TSuccess5">The fifth success type.</typeparam>
		/// <typeparam name="TSuccess6">The sixth success type.</typeparam>
		/// <typeparam name="TSuccess7">The seventh success type.</typeparam>
		/// <typeparam name="TSuccess8">The eighth success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="r1">The first result.</param>
		/// <param name="r2">The second result.</param>
		/// <param name="r3">The third result.</param>
		/// <param name="r4">The fourth result.</param>
		/// <param name="r5">The fifth result.</param>
		/// <param name="r6">The sixth result.</param>
		/// <param name="r7">The seventh result.</param>
		/// <param name="r8">The eighth result.</param>
		/// <returns>
		/// A <see cref="Result{TSuccess,TFailure}"/> holding (<typeparamref name="TSuccess1"/>, <typeparamref name="TSuccess2"/>, <typeparamref name="TSuccess3"/>)
		/// if all of <paramref name="r1"/>, <paramref name="r2"/>, <paramref name="r3"/>, <paramref name="r4"/>, <paramref name="r5"/>, <paramref name="r6"/>,
		/// <paramref name="r7"/>, and <paramref name="r8"/> hold success values; otherwise, a result holding all failures.
		/// </returns>
		[AllowAllocations]
		public static Result<(TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TSuccess6, TSuccess7, TSuccess8), TFailure[]> Zip<TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TSuccess6, TSuccess7, TSuccess8, TFailure>
		(
			Result<TSuccess1, TFailure> r1,
			Result<TSuccess2, TFailure> r2,
			Result<TSuccess3, TFailure> r3,
			Result<TSuccess4, TFailure> r4,
			Result<TSuccess5, TFailure> r5,
			Result<TSuccess6, TFailure> r6,
			Result<TSuccess7, TFailure> r7,
			Result<TSuccess8, TFailure> r8
		)
			where TSuccess1 : notnull
			where TSuccess2 : notnull
			where TSuccess3 : notnull
			where TSuccess4 : notnull
			where TSuccess5 : notnull
			where TSuccess6 : notnull
			where TSuccess7 : notnull
			where TSuccess8 : notnull
			where TFailure : notnull
		{
			if (r1.IsSuccess() && r2.IsSuccess() && r3.IsSuccess() && r4.IsSuccess() && r5.IsSuccess() && r6.IsSuccess() && r7.IsSuccess() && r8.IsSuccess())
				return Success<(TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TSuccess6, TSuccess7, TSuccess8), TFailure[]>((
					r1.SuccessUnsafe(),
					r2.SuccessUnsafe(),
					r3.SuccessUnsafe(),
					r4.SuccessUnsafe(),
					r5.SuccessUnsafe(),
					r6.SuccessUnsafe(),
					r7.SuccessUnsafe(),
					r8.SuccessUnsafe()));

			var errorCollection = new List<TFailure>(8);
			if (!r1.IsSuccess())
				errorCollection.Add(r1.FailureUnsafe());
			if (!r2.IsSuccess())
				errorCollection.Add(r2.FailureUnsafe());
			if (!r3.IsSuccess())
				errorCollection.Add(r3.FailureUnsafe());
			if (!r4.IsSuccess())
				errorCollection.Add(r4.FailureUnsafe());
			if (!r5.IsSuccess())
				errorCollection.Add(r5.FailureUnsafe());
			if (!r6.IsSuccess())
				errorCollection.Add(r6.FailureUnsafe());
			if (!r7.IsSuccess())
				errorCollection.Add(r7.FailureUnsafe());
			if (!r8.IsSuccess())
				errorCollection.Add(r8.FailureUnsafe());

			return Failure<(TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TSuccess6, TSuccess7, TSuccess8), TFailure[]>(errorCollection.ToArray());
		}

		/// <summary>
		/// Given a set of <see cref="Result{TSuccess,TFailure}"/> with identical failure types (<typeparamref name="TFailure"/>), produce
		/// a result holding all success values from the set if all input results hold success; otherwise, produce a result holding all
		/// failure values from the set.
		/// </summary>
		/// <typeparam name="TSuccess1">The first success type.</typeparam>
		/// <typeparam name="TSuccess2">The second success type.</typeparam>
		/// <typeparam name="TSuccess3">The third success type.</typeparam>
		/// <typeparam name="TSuccess4">The fourth success type.</typeparam>
		/// <typeparam name="TSuccess5">The fifth success type.</typeparam>
		/// <typeparam name="TSuccess6">The sixth success type.</typeparam>
		/// <typeparam name="TSuccess7">The seventh success type.</typeparam>
		/// <typeparam name="TSuccess8">The eighth success type.</typeparam>
		/// <typeparam name="TSuccess9">The ninth success type.</typeparam>
		/// <typeparam name="TFailure">The failure type.</typeparam>
		/// <param name="r1">The first result.</param>
		/// <param name="r2">The second result.</param>
		/// <param name="r3">The third result.</param>
		/// <param name="r4">The fourth result.</param>
		/// <param name="r5">The fifth result.</param>
		/// <param name="r6">The sixth result.</param>
		/// <param name="r7">The seventh result.</param>
		/// <param name="r8">The eighth result.</param>
		/// <param name="r9">The ninth result.</param>
		/// <returns>
		/// A <see cref="Result{TSuccess,TFailure}"/> holding (<typeparamref name="TSuccess1"/>, <typeparamref name="TSuccess2"/>, <typeparamref name="TSuccess3"/>)
		/// if all of <paramref name="r1"/>, <paramref name="r2"/>, <paramref name="r3"/>, <paramref name="r4"/>, <paramref name="r5"/>, <paramref name="r6"/>,
		/// <paramref name="r7"/>, <paramref name="r8"/>, and <paramref name="r9"/> hold success values; otherwise, a result holding all failures.
		/// </returns>
		[AllowAllocations]
		public static Result<(TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TSuccess6, TSuccess7, TSuccess8, TSuccess9), TFailure[]> Zip<TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TSuccess6, TSuccess7, TSuccess8, TSuccess9, TFailure>
		(
			Result<TSuccess1, TFailure> r1,
			Result<TSuccess2, TFailure> r2,
			Result<TSuccess3, TFailure> r3,
			Result<TSuccess4, TFailure> r4,
			Result<TSuccess5, TFailure> r5,
			Result<TSuccess6, TFailure> r6,
			Result<TSuccess7, TFailure> r7,
			Result<TSuccess8, TFailure> r8,
			Result<TSuccess9, TFailure> r9
		)
			where TSuccess1 : notnull
			where TSuccess2 : notnull
			where TSuccess3 : notnull
			where TSuccess4 : notnull
			where TSuccess5 : notnull
			where TSuccess6 : notnull
			where TSuccess7 : notnull
			where TSuccess8 : notnull
			where TSuccess9 : notnull
			where TFailure : notnull
		{
			if (r1.IsSuccess() && r2.IsSuccess() && r3.IsSuccess() && r4.IsSuccess() && r5.IsSuccess() && r6.IsSuccess() && r7.IsSuccess() && r8.IsSuccess() && r9.IsSuccess())
				return Success<(TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TSuccess6, TSuccess7, TSuccess8, TSuccess9), TFailure[]>((
					r1.SuccessUnsafe(),
					r2.SuccessUnsafe(),
					r3.SuccessUnsafe(),
					r4.SuccessUnsafe(),
					r5.SuccessUnsafe(),
					r6.SuccessUnsafe(),
					r7.SuccessUnsafe(),
					r8.SuccessUnsafe(),
					r9.SuccessUnsafe()));

			var errorCollection = new List<TFailure>(9);
			if (!r1.IsSuccess())
				errorCollection.Add(r1.FailureUnsafe());
			if (!r2.IsSuccess())
				errorCollection.Add(r2.FailureUnsafe());
			if (!r3.IsSuccess())
				errorCollection.Add(r3.FailureUnsafe());
			if (!r4.IsSuccess())
				errorCollection.Add(r4.FailureUnsafe());
			if (!r5.IsSuccess())
				errorCollection.Add(r5.FailureUnsafe());
			if (!r6.IsSuccess())
				errorCollection.Add(r6.FailureUnsafe());
			if (!r7.IsSuccess())
				errorCollection.Add(r7.FailureUnsafe());
			if (!r8.IsSuccess())
				errorCollection.Add(r8.FailureUnsafe());
			if (!r9.IsSuccess())
				errorCollection.Add(r9.FailureUnsafe());

			return Failure<(TSuccess1, TSuccess2, TSuccess3, TSuccess4, TSuccess5, TSuccess6, TSuccess7, TSuccess8, TSuccess9), TFailure[]>(errorCollection.ToArray());
		}
	}
}
