using FlashOWare.Tool.Core.Interceptors;
using FlashOWare.Tool.Core.Tests.Testing;
using Microsoft.CodeAnalysis.CSharp;
using Xunit.Abstractions;

namespace FlashOWare.Tool.Core.Tests.Interceptors;

public class InterceptorLocatorTests
{
    //TODO: Unit Test that uses actual [Generator] to generate the interceptors
    //TODO: Test [InterceptsLocation] with alias

    //TODO: Test corner cases
    // What if the Attribute-Arguments are not literals, but constant fields?
    // What if the Attribute is aliased
    // What if the Attribute is in a different namespace
    // What if the Interceptor / Intercepted are partial
    private readonly ITestOutputHelper output;

    public InterceptorLocatorTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public async Task ListAsync_()
    {
        //Arrange
        var project = await ProjectBuilder.CSharp(LanguageVersion.CSharp11)
            .AddDocument("CallSite.cs", """
                using System;

                namespace Test;

                class Program
                {
                    static void Main(string[] args)
                    {
                        Console.WriteLine("Hello, World!");
                        Intercepted.Foo();
                        MyClass.Bar(".NET");
                        MyClass.Baz("8.0");
                    }
                }
                """, Array.Empty<string>())
            .AddDocument("Intercepted.cs", """
                public static class Intercepted
                {
                    public static void Foo()
                    {
                    }
                }
                """, Array.Empty<string>())
            .AddDocument("MyClass.cs", """
                public static class MyClass
                {
                    public static void Bar(string text)
                    {
                    }
                
                    public static void Baz(string text)
                    {
                    }
                }
                """, Array.Empty<string>())
            .AddDocument("Interceptor.g.cs", """
                //------------------------------------------------------------------------------
                // <auto-generated>
                //     This code was generated by a tool.
                //
                //     Changes to this file may cause incorrect behavior and will be lost if
                //     the code is regenerated.
                // </auto-generated>
                //------------------------------------------------------------------------------
                #nullable enable

                namespace System.Runtime.CompilerServices
                {
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MyTest", "1.0.0.0")]
                    [global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = true)]
                    file sealed class InterceptsLocationAttribute : global::System.Attribute
                    {
                        public InterceptsLocationAttribute(string filePath, int line, int column)
                        {
                        }
                    }
                }

                namespace MyNamespace.Generated
                {
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MyTest", "1.0.0.0")]
                    file static class Interceptors
                    {
                        [global::System.Runtime.CompilerServices.InterceptsLocation(@"CallSite.cs", 10, 21)]
                        internal static void Interceptor0()
                        {
                        }

                        [global::System.Runtime.CompilerServices.InterceptsLocation(@"CallSite.cs", 11, 17)]
                        [global::System.Runtime.CompilerServices.InterceptsLocation(@"CallSite.cs", 12, 17)]
                        internal static void Interceptor1(string text)
                        {
                        }
                    }
                }
                """, Array.Empty<string>())
            .BuildCheckedAsync();
        //Act
        InterceptorList result = await InterceptorLocator.ListAsync(project);
        //Assert
        output.WriteLine(result.Result);
    }
}
