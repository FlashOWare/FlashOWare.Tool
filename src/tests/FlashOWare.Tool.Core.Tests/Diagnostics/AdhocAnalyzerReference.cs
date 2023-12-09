using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace FlashOWare.Tool.Core.Tests.Diagnostics;

internal sealed class AdhocAnalyzerReference : AnalyzerReference
{
    private readonly string _fullPath;
    private readonly string _display;
    private readonly object _id;

    private readonly DiagnosticAnalyzer? _analyzer;
    private readonly ISourceGenerator? _generator;

    private AdhocAnalyzerReference(Type type)
    {
        string name = type.ToString();

        _fullPath = $"{name}.dll";
        _display = name;
        _id = new AssemblyIdentity(name, new Version(1, 0, 0, 0));
    }

    public AdhocAnalyzerReference(DiagnosticAnalyzer analyzer)
        : this(analyzer.GetType())
    {
        _analyzer = analyzer;
    }

    public AdhocAnalyzerReference(ISourceGenerator generator)
        : this(generator.GetType())
    {
        _generator = generator;
    }

    public AdhocAnalyzerReference(IIncrementalGenerator generator)
        : this(generator.GetType())
    {
        _generator = generator.AsSourceGenerator();
    }

    public override string? FullPath => _fullPath;

    public override string Display => _display;

    public override object Id => _id;

    public override ImmutableArray<DiagnosticAnalyzer> GetAnalyzersForAllLanguages()
    {
        if (_analyzer is null)
        {
            return ImmutableArray<DiagnosticAnalyzer>.Empty;
        }

        return ImmutableArray.Create(_analyzer);
    }

    public override ImmutableArray<DiagnosticAnalyzer> GetAnalyzers(string language)
    {
        if (_analyzer is null)
        {
            return ImmutableArray<DiagnosticAnalyzer>.Empty;
        }

        return ImmutableArray.Create(_analyzer);
    }

    public override ImmutableArray<ISourceGenerator> GetGeneratorsForAllLanguages()
    {
        if (_generator is null)
        {
            return ImmutableArray<ISourceGenerator>.Empty;
        }

        return ImmutableArray.Create(_generator);
    }

    public override ImmutableArray<ISourceGenerator> GetGenerators(string language)
    {
        if (_generator is null)
        {
            return ImmutableArray<ISourceGenerator>.Empty;
        }

        return ImmutableArray.Create(_generator);
    }
}
