using Basic.Reference.Assemblies;
using FlashOWare.Tool.Core.UsingDirectives;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using System.Text;

namespace FlashOWare.Tool.Core.Tests.UsingDirectives;

//TODO: use async/await over Task.Result
//or move Errors-Check to production code instead?

//TODO: more Auto-Generated Comment Tests
//TODO: change Assert.Equal to Assert.Collection + Assert.Multiple

public class UsingCounterTests
{
    [Fact]
    public async Task CountAsync_SingleUsing_FindsOneOccurrence()
    {
        //Arrange
        var project = CreateProject("""
            using System;
            """);
        var expectedResult = new UsingDirective[]
        {
            new("System", 1),
        };
        //Act
        var result = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, result.Usings);
    }

    [Fact]
    public async Task CountAsync_MultipleUsings_FindsMultipleOccurrences()
    {
        //Arrange
        var project = CreateProject("""
            using System;
            using System.Collections.Generic;
            using System.IO;
            using System.Linq;
            using System.Net.Http;
            using System.Threading;
            using System.Threading.Tasks;
            """);
        var expectedResult = new UsingDirective[]
        {
            new("System", 1 ),
            new("System.Collections.Generic", 1),
            new("System.IO", 1 ),
            new("System.Linq", 1 ),
            new("System.Net.Http", 1 ),
            new("System.Threading", 1 ),
            new("System.Threading.Tasks", 1 ),
        };
        //Act
        var result = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, result.Usings);
    }

    [Fact]
    public async Task CountAsync_SingleUsingInMultipleDocuments_FindsTwoOccurrences()
    {
        //Arrange
        var project = CreateProject("""
            using System;
            """, """
            using System;
            """);
        var expectedResult = new UsingDirective[]
        {
            new("System", 2),
        };
        //Act
        var result = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, result.Usings);
    }

    [Fact]
    public async Task CountAsync_MultipleUsingsInMultipleDocuments_FindsManyOccurrences()
    {
        //Arrange
        var project = CreateProject("""
            using System;
            using System.Collections.Generic;
            using System.IO;
            using System.Linq;
            using System.Net.Http;
            using System.Threading;
            using System.Threading.Tasks;
            """, """
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Threading.Tasks;
            """, """
            using System;
            using System.Collections.Generic;
            using System.IO;
            using System.Threading;
            """);
        var expectedResult = new UsingDirective[]
        {
            new("System", 3),
            new("System.Collections.Generic", 3),
            new("System.IO", 2),
            new("System.Linq", 2),
            new("System.Net.Http", 1),
            new("System.Threading", 2),
            new("System.Threading.Tasks", 2),
        };
        //Act
        var result = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, result.Usings);
    }

    [Fact]
    public async Task CountAsync_FileScopedNamespaces_FindsAllTopLevelOccurrences()
    {
        //Arrange
        var project = CreateProject("""
            using System;
            using System.Collections.Generic;
            using System.IO;
            using System.Linq;
            using System.Net.Http;
            using System.Threading;
            using System.Threading.Tasks;

            namespace MyNamespace;

            public class MyClass1 {}
            """, """
            using System;
            using System.Collections.Generic;

            namespace MyNamespace;
            
            using System.Linq;
            using System.Threading.Tasks;
            
            public class MyClass2 {}
            """, """
            namespace MyNamespace;

            using System;
            using System.Collections.Generic;
            using System.IO;
            using System.Threading;

            public class MyClass3 {}
            """);
        var expectedResult = new UsingDirective[]
        {
            new("System", 2),
            new("System.Collections.Generic", 2),
            new("System.IO", 1),
            new("System.Linq", 1),
            new("System.Net.Http", 1),
            new("System.Threading", 1),
            new("System.Threading.Tasks", 1),
        };
        //Act
        var result = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, result.Usings);
    }

    [Fact]
    public async Task CountAsync_BlockScopedNamespaces_FindsAllTopLevelOccurrences()
    {
        //Arrange
        var project = CreateProject("""
            using System;
            using System.Collections.Generic;
            using System.IO;
            using System.Linq;
            using System.Net.Http;
            using System.Threading;
            using System.Threading.Tasks;

            namespace MyNamespace
            {
                public class MyClass1 {}
            }
            """, """
            using System;
            using System.Collections.Generic;

            namespace MyNamespace
            {
                using System.Linq;
                using System.Threading.Tasks;

                public class MyClass2 {}
            }
            """, """
            namespace MyNamespace
            {
                using System;
                using System.Collections.Generic;
                using System.IO;
                using System.Threading;

                public class MyClass3 {}
            }
            """);
        var expectedResult = new UsingDirective[]
        {
            new("System", 2),
            new("System.Collections.Generic", 2),
            new("System.IO", 1),
            new("System.Linq", 1),
            new("System.Net.Http", 1),
            new("System.Threading", 1),
            new("System.Threading.Tasks", 1),
        };
        //Act
        var result = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, result.Usings);
    }

    [Fact]
    public async Task CountAsync_WithUsingAlias_DoNotInclude()
    {
        //Arrange
        var project = CreateProject("""
            using MyNamespace = System;
            using MyType = System.Console;
            """);
        var expectedResult = new UsingDirective[]
        {
        };
        //Act
        var result = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, result.Usings);
    }

    [Fact]
    public async Task CountAsync_WithUsingStatic_DoNotInclude()
    {
        //Arrange
        var project = CreateProject("""
            using static System.Console;
            """);
        var expectedResult = new UsingDirective[]
        {
        };
        //Act
        var result = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, result.Usings);
    }

    [Fact]
    public async Task CountAsync_WithGlobalUsings_DoNotInclude()
    {
        //Arrange
        var project = CreateProject("""
            global using System;
            global using MyNamespace = System;
            global using MyType = System.Console;
            global using static System.Console;
            """);
        var expectedResult = new UsingDirective[]
        {
        };
        //Act
        var result = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, result.Usings);
    }

    [Fact]
    public async Task CountAsync_AutoGeneratedDocuments_Ignore()
    {
        //Arrange
        var project = CreateProject(
            ("TemporaryGeneratedFile_File1.cs", "using System;"),
            ("File2.designer.cs", "using System;"),
            ("File3.generated.cs", "using System;"),
            ("File4.g.cs", "using System;"),
            ("File5.g.i.cs", "using System;")
        );
        var expectedResult = new UsingDirective[]
        {
        };
        //Act
        var result = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, result.Usings);
    }

    [Fact]
    public async Task CountAsync_AutoGeneratedTexts_Ignore()
    {
        //Arrange
        var project = CreateProject("""
            //<auto-generated>
            using System;
            """, """
            //<auto-generated/>
            using System;
            """, """
            /*<auto-generated>*/
            using System;
            """, """
            /*<auto-generated/>*/
            using System;
            """, """
            // auto-generated
            using System;
            """);
        var expectedResult = new UsingDirective[]
        {
            new("System", 1),
        };
        //Act
        var result = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, result.Usings);
    }

    private static Project CreateProject(params string[] documents)
    {
        int index = 0;
        return CreateProject(documents.Select(text => ($"TestDocument{index++}.cs", text)).ToArray());
    }

    private static Project CreateProject(params (string Name, string Text)[] documents)
    {
        using var workspace = new AdhocWorkspace();
        var solution = workspace.CurrentSolution;

        var projectId = ProjectId.CreateNewId();
        solution = solution.AddProject(projectId, "TestProject", "TestAssembly", LanguageNames.CSharp);

        foreach (var document in documents)
        {
            var documentId = DocumentId.CreateNewId(projectId);
            solution = solution.AddDocument(documentId, document.Name, document.Text);
        }

        Project? project = solution.GetProject(projectId);
        Debug.Assert(project is not null, $"{nameof(ProjectId)} {projectId} is not an id of a project that is part of this solution.");
        project = project.AddMetadataReferences(ReferenceAssemblies.NetStandard20);
        project = project.WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        Debug.Assert(project.SupportsCompilation, $"{nameof(Project)}.{nameof(Project.SupportsCompilation)} = {project.SupportsCompilation} ({project.Name})");
        var errors = project.GetCompilationAsync(CancellationToken.None).Result!.GetDiagnostics()
            .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .ToArray();

        if (errors.Length > 0)
        {
            StringBuilder test = new();
            foreach (var error in errors)
            {
                test.AppendLine(error.ToString());
            }

            throw new InvalidOperationException(test.ToString());
        }

        return project;
    }
}
