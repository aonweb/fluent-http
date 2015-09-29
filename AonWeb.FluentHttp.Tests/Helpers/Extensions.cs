using System;
using System.Collections;
using System.Collections.Generic;
using Shouldly;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public static class Extensions
    {
        public static void ShouldBeEqual<T>(this T actual, T expected, IEqualityComparer comparer, string customMessage = null)
        {
            comparer.Equals(actual, expected).ShouldBeTrue();
        }
    }
}