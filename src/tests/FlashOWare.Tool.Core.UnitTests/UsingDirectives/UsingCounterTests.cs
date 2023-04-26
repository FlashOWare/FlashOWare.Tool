using FlashOWare.Tool.Core.UsingDirectives;
using Microsoft.CodeAnalysis;

namespace FlashOWare.Tool.Core.UnitTests.UsingDirectives;

public class UsingCounterTests
{
    //TODO: Workspace is IDisposable

    //TODO: Assert that Compilation of Project has no C# Syntax Errors
    //no semantics, references to BCL, other projects
    //Basic.Reference.Assemblies
    //Basic.Reference.Assemblies.[TFM]

    [Fact]
    public async Task CountAsync_SingleUsing_FindsOne()
    {
        //Arrange
        var project = CreateProject("""
            using System;
            """);
        var expectedResult = new Dictionary<string, int>
        {
            { "System", 1 },
        };
        //Act
        var count = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, count);
    }

    [Fact]
    public async Task CountAsync_MultipleUsings_FindsMultiple()
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
            """ );
        var expectedResult = new Dictionary<string, int>
        {
            { "System", 1 },
            { "System.Collections.Generic", 1 },
            { "System.IO", 1 },
            { "System.Linq", 1 },
            { "System.Net.Http", 1 },
            { "System.Threading", 1 },
            { "System.Threading.Tasks", 1 },
        };
        //Act
        var count = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, count);
    }

    [Fact]
    public async Task CountAsync_SingleUsingInMultipleDocuments_FindsTwoOccurences()
    {
        //Arrange
        var project = CreateProject("""
            using System;
            """, """
            using System;
            """ );
        var expectedResult = new Dictionary<string, int>
        {
            { "System", 2 },
        };
        //Act
        var count = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, count);
    }

    [Fact]
    public async Task CountAsync_MultipleUsingsInMultipleDocuments_FindsManyOccurences()
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
            """ );
        var expectedResult = new Dictionary<string, int>
        {
            { "System", 3 },
            { "System.Collections.Generic", 3 },
            { "System.IO", 2 },
            { "System.Linq", 2 },
            { "System.Net.Http", 1 },
            { "System.Threading", 2 },
            { "System.Threading.Tasks", 2 },
        };
        //Act
        var count = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, count);
    }

    [Fact]
    public async Task CountAsync_FileScopedNamespaces_FindsAllOccurences()
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
        var expectedResult = new Dictionary<string, int>
        {
            { "System", 2 },
            { "System.Collections.Generic", 2 },
            { "System.IO", 1 },
            { "System.Linq", 1 },
            { "System.Net.Http", 1 },
            { "System.Threading", 1 },
            { "System.Threading.Tasks", 1 },
        };
        //Act
        var count = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, count);
    }

    [Fact]
    public async Task CountAsync_BlockScopedNamespaces_FindsAllOccurences()
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
        var expectedResult = new Dictionary<string, int>
        {
            { "System", 2 },
            { "System.Collections.Generic", 2 },
            { "System.IO", 1 },
            { "System.Linq", 1 },
            { "System.Net.Http", 1 },
            { "System.Threading", 1 },
            { "System.Threading.Tasks", 1 },
        };
        //Act
        var count = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, count);
    }

    [Fact]
    public async Task CountAsync_WithUsingAlias_DoNotInclude()
    {
        //Arrange
        var project = CreateProject("""
            using MyNamespace = System;
            using MyType = System.Console;
            """);
        var expectedResult = new Dictionary<string, int>
        {
        };
        //Act
        var count = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, count);
    }

    [Fact]
    public async Task CountAsync_WithUsingStatic_DoNotInclude()
    {
        //Arrange
        var project = CreateProject("""
            using static System.Console;
            """);
        var expectedResult = new Dictionary<string, int>
        {
        };
        //Act
        var count = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, count);
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
        var expectedResult = new Dictionary<string, int>
        {
        };
        //Act
        var count = await UsingCounter.CountAsync(project);
        //Assert
        Assert.Equal(expectedResult, count);
    }

    private static Project CreateProject(params string[] documents)
    {
        var workspace = new AdhocWorkspace();
        var solution = workspace.CurrentSolution;

        var projectId = ProjectId.CreateNewId();
        solution = solution.AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp);

        foreach (string document in documents)
        {
            var documentId = DocumentId.CreateNewId(projectId);
            solution = solution.AddDocument(documentId, "TestDocument.cs", document);
        }

        return solution.GetProject(projectId);
    }
}
