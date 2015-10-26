using System;
using System.Collections.Generic;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Serialization;
using AonWeb.FluentHttp.HAL.Serialization.Exceptions;
using AonWeb.FluentHttp.Tests.Helpers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests.Hal
{
    public class HypermediaLinkHelpersTest
    {
        private readonly ITestOutputHelper _logger;

        public HypermediaLinkHelpersTest(ITestOutputHelper logger)
        {
            _logger = logger;
        }
        private const string DefaultRel = "mylink";
        private const string DefaultHref = "http://somesite.com/resource/1";
        private const string DefaultSingleTemplatedHref = "http://somesite.com/resource/{token1}";
        private const string DefaultMultipleTemplatedHref = "http://somesite.com/resource/{token1}?qs={token2}";
        private const string DefaultToken1 = "token1";
        private const string DefaultToken2 = "token2";
        private const string DefaultToken1Value = "12345";
        private const string DefaultToken2Value = "54321";

        private static readonly Uri DefaultUri = new Uri(DefaultHref);
        private static readonly Uri DefaultSingleTemplatedUri = new Uri("http://somesite.com/resource/12345");
        private static readonly Uri DefaultMultipleTemplatedUri = new Uri("http://somesite.com/resource/12345?qs=54321");


        #region HasLink

        [Fact]
        public void HasLinkOnResource_WithValidResourceWithLinks_ExpectTrue()
        {
            var resource = CreateResource();

            var actual = resource.HasLink(DefaultRel);

            actual.ShouldBeTrue();
        }

        [Fact]
        public void HasLinkOnResource_WithValidResourceWithLinksWhenKeyDifferentCase_ExpectTrue()
        {
            var resource = CreateResource();

            var actual = resource.HasLink(DefaultRel.ToUpper());

            actual.ShouldBeTrue();
        }

        [Fact]
        public void HasLinkOnResource_WithValidResourceWithLinksWhenKeyDoesNotExist_ExpectFalse()
        {
            var resource = CreateResource();

            var actual = resource.HasLink("themissinglink");

            actual.ShouldBeFalse();
        }

        [Fact]
        public void HasLinkOnResource_WithNullResource_ExpectFalse()
        {
            var actual = ((IHalResource)null).HasLink(DefaultRel);

            actual.ShouldBeFalse();
        }

        [Fact]
        public void HasLinkOnResource_WithValidResourceWithNullLinks_ExpectFalse()
        {
            var resource = new TestResource();

            var actual = resource.HasLink(DefaultRel);

            actual.ShouldBeFalse();
        }

        [Fact]
        public void HasLinkOnResource_WithValidResourceWithEmptyLinks_ExpectFalse()
        {
            var resource = new TestResource
            {
                Links = new HyperMediaLinks()
            };

            var actual = resource.HasLink(DefaultRel);

            actual.ShouldBeFalse();
        }

        [Fact]
        public void HasLinkOnResource_WithValidResourceWithLinksWhenKeyIsNull_ExpectFalse()
        {
            var resource = CreateResource();

            var actual = resource.HasLink(null);

            actual.ShouldBeFalse();
        }

        [Fact]
        public void HasLinkOnResource_WithValidResourceWithLinksWhenKeyIsEmpty_ExpectFalse()
        {
            var resource = CreateResource();

            var actual = resource.HasLink(string.Empty);

            actual.ShouldBeFalse();
        }

        [Fact]
        public void HasLinkOnLinks_WithValidLinks_ExpectTrue()
        {
            var links = CreateLinks();

            var actual = links.HasLink(DefaultRel);

            actual.ShouldBeTrue();
        }

        [Fact]
        public void HasLinkOnLinks_WithValidLinksWhenKeyDifferentCase_ExpectTrue()
        {
            var resource = CreateLinks();

            var actual = resource.HasLink(DefaultRel.ToUpper());

            actual.ShouldBeTrue();
        }

        [Fact]
        public void HasLinkOnLinks_WithValidLinksWhenKeyDoesNotExist_ExpectFalse()
        {
            var links = CreateLinks();

            var actual = links.HasLink("themissinglink");

            actual.ShouldBeFalse();
        }

        [Fact]
        public void HasLinkOnLinks_WithNullLinks_ExpectFalse()
        {
            var actual = ((HyperMediaLinks)null).HasLink(DefaultRel);

            actual.ShouldBeFalse();
        }

        [Fact]
        public void HasLinkOnLinks_WithEmptyLinks_ExpectFalse()
        {
            var actual = new HyperMediaLinks().HasLink(DefaultRel);

            actual.ShouldBeFalse();
        }

        [Fact]
        public void HasLinkOnLinks_WithValidLinksWhenKeyIsNull_ExpectFalse()
        {
            var links = CreateLinks();

            var actual = links.HasLink(null);

            actual.ShouldBeFalse();
        }

        [Fact]
        public void HasLinkOnLinks_WithValidLinksWhenKeyIsEmpty_ExpectFalse()
        {
            var links = CreateLinks();

            var actual = links.HasLink(string.Empty);

            actual.ShouldBeFalse();
        }


        #endregion

        #region GetLink

        #region GetLink Non-templated on resource

        [Fact]
        public void GetNonTemplatedLinkOnResource_WithValidResourceWithLinks_ExpectCorrectLink()
        {
            var resource = CreateResource();

            var actual = resource.GetLink(DefaultRel);

            actual.ShouldBe(DefaultUri);
        }

        [Fact]
        public void GetNonTemplatedLinkOnResource_WithValidResourceWithLinksWhenKeyDifferentCase_ExpectCorrectLink()
        {
            var resource = CreateResource();

            var actual = resource.GetLink(DefaultRel.ToUpper());

            actual.ShouldBe(DefaultUri);
        }

        [Fact]
        public void GetNonTemplatedLinkOnResource_WithValidResourceWithLinksWhenUriInvalid_ExpectException()
        {
            var resource = CreateResource("invalid;uri");

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink(DefaultRel));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetNonTemplatedLinkOnResource_WithValidResourceWithLinksWhenKeyDoesNotExist_ExpectException()
        {
            var resource = CreateResource();

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink("themissinglink"));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetNonTemplatedLinkOnResource_WithNullResource_ExpectException()
        {
            var ex = Should.Throw<MissingLinkException>(() => ((IHalResource)null).GetLink(DefaultRel));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetNonTemplatedLinkOnResource_WithValidResourceWithNullLinks_ExpectException()
        {
            var resource = new TestResource();

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink(DefaultRel));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetNonTemplatedLinkOnResource_WithValidResourceWithEmptyLinks_ExpectException()
        {
            var resource = new TestResource
            {
                Links = new HyperMediaLinks()
            };

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink(DefaultRel));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetNonTemplatedLinkOnResource_WithValidResourceWithLinksWhenKeyIsNull_ExpectException()
        {
            var resource = CreateResource();

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink(null));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetNonTemplatedLinkOnResource_WithValidResourceWithLinksWhenKeyIsEmpty_ExpectException()
        {
            var resource = CreateResource();

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink(string.Empty));
            _logger.WriteLine(ex.Message);
        }

        #endregion

        #region GetLink Non-templated on link

        [Fact]
        public void GetNonTemplatedLinkOnLinks_WithValidLinks_ExpectCorrectLink()
        {
            var links = CreateLinks();

            var actual = links.GetLink(DefaultRel);

            actual.ShouldBe(DefaultUri);
        }

        [Fact]
        public void GetNonTemplatedLinkOnLinks_WithValidLinksWhenKeyDifferentCase_ExpectCorrectLink()
        {
            var links = CreateLinks();

            var actual = links.GetLink(DefaultRel.ToUpper());

            actual.ShouldBe(DefaultUri);
        }

        [Fact]
        public void GetNonTemplatedLinkOnLinks_WithValidLinksWhenKeyDoesNotExist_ExpectException()
        {
            var links = CreateLinks();

            var ex = Should.Throw<MissingLinkException>(() => links.GetLink("themissinglink"));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetNonTemplatedLinkOnLinks_WithNullLinks_ExpectException()
        {
            var ex = Should.Throw<MissingLinkException>(() => ((HyperMediaLinks)null).GetLink(DefaultRel));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetNonTemplatedLinkOnLinks_WithEmptyLinks_ExpectException()
        {
            var ex = Should.Throw<MissingLinkException>(() => new HyperMediaLinks().GetLink(DefaultRel));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetNonTemplatedLinkOnLinks_WithValidLinksWhenKeyIsNull_ExpectException()
        {
            var links = CreateLinks();

            var ex = Should.Throw<MissingLinkException>(() => links.GetLink(null));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetNonTemplatedLinkOnLinks_WithValidLinksWhenKeyIsEmpty_ExpectException()
        {
            var links = CreateLinks();

            var ex = Should.Throw<MissingLinkException>(() => links.GetLink(string.Empty));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetNonTemplatedLinkOnLinks_WithValidLinksWhenLinkIsTemplated_ExpectException()
        {
            var links = CreateLinks(DefaultSingleTemplatedHref, true);

            var ex = Should.Throw<MissingLinkException>(() => links.GetLink(DefaultRel));
            _logger.WriteLine(ex.Message);
        }

        #endregion

        #region GetLink templated on resource

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinkWithSingleTemplate_ExpectCorrectLink()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var actual = resource.GetLink(DefaultRel, DefaultToken1, DefaultToken1Value);

            actual.ShouldBe(DefaultSingleTemplatedUri);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinkWithMultipleTemplate_ExpectCorrectLink()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var actual = resource.GetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            });

            actual.ShouldBe(DefaultMultipleTemplatedUri);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinkWithSingleTemplateWithKeyInDifferentCase_ExpectCorrectLink()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var actual = resource.GetLink(DefaultRel.ToUpper(), DefaultToken1, DefaultToken1Value);

            actual.ShouldBe(DefaultSingleTemplatedUri);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinkWithMultipleTemplateWithKeyInDifferentCase_ExpectCorrectLink()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var actual = resource.GetLink(DefaultRel.ToUpper(), new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            });

            actual.ShouldBe(DefaultMultipleTemplatedUri);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenUriInvalid_ExpectException()
        {
            var resource = CreateResource("invalid;uri{token1}", true);

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink(DefaultRel, DefaultToken1, DefaultToken1Value));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenUriInvalid_ExpectException()
        {
            var resource = CreateResource("invalid;uri{token1}{token2}", true);

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            }));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenKeyDoesNotExist_ExpectException()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink("themissinglink", new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            }));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenKeyDoesNotExist_ExpectException()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink("themissinglink", new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            }));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithNullResourceWithSingleTemplate_ExpectException()
        {
            var ex = Should.Throw<MissingLinkException>(() => ((IHalResource)null).GetLink(DefaultRel, DefaultToken1, DefaultToken1Value));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithNullResourceWithMultipleTemplate_ExpectException()
        {
            var ex = Should.Throw<MissingLinkException>(() => ((IHalResource)null).GetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            }));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithNullLinksWithSingleTemplate_ExpectException()
        {
            var resource = new TestResource();

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink(DefaultRel, DefaultToken1, DefaultToken1Value));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithNullLinksWithMultipleTemplate_ExpectException()
        {
            var resource = new TestResource();

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            }));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithEmptyLinksWithSingleTemplate_ExpectException()
        {
            var resource = new TestResource
            {
                Links = new HyperMediaLinks()
            };

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink(DefaultRel, DefaultToken1, DefaultToken1Value));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithEmptyLinksWithMultipleTemplate_ExpectException()
        {
            var resource = new TestResource
            {
                Links = new HyperMediaLinks()
            };

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            }));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenKeyIsNull_ExpectException()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink(null, DefaultToken1, DefaultToken1Value));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenKeyIsNull_ExpectException()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink(null, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            }));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenKeyIsEmpty_ExpectException()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink(string.Empty, DefaultToken1, DefaultToken1Value));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenKeyIsEmpty_ExpectException()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var ex = Should.Throw<MissingLinkException>(() => resource.GetLink(string.Empty, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            }));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenTokenIsNull_ExpectException()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var ex = Should.Throw<InvalidTokenException>(() => resource.GetLink(DefaultRel, null, DefaultToken1Value));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenTokenIsEmpty_ExpectException()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var ex = Should.Throw<InvalidTokenException>(() => resource.GetLink(DefaultRel, string.Empty, DefaultToken1Value));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenTokenIsEmpty_ExpectException()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var ex = Should.Throw<InvalidTokenException>(() => resource.GetLink(DefaultRel, new Dictionary<string, object>
            {
                { string.Empty, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            }));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenTokensAreNull_ExpectException()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var ex = Should.Throw<InvalidTokenException>(() => resource.GetLink(DefaultRel, null));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenTokenMissing_ExpectException()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var ex = Should.Throw<InvalidTokenException>(() => resource.GetLink(DefaultRel, "missingtoken", DefaultToken1Value));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenTokenMissing_ExpectException()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var ex = Should.Throw<InvalidTokenException>(() => resource.GetLink(DefaultRel, new Dictionary<string, object>
            {
                { "missingtoken", DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            }));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenExtraToken_ExpectException()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var ex = Should.Throw<InvalidTokenException>(() => resource.GetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            }));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenExtraToken_ExpectException()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var ex = Should.Throw<InvalidTokenException>(() => resource.GetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value },
                { "extratoken", "extratokenvalue" }
            }));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenLinkIsNotTemplated_ExpectException()
        {
            var resource = CreateResource();

            var ex = Should.Throw<InvalidTokenException>(() => resource.GetLink(DefaultRel, DefaultToken1, DefaultToken1Value));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenLinkIsNotTemplated_ExpectException()
        {
            var resource = CreateResource();

            var ex = Should.Throw<InvalidTokenException>(() => resource.GetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            }));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenTokenValueIsNull_ExpectNoExceptionAndCorrectLink()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var actual = resource.GetLink(DefaultRel, DefaultToken1, null);
            actual.ShouldBe(new Uri("http://somesite.com/resource/"));
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenTokenValueIsNull_ExpectNoExceptionAndCorrectLink()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var actual = resource.GetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, null },
                { DefaultToken2, string.Empty }
            });

            actual.ShouldBe(new Uri("http://somesite.com/resource/?qs="));
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenMultipleTokens_ExpectException()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref);

            var ex = Should.Throw<InvalidTokenException>(() => resource.GetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            }));
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenSingleTokens_ExpectException()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref);

            var ex = Should.Throw<InvalidTokenException>(() => resource.GetLink(DefaultRel, DefaultToken1, DefaultToken1Value));
            _logger.WriteLine(ex.Message);
        }

        #endregion

        #endregion

        #region TryGetLink

        #region TryGetLink Non-templated on resource

        [Fact]
        public void TryGetNonTemplatedLinkOnResource_WithValidResourceWithLinks_ExpectCorrectLink()
        {
            var resource = CreateResource();

            var actual = resource.TryGetLink(DefaultRel);

            actual.ShouldBe(DefaultUri);
        }

        [Fact]
        public void TryGetNonTemplatedLinkOnResource_WithValidResourceWithLinksWhenKeyDifferentCase_ExpectCorrectLink()
        {
            var resource = CreateResource();

            var actual = resource.TryGetLink(DefaultRel.ToUpper());

            actual.ShouldBe(DefaultUri);
        }

        [Fact]
        public void TryGetNonTemplatedLinkOnResource_WithValidResourceWithLinksWhenUriInvalid_ExpectNull()
        {
            var resource = CreateResource("invalid;uri");

            var actual = resource.TryGetLink(DefaultRel);

           actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetNonTemplatedLinkOnResource_WithValidResourceWithLinksWhenKeyDoesNotExist_ExpectNoExceptionAndNull()
        {
            var resource = CreateResource();

            var actual = resource.TryGetLink("themissinglink");

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetNonTemplatedLinkOnResource_WithNullResource_ExpectNull()
        {
            var actual = ((IHalResource)null).TryGetLink(DefaultRel);

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetNonTemplatedLinkOnResource_WithValidResourceWithNullLinks_ExpectNoExceptionAndNull()
        {
            var resource = new TestResource();

            var actual = resource.TryGetLink(DefaultRel);

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetNonTemplatedLinkOnResource_WithValidResourceWithEmptyLinks_ExpectNoExceptionAndNull()
        {
            var resource = new TestResource
            {
                Links = new HyperMediaLinks()
            };

            var actual = resource.TryGetLink(DefaultRel);

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetNonTemplatedLinkOnResource_WithValidResourceWithLinksWhenKeyIsNull_ExpectNoExceptionAndNull()
        {
            var resource = CreateResource();

            var actual = resource.TryGetLink(null);

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetNonTemplatedLinkOnResource_WithValidResourceWithLinksWhenKeyIsEmpty_ExpectNoExceptionAndNull()
        {
            var resource = CreateResource();

            var actual = resource.TryGetLink(string.Empty);

            actual.ShouldBeNull();
        }

        #endregion

        #region TryGetLink Non-templated on link

        [Fact]
        public void TryGetNonTemplatedLinkOnLinks_WithValidLinks_ExpectCorrectLink()
        {
            var links = CreateLinks();

            var actual = links.TryGetLink(DefaultRel);

            actual.ShouldBe(DefaultUri);
        }

        [Fact]
        public void TryGetNonTemplatedLinkOnLinks_WithValidLinksWhenKeyDifferentCase_ExpectCorrectLink()
        {
            var links = CreateLinks();

            var actual = links.TryGetLink(DefaultRel.ToUpper());

            actual.ShouldBe(DefaultUri);
        }

        [Fact]
        public void TryGetNonTemplatedLinkOnLinks_WithValidLinksWhenKeyDoesNotExist_ExpectNoExceptionAndNull()
        {
            var links = CreateLinks();

            var actual = links.TryGetLink("themissinglink");

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetNonTemplatedLinkOnLinks_WithNullLinks_ExpectNoExceptionAndNull()
        {
            var actual = ((HyperMediaLinks)null).TryGetLink(DefaultRel);

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetNonTemplatedLinkOnLinks_WithEmptyLinks_ExpectNoExceptionAndNull()
        {
            var actual = new HyperMediaLinks().TryGetLink(DefaultRel);

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetNonTemplatedLinkOnLinks_WithValidLinksWhenKeyIsNull_ExpectNoExceptionAndNull()
        {
            var links = CreateLinks();

            var actual = links.TryGetLink(null);

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetNonTemplatedLinkOnLinks_WithValidLinksWhenKeyIsEmpty_ExpectNoExceptionAndNull()
        {
            var links = CreateLinks();

            var actual = links.TryGetLink(string.Empty);

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetNonTemplatedLinkOnLinks_WithValidLinksWhenLinkIsTemplated_ExpectNoExceptionAndTemplatedLink()
        {
            var links = CreateLinks(DefaultSingleTemplatedHref, true);

            var actual = links.TryGetLink(DefaultRel);

            actual.ShouldBe(new Uri("http://somesite.com/resource/{token1}"));
        }

        #endregion

        #region TryGetLink templated on resource

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinkWithSingleTemplate_ExpectCorrectLink()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var actual = resource.TryGetLink(DefaultRel, DefaultToken1, DefaultToken1Value);

            actual.ShouldBe(DefaultSingleTemplatedUri);
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinkWithMultipleTemplate_ExpectCorrectLink()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var actual = resource.TryGetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            });

            actual.ShouldBe(DefaultMultipleTemplatedUri);
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinkWithSingleTemplateWithKeyInDifferentCase_ExpectCorrectLink()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var actual = resource.TryGetLink(DefaultRel.ToUpper(), DefaultToken1, DefaultToken1Value);

            actual.ShouldBe(DefaultSingleTemplatedUri);
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinkWithMultipleTemplateWithKeyInDifferentCase_ExpectCorrectLink()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var actual = resource.TryGetLink(DefaultRel.ToUpper(), new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            });

            actual.ShouldBe(DefaultMultipleTemplatedUri);
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenUriInvalid_ExpectNull()
        {
            var resource = CreateResource("invalid;uri{token1}");

            var actual = resource.TryGetLink(DefaultRel, DefaultToken1, DefaultToken1Value);

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenUriInvalid_ExpectNull()
        {
            var resource = CreateResource("invalid;uri{token1}{token2}");

            var actual = resource.TryGetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            });

            actual.ShouldBeNull();
        }


        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenKeyDoesNotExist_ExpectNoExceptionAndNull()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var actual = resource.TryGetLink("themissinglink", DefaultToken1, DefaultToken1Value);

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenKeyDoesNotExist_ExpectNoExceptionAndNull()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var actual = resource.TryGetLink("themissinglink", new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            });

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithNullResourceWithSingleTemplate_ExpectNoExceptionAndNull()
        {
            var actual = ((IHalResource)null).TryGetLink(DefaultRel, DefaultToken1, DefaultToken1Value);

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithNullResourceWithMultipleTemplate_ExpectNoExceptionAndNull()
        {
            var actual = ((IHalResource)null).TryGetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            });

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithNullLinksWithSingleTemplate_ExpectNoExceptionAndNull()
        {
            var resource = new TestResource();

            var actual = resource.TryGetLink(DefaultRel, DefaultToken1, DefaultToken1Value);

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithNullLinksWithMultipleTemplate_ExpectNoExceptionAndNull()
        {
            var resource = new TestResource();

            var actual = resource.TryGetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            });

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithEmptyLinksWithSingleTemplate_ExpectNoExceptionAndNull()
        {
            var resource = new TestResource
            {
                Links = new HyperMediaLinks()
            };

            var actual = resource.TryGetLink(DefaultRel, DefaultToken1, DefaultToken1Value);

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithEmptyLinksWithMultipleTemplate_ExpectNoExceptionAndNull()
        {
            var resource = new TestResource
            {
                Links = new HyperMediaLinks()
            };

            var actual = resource.TryGetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            });

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenKeyIsNull_ExpectNoExceptionAndNull()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var actual = resource.TryGetLink(null, DefaultToken1, DefaultToken1Value);

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenKeyIsNull_ExpectNoExceptionAndNull()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var actual = resource.TryGetLink(null, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            });

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenKeyIsEmpty_ExpectNoExceptionAndNull()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var actual = resource.TryGetLink(string.Empty, DefaultToken1, DefaultToken1Value);

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenKeyIsEmpty_ExpectNoExceptionAndNull()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var actual = resource.TryGetLink(string.Empty, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            });

            actual.ShouldBeNull();
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenTokenIsNull_ExpectNoExceptionAndLink()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var actual = resource.TryGetLink(DefaultRel, null, DefaultToken1Value);

            actual.ShouldBe(new Uri(DefaultSingleTemplatedHref));
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenTokenIsEmpty_ExpectNoExceptionAndLink()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var actual = resource.TryGetLink(DefaultRel, string.Empty, DefaultToken1Value);

            actual.ShouldBe(new Uri(DefaultSingleTemplatedHref));
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenTokenIsEmpty_ExpectNoExceptionAndLink()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var actual = resource.TryGetLink(DefaultRel, new Dictionary<string, object>
            {
                { string.Empty, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            });

            actual.ShouldBe(new Uri("http://somesite.com/resource/{token1}?qs=54321"));
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenTokensAreNull_ExpectNoExceptionAndLink()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var actual = resource.TryGetLink(DefaultRel, null);

            actual.ShouldBe(new Uri(DefaultMultipleTemplatedHref));
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenTokenMissing_ExpectNoExceptionAndLink()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var actual = resource.TryGetLink(DefaultRel, "missingtoken", DefaultToken1Value);

            actual.ShouldBe(new Uri(DefaultSingleTemplatedHref));
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenTokenMissing_ExpectNoExceptionAndLink()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var actual = resource.TryGetLink(DefaultRel, new Dictionary<string, object>
            {
                { "missingtoken", DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            });

            actual.ShouldBe(new Uri("http://somesite.com/resource/{token1}?qs=54321"));
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenExtraToken_ExpectNoExceptionAndLink()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var actual = resource.TryGetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            });

            actual.ShouldBe(DefaultSingleTemplatedUri);
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenExtraToken_ExpectNoExceptionAndLink()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var actual = resource.TryGetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value },
                { "extratoken", "extratokenvalue" }
            });

            actual.ShouldBe(DefaultMultipleTemplatedUri);
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenLinkIsNotTemplated_ExpectNoExceptionAndLink()
        {
            var resource = CreateResource();

            var actual = resource.TryGetLink(DefaultRel, DefaultToken1, DefaultToken1Value);

            actual.ShouldBe(DefaultUri);
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenLinkIsNotTemplated_ExpectNoExceptionAndLink()
        {
            var resource = CreateResource();

            var actual = resource.TryGetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            });

            actual.ShouldBe(DefaultUri);
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenTokenValueIsNull_ExpectNoExceptionAndCorrectLink()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref, true);

            var actual = resource.TryGetLink(DefaultRel, DefaultToken1, null);
            actual.ShouldBe(new Uri("http://somesite.com/resource/"));
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenTokenValueIsNull_ExpectNoExceptionAndCorrectLink()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref, true);

            var actual = resource.TryGetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, null },
                { DefaultToken2, string.Empty }
            });

            actual.ShouldBe(new Uri("http://somesite.com/resource/?qs="));
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithSingleTemplateWhenMultipleTokens_ExpectNoExceptionAndLink()
        {
            var resource = CreateResource(DefaultSingleTemplatedHref);

            var actual = resource.TryGetLink(DefaultRel, new Dictionary<string, object>
            {
                { DefaultToken1, DefaultToken1Value },
                { DefaultToken2, DefaultToken2Value }
            });

            actual.ShouldBe(DefaultSingleTemplatedUri);
        }

        [Fact]
        public void TryGetTemplatedLinkOnResource_WithValidResourceWithLinksWithMultipleTemplateWhenSingleTokens_ExpectNoExceptionAndLink()
        {
            var resource = CreateResource(DefaultMultipleTemplatedHref);

            var actual = resource.TryGetLink(DefaultRel, DefaultToken1, DefaultToken1Value);

            actual.ShouldBe(new Uri(" http://somesite.com/resource/12345?qs={token2}"));
        }

        #endregion

        #region GetLink templated on link



        #endregion

        //public static Uri GetLink(this IHalResource resource, string key, string tokenKey, object tokenValue)
        //{
        //    return resource.GetLink(key, new Dictionary<string, object> { { tokenKey, tokenValue } });
        //}

        //public static Uri GetLink(this IHalResource resource, string key, IDictionary<string, object> tokens)
        //{
        //    if (resource == null)
        //        throw new ArgumentNullException(nameof(resource));

        //    return resource.Links.GetLink(key, tokens);
        //}

        //public static Uri GetLink(this IEnumerable<HyperMediaLink> linkEntity, string key, string tokenKey, object tokenValue)
        //{
        //    return linkEntity.GetLink(key, new Dictionary<string, object> { { tokenKey, tokenValue } });
        //}

        //public static Uri GetLink(this IEnumerable<HyperMediaLink> linkEntity, string key, IDictionary<string, object> tokens)
        //{
        //    var url = linkEntity.GetTemplatedLinkString(key, tokens);

        //    return new Uri(url);
        //}

        #endregion

        #region GetSelf

        #region GetSelf on resource

        [Fact]
        public void GetSelfOnResource_WithValidResourceWithLinks_ExpectCorrectLink()
        {
            var resource = CreateResource();

            var actual = resource.GetSelf();

            actual.ShouldBe(DefaultUri);
        }

        [Fact]
        public void GetSelfOnResource_WithValidResourceWithLinksWhenKeyDoesNotExist_ExpectException()
        {
            var resource = CreateResource(withSelf:false);

            var ex = Should.Throw<MissingLinkException>(() => resource.GetSelf());
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetSelfOnResource_WithValidResourceWithLinksWhenUriInvalid_ExpectException()
        {
            var resource = CreateResource(selfHref:"invalid;uri");

            var ex = Should.Throw<MissingLinkException>(() => resource.GetSelf());
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetSelfOnResource_WithNullResource_ExpectException()
        {
            var ex = Should.Throw<MissingLinkException>(() => ((IHalResource)null).GetSelf());
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetSelfOnResource_WithValidResourceWithNullLinks_ExpectException()
        {
            var resource = new TestResource();

            var ex = Should.Throw<MissingLinkException>(() => resource.GetSelf());
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetSelfOnResource_WithValidResourceWithEmptyLinks_ExpectException()
        {
            var resource = new TestResource
            {
                Links = new HyperMediaLinks()
            };

            var ex = Should.Throw<MissingLinkException>(() => resource.GetSelf());
            _logger.WriteLine(ex.Message);
        }

        

        #endregion

        #region GetSelf on link

        [Fact]
        public void GetSelfOnLinks_WithValidLinks_ExpectCorrectLink()
        {
            var links = CreateLinks();

            var actual = links.GetSelf();

            actual.ShouldBe(DefaultUri);
        }

        [Fact]
        public void GetSelfOnLinks_WithValidResourceWithLinksWhenUriInvalid_ExpectException()
        {
            var resource = CreateLinks(selfHref: "invalid;uri");

            var ex = Should.Throw<MissingLinkException>(() => resource.GetSelf());
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetSelfOnLinks_WithNullLinks_ExpectException()
        {
            var ex = Should.Throw<MissingLinkException>(() => ((HyperMediaLinks)null).GetSelf());
            _logger.WriteLine(ex.Message);
        }

        [Fact]
        public void GetSelfOnLinks_WithEmptyLinks_ExpectException()
        {
            var ex = Should.Throw<MissingLinkException>(() => new HyperMediaLinks().GetSelf());
            _logger.WriteLine(ex.Message);
        }

        #endregion

        #endregion

        #region Construction

        private IHalResource CreateResource(string href = DefaultHref, bool isTemplated = false, string rel = DefaultRel, bool withSelf = true, string selfHref = DefaultHref)
        {
            return new TestResource
            {
                Links = CreateLinks(href, isTemplated, rel, withSelf, selfHref)
            };
        }

        private HyperMediaLinks CreateLinks(string href = DefaultHref, bool isTemplated = false, string rel = DefaultRel, bool withSelf = true, string selfHref = DefaultHref)
        {
            var links = new HyperMediaLinks
            {
                CreateLink(href, isTemplated, rel)
            };

            if (withSelf)
                links.Add(CreateSelfLink(selfHref));

            return links;
        }

        private HyperMediaLink CreateLink(string href = DefaultHref, bool isTemplated = false, string rel = DefaultRel)
        {
            return new HyperMediaLink
            {
                Rel = rel,
                Href = href,
                Templated = isTemplated
            };
        }

        private HyperMediaLink CreateSelfLink(string href = DefaultHref)
        {
            return CreateLink(href, false, "self");
        }

        #endregion
    }
}