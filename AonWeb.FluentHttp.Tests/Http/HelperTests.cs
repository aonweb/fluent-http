using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading;

using Moq;

using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Http
{
    [TestFixture]
    public class HelperTests
    {
        [Test]
        public void MergeAction_WhenCalled_ExpectMerge()
        {
            var action1Called = new DateTime?();
            var action2Called = new DateTime?();
            Action<int> action1 = i => { action1Called = DateTime.Now; Thread.Sleep(1); };
            Action<int> action2 = i => { action2Called = DateTime.Now; Thread.Sleep(1); };

            var actual = Helper.MergeAction(action1, action2);

            actual(1);

            Assert.NotNull(action1Called, "Action 1 was not called");
            Assert.NotNull(action2Called, "Action 2 was not called");
        }

        [Test]
        public void MergeAction_WhenCalled_ExpectCorrectOrder()
        {
            var action1Called = new DateTime?();
            var action2Called = new DateTime?();
            Action<int> action1 = i => { action1Called = DateTime.Now; Thread.Sleep(1); };
            Action<int> action2 = i => { action2Called = DateTime.Now; Thread.Sleep(1); };

            var actual = Helper.MergeAction(action1, action2);

            actual(1);

            Assert.Less(action1Called, action2Called);
        }

        [Test]
        public void MergeAction_WhenCalledWithNulls_ExpectNoException()
        {
            var actual = Helper.MergeAction<int>(null, null);

            actual(1);

            Assert.Pass();
        }

        [Test]
        public void MergeAction_WhenCalledWithFirstNull_ExpectNoException()
        {
            Action<int> action = i => { };

            var actual = Helper.MergeAction(null, action);

            actual(1);

            Assert.Pass();
        }

        [Test]
        public void MergeAction_WhenCalledWithSecondNull_ExpectNoException()
        {
            Action<int> action = i => { };

            var actual = Helper.MergeAction(action, null);

            actual(1);

            Assert.Pass();
        }

        [Test]
        public void AddDistinct_WhenAddSameHeaderTwice_ExpectExpectSetOnce()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://foo.com");

            //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Assert.AreEqual(1, request.Headers.Accept.Count);

            request.Headers.AcceptEncoding.AddDistinct(h => h.Value, "gzip");
            request.Headers.AcceptEncoding.AddDistinct(h => h.Value, "gzip");

            Assert.AreEqual(1, request.Headers.AcceptEncoding.Count);

        }
    }
}
