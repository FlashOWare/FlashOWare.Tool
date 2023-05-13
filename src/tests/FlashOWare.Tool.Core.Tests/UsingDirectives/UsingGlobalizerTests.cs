using FlashOWare.Tool.Core.Tests.Assertions;
using FlashOWare.Tool.Core.UsingDirectives;

namespace FlashOWare.Tool.Core.Tests.UsingDirectives;

public class UsingGlobalizerTests
{
    [Fact]
    public async Task Globalize_NoLocalUsingDirectives_NoReplace()
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
    public async Task Globalize_LocalUsingDirective_ReplacesWithGlobalUsingDirective()
    {
        //Arrange
        var project = await CreateProjectCheckedAsync("""
            using System;
            """);
        var expectedProject = await CreateProjectCheckedAsync(
            "", """
            global using System;

            """);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task Globalize_LocalUsingDirectives_ReplacesWithGlobalUsingDirective()
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

            """);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System.Collections.Generic");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task Globalize_MultipleDocuments_ReplacesWithGlobalUsingDirective()
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

            """);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task Globalize_MultipleDocuments_ReplacesWithGlobalUsingDirectives()
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

            """);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System.Collections.Generic");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task Globalize_FileScopedNamespaces_ReplacesWithGlobalUsingDirective()
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
            using System.IO;
            using System.Threading;

            public class MyClass3 {}
            """, """
            global using System.Collections.Generic;

            """);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System.Collections.Generic");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }

    [Fact]
    public async Task Globalize_BlockScopedNamespaces_ReplacesWithGlobalUsingDirective()
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
                using System.IO;
                using System.Threading;

                public class MyClass3 {}
            }
            """, """
            global using System.Collections.Generic;

            """);
        //Act
        var actualProject = await UsingGlobalizer.GlobalizeAsync(project, "System.Collections.Generic");
        //Assert
        await RoslynAssert.EqualAsync(expectedProject, actualProject);
    }
}
