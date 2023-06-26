﻿namespace Functional.Unions;

[Generator]
public sealed class DiscriminatedUnionAttributeGenerator : ISourceGenerator
{
	public void Initialize(GeneratorInitializationContext context) 
		=> context.RegisterForPostInitialization(c => c.AddSource("DiscriminatedUnionAttribute.g.cs", Code.GetDiscriminatedUnionAttributes()));

	public void Execute(GeneratorExecutionContext context)
	{
	}
}