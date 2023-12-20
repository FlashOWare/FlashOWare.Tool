using FlashOWare.Tool.Core.Tests.Assertions;
using FlashOWare.Tool.Core.Tests.Testing;
using FlashOWare.Tool.Core.UsingDirectives;
using System.Collections.Immutable;

namespace FlashOWare.Tool.Core.Tests.UsingDirectives;

public class UsingCounterTests
{
    [Fact]
    public async Task CountAsync_Empty_FindsNoOccurrences()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync();
        UsingDirective[] expectedResult = [];
        //Act
        var actualResult = await UsingCounter.CountAsync(project);
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_NoUsings_FindsNoOccurrences()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            internal class Program
            {
                private static void Main(string[] args)
                {
                    System.Console.WriteLine("Hello, World!");
                }
            }
            """);
        UsingDirective[] expectedResult = [];
        //Act
        var actualResult = await UsingCounter.CountAsync(project);
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_SingleUsing_FindsOneOccurrence()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using System;
            """);
        UsingDirective[] expectedResult =
        [
            new("System", 1),
        ];
        //Act
        var actualResult = await UsingCounter.CountAsync(project);
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_MultipleUsings_FindsMultipleOccurrences()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using System;
            using System.Collections.Generic;
            using System.IO;
            using System.Linq;
            using System.Net.Http;
            using System.Threading;
            using System.Threading.Tasks;
            """);
        UsingDirective[] expectedResult =
        [
            new("System", 1),
            new("System.Collections.Generic", 1),
            new("System.IO", 1),
            new("System.Linq", 1),
            new("System.Net.Http", 1),
            new("System.Threading", 1),
            new("System.Threading.Tasks", 1),
        ];
        //Act
        var actualResult = await UsingCounter.CountAsync(project);
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_SingleUsingInMultipleDocuments_FindsTwoOccurrences()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using System;
            """, """
            using System;
            """);
        UsingDirective[] expectedResult =
        [
            new("System", 2),
        ];
        //Act
        var actualResult = await UsingCounter.CountAsync(project);
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_MultipleUsingsInMultipleDocuments_FindsManyOccurrences()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
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
        UsingDirective[] expectedResult =
        [
            new("System", 3),
            new("System.Collections.Generic", 3),
            new("System.IO", 2),
            new("System.Linq", 2),
            new("System.Net.Http", 1),
            new("System.Threading", 2),
            new("System.Threading.Tasks", 2),
        ];
        //Act
        var actualResult = await UsingCounter.CountAsync(project);
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_FileScopedNamespaces_FindsAllTopLevelOccurrences()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
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
        UsingDirective[] expectedResult =
        [
            new("System", 2),
            new("System.Collections.Generic", 2),
            new("System.IO", 1),
            new("System.Linq", 1),
            new("System.Net.Http", 1),
            new("System.Threading", 1),
            new("System.Threading.Tasks", 1),
        ];
        //Act
        var actualResult = await UsingCounter.CountAsync(project);
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_BlockScopedNamespaces_FindsAllTopLevelOccurrences()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
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
        UsingDirective[] expectedResult =
        [
            new("System", 2),
            new("System.Collections.Generic", 2),
            new("System.IO", 1),
            new("System.Linq", 1),
            new("System.Net.Http", 1),
            new("System.Threading", 1),
            new("System.Threading.Tasks", 1),
        ];
        //Act
        var actualResult = await UsingCounter.CountAsync(project);
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_WithUsingAlias_DoNotInclude()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using MyNamespace = System;
            using MyType = System.Console;
            """);
        UsingDirective[] expectedResult = [];
        //Act
        var actualResult = await UsingCounter.CountAsync(project);
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_WithUsingStatic_DoNotInclude()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using static System.Console;
            """);
        UsingDirective[] expectedResult = [];
        //Act
        var actualResult = await UsingCounter.CountAsync(project);
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_WithGlobalUsings_DoNotInclude()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            global using System;
            global using MyNamespace = System;
            global using MyType = System.Console;
            global using static System.Console;
            """);
        UsingDirective[] expectedResult = [];
        //Act
        var actualResult = await UsingCounter.CountAsync(project);
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_SpecificUsing_FindsSpecifiedOccurrences()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using System;
            using System.Collections.Generic;
            using System.IO;
            using System.Linq;
            using System.Net.Http;
            using System.Threading;
            using System.Threading.Tasks;
            """);
        UsingDirective[] expectedResult =
        [
            new("System", 1),
        ];
        //Act
        var actualResult = await UsingCounter.CountAsync(project, "System");
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_SpecificUsingNotFound_ContainsWithoutOccurrences()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using System;
            using System.Collections.Generic;
            using System.IO;
            using System.Linq;
            using System.Net.Http;
            using System.Threading;
            using System.Threading.Tasks;
            """);
        UsingDirective[] expectedResult =
        [
            new("Not.Found", 0),
        ];
        //Act
        var actualResult = await UsingCounter.CountAsync(project, "Not.Found");
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_SpecificUsings_FindsSpecifiedOccurrences()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using System;
            using System.Collections.Generic;
            using System.IO;
            using System.Linq;
            using System.Net.Http;
            using System.Threading;
            using System.Threading.Tasks;
            """);
        UsingDirective[] expectedResult =
        [
            new("System", 1),
            new("System.IO", 1),
            new("System.Net.Http", 1),
            new("System.Threading.Tasks", 1),
        ];
        //Act
        var actualResult = await UsingCounter.CountAsync(project, ImmutableArray.Create("System", "System.IO", "System.Net.Http", "System.Threading.Tasks"));
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_SpecificUsingsNotFound_ContainsWithoutOccurrences()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using System;
            using System.Collections.Generic;
            using System.IO;
            using System.Linq;
            using System.Net.Http;
            using System.Threading;
            using System.Threading.Tasks;
            """);
        UsingDirective[] expectedResult =
        [
            new("System.Collections.Generic", 1),
            new("Not.Found", 0),
            new("System.Linq", 1),
            new("Duplicate", 0),
            new("System.Text", 0),
            new("System.Threading", 1),
        ];
        //Act
        var actualResult = await UsingCounter.CountAsync(project, ImmutableArray.Create("System.Collections.Generic", "Not.Found", "System.Linq", "Duplicate", "System.Text", "Duplicate", "System.Threading"));
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_GeneratedCodeAttribute_Ignore()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using System.CodeDom.Compiler;
            [assembly: GeneratedCodeAttribute("FlashOWare.Tool", "1.0.0.0")]
            """, """
            using System;
            using System.Collections.Generic;
            using System.IO;
            using System.Linq;
            using System.Net.Http;
            using System.Threading;
            using System.Threading.Tasks;
            """);
        UsingDirective[] expectedResult = [];
        //Act
        var actualResult = await UsingCounter.CountAsync(project);
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_AutoGeneratedDocuments_Ignore()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync(
            ("TemporaryGeneratedFile_File1.cs", "using System;"),
            ("File2.designer.cs", "using System;"),
            ("File3.generated.cs", "using System;"),
            ("File4.g.cs", "using System;"),
            ("File5.g.i.cs", "using System;"),
            ("File6.cs", "using System.IO;")
        );
        UsingDirective[] expectedResult =
        [
            new("System.IO", 1),
        ];
        //Act
        var actualResult = await UsingCounter.CountAsync(project);
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_AutoGeneratedTexts_Ignore()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            // <auto-generated>
            using System;
            """, """
            // <auto-generated/>
            using System;
            """, """
            /* <auto-generated> */
            using System;
            """, """
            /* <auto-generated/> */
            using System;
            """, """
            // auto-generated
            using System.Linq;
            """, """
            /* auto-generated */
            using System.Threading;
            """);
        UsingDirective[] expectedResult =
        [
            new("System.Linq", 1),
            new("System.Threading", 1),
        ];
        //Act
        var actualResult = await UsingCounter.CountAsync(project);
        //Assert
        ToolAssert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public async Task CountAsync_Error_Throws()
    {
        //Arrange
        var project = CreateProjectUnchecked("Hello, World!");
        //Act
        var result = () => UsingCounter.CountAsync(project);
        //Assert
        await Assert.ThrowsAsync<InvalidOperationException>(result);
    }

    [Fact]
    public async Task CountAsync_Cancel_Throws()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using System;
            """);
        CancellationToken cancellationToken = new(true);
        //Act
        var result = () => UsingCounter.CountAsync(project, cancellationToken);
        //Assert
        await Assert.ThrowsAsync<OperationCanceledException>(result);
    }

    [Fact]
    public async Task CountAsync_VisualBasic_Throws()
    {
        //Arrange
        var project = await VisualBasicFactory.CreateProjectCheckedAsync(("Program.vb", """
            Imports System

            Module Program
                Sub Main(args As String())
                    Console.WriteLine("Hello World!")
                End Sub
            End Module
            """));
        //Act
        var result = () => UsingCounter.CountAsync(project);
        //Assert
        await Assert.ThrowsAsync<InvalidOperationException>(result);
    }
}
