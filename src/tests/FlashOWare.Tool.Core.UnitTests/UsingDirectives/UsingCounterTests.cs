using FlashOWare.Tool.Core.UsingDirectives;

namespace FlashOWare.Tool.Core.UnitTests.UsingDirectives;

public class UsingCounterTests
{
    [Fact]
    public void Count_SingleUsing_FindsOne()
    {
        //Arrange
        var documents = new List<string> { """
            using System;
            """ };
        var expectedResult = new Dictionary<string, int>
        {
            { "System", 1 },
        };
        //Act
        var count = UsingCounter.Count(documents);
        //Assert
        Assert.Equal(expectedResult, count);
    }

    [Fact]
    public void Count_MultipleUsings_FindsMultiple()
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
        var count = UsingCounter.Count(documents);
        //Assert
        Assert.Equal(expectedResult, count);
    }

    [Fact]
    public void Count_SingleUsingInMultipleDocuments_FindsTwoOccurences()
    {
        //Arrange
        var documents = new List<string> { """
            using System;
            """, """
            using System;
            """ };
        var expectedResult = new Dictionary<string, int>
        {
            { "System", 2 },
        };
        //Act
        var count = UsingCounter.Count(documents);
        //Assert
        Assert.Equal(expectedResult, count);
    }

    [Fact]
    public void Count_MultipleUsingsInMultipleDocuments_FindsManyOccurences()
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
        var count = UsingCounter.Count(documents);
        //Assert
        Assert.Equal(expectedResult, count);
    }

    [Fact]
    public void Count_FileScopedNamespaces_FindsAllOccurences()
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
        var count = UsingCounter.Count(documents);
        //Assert
        Assert.Equal(expectedResult, count);
    }

    [Fact]
    public void Count_BlockScopedNamespaces_FindsAllOccurences()
    {
        //Arrange
        var documents = new List<string> { """
            //using static System;
            //using System.Console;
            //using MyAlias = System;
            using static System.Console;
            using MyAlias = System.Console;

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
        var expectedResult = new Dictionary<string, int>
        {
            { "static System.Console", 1 },
            { "System", 3 },
            { "System.Collections.Generic", 3 },
            { "System.IO", 2 },
            { "System.Linq", 2 },
            { "System.Net.Http", 1 },
            { "System.Threading", 2 },
            { "System.Threading.Tasks", 2 },
        };
        //Act
        var count = UsingCounter.Count(documents);
        //Assert
        Assert.Equal(expectedResult, count);
    }

    //TODO: explore AdhocWorkspace (empty workspace)
    //AdhocWorkspace.Create
    //no semantics, references to BCL, other projects
    //Basic.Reference.Assemblies
    //Basic.Reference.Assemblies.[TFM]
}
