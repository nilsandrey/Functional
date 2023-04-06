﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Functional.Tests.IL
{
	public class ParsedMethodBody
	{
		public abstract record ParentInfo
		{
			public record Constructor(ConstructorInfo ConstructorInfo) : ParentInfo
			{
				public override TValue Match<TValue>(Func<ConstructorInfo, TValue> onConstructor, Func<MethodInfo, TValue> onMethod)
					=> onConstructor.Invoke(ConstructorInfo);
			};

			public record Method(MethodInfo MethodInfo) : ParentInfo
			{
				public override TValue Match<TValue>(Func<ConstructorInfo, TValue> onConstructor, Func<MethodInfo, TValue> onMethod)
					=> onMethod.Invoke(MethodInfo);
			}

			public abstract TValue Match<TValue>(Func<ConstructorInfo, TValue> onConstructor, Func<MethodInfo, TValue> onMethod);
		}
		public ParentInfo Parent { get; }

		public IReadOnlyList<Instruction> Instructions { get; }

		private ParsedMethodBody(ParentInfo parent, Instruction[] instructions)
		{
			Parent = parent;
			Instructions = instructions;
		}

		public static ParsedMethodBody Create(ConstructorInfo constructor, Instruction[] instructions)
			=> new ParsedMethodBody(new ParentInfo.Constructor(constructor), instructions);

		public static ParsedMethodBody Create(MethodInfo method, Instruction[] instructions)
			=> new ParsedMethodBody(new ParentInfo.Method(method), instructions);
	}
}
