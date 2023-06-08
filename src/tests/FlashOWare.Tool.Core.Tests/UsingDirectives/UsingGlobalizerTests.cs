using FlashOWare.Tool.Core.Tests.Assertions;
using FlashOWare.Tool.Core.Tests.Testing;
using FlashOWare.Tool.Core.UsingDirectives;
using Microsoft.CodeAnalysis.CSharp;

namespace FlashOWare.Tool.Core.Tests.UsingDirectives;

public class UsingGlobalizerTests
{
    private const string DefaultDocumentName = "GlobalUsings.cs";

    [Fact]
    public async Task GlobalizeAsync_Empty_NoReplace()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync();
        var expectedProject = await CreateProjectCheckedAsync();
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task GlobalizeAsync_NoLocalUsingDirectives_NoReplace()
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
        var expectedProject = await CreateProjectCheckedAsync("""
            internal class Program
            {
                private static void Main(string[] args)
                {
                    System.Console.WriteLine("Hello, World!");
                }
            }
            """);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task GlobalizeAsync_LocalUsingDirective_ReplacesWithGlobalUsingDirective()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using System;
            """);
        var expectedProject = await CreateProjectCheckedAsync(
            "", """
            global using System;

            """).WithDocumentNameAsync(^1, DefaultDocumentName);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task GlobalizeAsync_LocalUsingDirectives_ReplacesWithGlobalUsingDirective()
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
        var expectedProject = await CreateProjectCheckedAsync("""
            using System;
            using System.IO;
            using System.Linq;
            using System.Net.Http;
            using System.Threading;
            using System.Threading.Tasks;
            """, """
            global using System.Collections.Generic;

            """).WithDocumentNameAsync(^1, DefaultDocumentName);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System.Collections.Generic");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task GlobalizeAsync_MultipleDocuments_ReplacesWithGlobalUsingDirective()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using System;
            """, """
            using System;
            """);
        var expectedProject = await CreateProjectCheckedAsync(
            "",
            "", """
            global using System;

            """).WithDocumentNameAsync(^1, DefaultDocumentName);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task GlobalizeAsync_MultipleDocuments_ReplacesWithGlobalUsingDirectives()
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
        var expectedProject = await CreateProjectCheckedAsync("""
            using System;
            using System.IO;
            using System.Linq;
            using System.Net.Http;
            using System.Threading;
            using System.Threading.Tasks;
            """, """
            using System;
            using System.Linq;
            using System.Threading.Tasks;
            """, """
            using System;
            using System.IO;
            using System.Threading;
            """, """
            global using System.Collections.Generic;

            """).WithDocumentNameAsync(^1, DefaultDocumentName);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System.Collections.Generic");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task GlobalizeAsync_FileScopedNamespaces_ReplacesWithGlobalUsingDirective()
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
        var expectedProject = await CreateProjectCheckedAsync("""
            using System;
            using System.IO;
            using System.Linq;
            using System.Net.Http;
            using System.Threading;
            using System.Threading.Tasks;

            namespace MyNamespace;

            public class MyClass1 {}
            """, """
            using System;

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
            """, """
            global using System.Collections.Generic;

            """).WithDocumentNameAsync(^1, DefaultDocumentName);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System.Collections.Generic");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task GlobalizeAsync_BlockScopedNamespaces_ReplacesWithGlobalUsingDirective()
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
        var expectedProject = await CreateProjectCheckedAsync("""
            using System;
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
            """, """
            global using System.Collections.Generic;

            """).WithDocumentNameAsync(^1, DefaultDocumentName);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System.Collections.Generic");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Theory]
    [InlineData("System")]
    [InlineData("System.Console")]
    public async Task GlobalizeAsync_WithUsingAlias_DoNotReplace(string localUsing)
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using MyNamespace = System;
            using MyType = System.Console;
            """);
        var expectedProject = await CreateProjectCheckedAsync("""
            using MyNamespace = System;
            using MyType = System.Console;
            """);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, localUsing);
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task GlobalizeAsync_WithUsingStatic_DoNotReplace()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using static System.Console;
            """);
        var expectedProject = await CreateProjectCheckedAsync("""
            using static System.Console;
            """);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System.Console");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Theory]
    [InlineData("System")]
    [InlineData("System.Console")]
    public async Task GlobalizeAsync_WithGlobalUsings_DoNotReplace(string localUsing)
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            global using System;
            global using MyNamespace = System;
            global using MyType = System.Console;
            global using static System.Console;
            """);
        var expectedProject = await CreateProjectCheckedAsync("""
            global using System;
            global using MyNamespace = System;
            global using MyType = System.Console;
            global using static System.Console;
            """);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, localUsing);
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task GlobalizeAsync_GeneratedCodeAttribute_Ignore()
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
        var expectedProject = await CreateProjectCheckedAsync("""
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
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task GlobalizeAsync_AutoGeneratedDocuments_Ignore()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync(
            ("TemporaryGeneratedFile_File1.cs", "using System;"),
            ("File2.designer.cs", "using System;"),
            ("File3.generated.cs", "using System;"),
            ("File4.g.cs", "using System;"),
            ("File5.g.i.cs", "using System;"),
            ("File6.cs", "using System;")
        );
        var expectedProject = await CreateProjectCheckedAsync(
            ("TemporaryGeneratedFile_File1.cs", "using System;"),
            ("File2.designer.cs", "using System;"),
            ("File3.generated.cs", "using System;"),
            ("File4.g.cs", "using System;"),
            ("File5.g.i.cs", "using System;"),
            ("File6.cs", "using System;"),
            ("", """
                global using System;

                """)
            ).WithDocumentNameAsync(^1, DefaultDocumentName);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task GlobalizeAsync_AutoGeneratedTexts_Ignore()
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
            using System;
            """, """
            /* auto-generated */
            using System;
            """);
        var expectedProject = await CreateProjectCheckedAsync("""
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
            """, """
            /* auto-generated */
            """, """
            global using System;

            """).WithDocumentNameAsync(^1, DefaultDocumentName);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task GlobalizeAsync_Error_Throws()
    {
        //Arrange
        var project = CreateProjectUnchecked("Hello, World!");
        //Act
        var result = () => UsingGlobalizer.GlobalizeAsync(project, "System");
        //Assert
        await Assert.ThrowsAsync<InvalidOperationException>(result);
    }

    [Fact]
    public async Task GlobalizeAsync_Cancel_Throws()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using System;
            """);
        var cancellationToken = new CancellationToken(true);
        //Act
        var result = () => UsingGlobalizer.GlobalizeAsync(project, "System", cancellationToken);
        //Assert
        await Assert.ThrowsAsync<OperationCanceledException>(result);
    }

    [Fact]
    public async Task GlobalizeAsync_VisualBasic_Throws()
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
        var result = () => UsingGlobalizer.GlobalizeAsync(project, "System");
        //Assert
        await Assert.ThrowsAsync<InvalidOperationException>(result);
    }

    [Fact]
    public async Task GlobalizeAsync_UnsupportedLanguageVersion_Throws()
    {
        //Arrange
        var project = await ProjectBuilder.CSharp(LanguageVersion.CSharp9)
            .AddDocument("Document.cs", "using System;")
            .BuildCheckedAsync();
        //Act
        var result = () => UsingGlobalizer.GlobalizeAsync(project, "System");
        //Assert
        await Assert.ThrowsAsync<InvalidOperationException>(result);
    }

    [Fact]
    public async Task GlobalizeAsync_Exists_Append()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            global using System;

            """, """
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;
            """).WithDocumentNameAsync(0, DefaultDocumentName);
        var expectedProject = await CreateProjectCheckedAsync("""
            global using System;
            global using System.Linq;

            """, """
            using System.Collections.Generic;
            using System.Text;
            using System.Threading.Tasks;
            """).WithDocumentNameAsync(0, DefaultDocumentName);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System.Linq");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }
}