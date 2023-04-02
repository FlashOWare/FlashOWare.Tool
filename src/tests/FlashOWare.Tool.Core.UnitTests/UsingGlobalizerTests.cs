namespace FlashOWare.Tool.Core.UnitTests;

public class UsingGlobalizerTests
{
    [Fact]
    public void Globalize_LocalUsingDirective_ReplacesWithGlobalUsingDirective()
    {
        //Arrange
        var documents = new List<string> { """
            using System;
            """ };
        var expectedResult = new List<string> {
            "", """
            global using System;
            """ };
        //Act
        var actualProject = UsingGlobalizer.Globalize(documents, "System");
        //Assert
        Assert.Equal(expectedResult, actualProject);
    }

    [Fact]
    public void Globalize_LocalUsingDirectives_ReplacesWithGlobalUsingDirective()
    {
        //Arrange
        var documents = new List<string> { """
            using System;
            using System.Collections.Generic;
            using System.IO;
            using System.Linq;
            using System.Net.Http;
            using System.Threading;
            using System.Threading.Tasks;
            """ };
        var expectedResult = new List<string> { """
            using System;
            using System.IO;
            using System.Linq;
            using System.Net.Http;
            using System.Threading;
            using System.Threading.Tasks;
            """, """
            global using System.Collections.Generic;
            """ };
        //Act
        var actualProject = UsingGlobalizer.Globalize(documents, "System.Collections.Generic");
        //Assert
        Assert.Equal(expectedResult, actualProject);
    }

    [Fact]
    public void Globalize_MultipleDocuments_ReplacesWithGlobalUsingDirective()
    {
        //Arrange
        var documents = new List<string> { """
            using System;
            """, """
            using System;
            """ };
        var expectedResult = new List<string> {
            "",
            "", """
            global using System;
            """ };
        //Act
        var actualProject = UsingGlobalizer.Globalize(documents, "System");
        //Assert
        Assert.Equal(expectedResult, actualProject);
    }

    [Fact]
    public void Globalize_MultipleDocuments_ReplacesWithGlobalUsingDirectives()
    {
        //Arrange
        var documents = new List<string> { """
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
            """ };
        var expectedResult = new List<string> { """
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
            """ };
        //Act
        var actualProject = UsingGlobalizer.Globalize(documents, "System.Collections.Generic");
        //Assert
        Assert.Equal(expectedResult, actualProject);
    }

    [Fact]
    public void Globalize_FileScopedNamespaces_ReplacesWithGlobalUsingDirective()
    {
        //Arrange
        var documents = new List<string> { """
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
            """ };
        var expectedResult = new List<string> { """
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
            """ };
        //Act
        var actualProject = UsingGlobalizer.Globalize(documents, "System.Collections.Generic");
        //Assert
        Assert.Equal(expectedResult, actualProject);
    }

    [Fact]
    public void Globalize_BlockScopedNamespaces_ReplacesWithGlobalUsingDirective()
    {
        //Arrange
        var documents = new List<string> { """
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
            """ };
        var expectedResult = new List<string> { """
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
            """ };
        //Act
        var actualProject = UsingGlobalizer.Globalize(documents, "System.Collections.Generic");
        //Assert
        Assert.Equal(expectedResult, actualProject);
    }
}
