﻿using System.Collections.Immutable;

namespace Functional.Unions;

public static class CompilationExtensions
{
	public static IImmutableSet<INamedTypeSymbol> GetDiscriminatedUnionTypes(this Compilation compilation)
		=> Enumerable
			.Range(1, Code.DiscriminatedUnion.MaxSupportedTypes)
			.Select(i => compilation.Assembly.GetTypeByMetadataName($"{Code.DiscriminatedUnion.Namespace}.{Code.DiscriminatedUnion.Name}`{i}") ?? throw new Exception("Couldn't find discriminated union interface types."))
			.ToImmutableHashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

	public static IEnumerable<INamedTypeSymbol> GetAllNamedTypeSymbols(this Compilation compilation)
		=> compilation
			.Assembly
			.GlobalNamespace
			.GetAllNamedTypeSymbols();

	private static IEnumerable<INamedTypeSymbol> GetAllNamedTypeSymbols(this INamespaceSymbol namespaceSymbol)
		=> namespaceSymbol
			.GetTypeMembers()
			.SelectMany(t => t.GetAllNamedTypeSymbols())
			.Concat(namespaceSymbol
				.GetNamespaceMembers()
				.SelectMany(n => n.GetAllNamedTypeSymbols())
			);

	private static IEnumerable<INamedTypeSymbol> GetAllNamedTypeSymbols(this INamedTypeSymbol symbol)
		=> new[] { symbol }
			.Concat(symbol
				.GetTypeMembers()
				.SelectMany(t => t.GetAllNamedTypeSymbols())
			);
}
