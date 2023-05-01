namespace FlashOWare.Tool.Core.UsingDirectives;

//TODO: Initial Release Version (MVP)
//create new "GlobalUsings.cs" if not existing
//- Directory/Name?
//  - in "Properties" directory?
//    - e.g. .NET Framework: <Compile Include="Properties\AssemblyInfo.cs" />
//  - or in "root"
//    - e.g. .NET 7 "xUnit Test Project" template: "\Usings.cs"
//- File-Name?
//  - e.g. from .NET 7 "xUnit" template: ".\Usings.cs"
//  - e.g. from ImplicitUsings: "\obj\Debug\net7.0\MyProject.GlobalUsings.g.cs"
//Append if exists

//top-level usings only, no nested usings within namespaces
//no using alias (neither type nor namespace alias)
//no using static
//no global usings
//Ignore auto-generated documents and auto-generated code
//check C# 10 or greater
//support cancellation via CancellationToken

public static class UsingGlobalizer
{
    public static IReadOnlyList<string> Globalize(IReadOnlyList<string> documents, string localUsing)
    {
        //TODO
        //var newNode = rootSyntaxNode.RemoveNode(usingNode);
        //return new node
        //var newNode = usingNode.WithAlias("MyAlias");
        //var newDocument = cSharpDocument.WithSyntaxRoot(null);

        throw new NotImplementedException("2code ^ !2code [S2023E07] Global Usings .NET Tool Chapter 3");
    }
}
