using ExchangeManagement.Contract;
using ExchangeManagement.Contract.Messages;

namespace NotifyRecipientsOfFinishedProductService.UnitTests
{
    public class ServiceTests
    {
        public Subscription GetSubscription(string url)
        {
            return new Subscription()
            {
                Url = url,
                Query = "test",
                AuthorizationOptions = new AuthorizationOptions()
                {
                    TokenEndpoint = "test",
                    ClientId = "test",
                    ClientSecret = "test"
                },
                IsDownloadResourceFile = true
            };
        }
        

        public MessageMetadata GetAggregateInformationResource(long infTestId, long demoTestId)
        {
            return new MessageMetadata()
            {
                Id = 5,
                Content = "P"
            };
        }

        //[Fact]
        //public void OneFindDemopicture()
        //{
        //    var subscription = GetSubscription("test");
        //    var subscriptionList = new List<Subscription>()
        //    {
        //        subscription,
        //        subscription
        //    };

        //    var infTestId = 100;
        //    var demoTestId = 101;

        //    var aggregateInformationResource = GetAggregateInformationResource(infTestId, demoTestId);

        //    var query = subscription.Query + "&informationResourceId=" +
        //                aggregateInformationResource.Id;

        //    var subscriptionServiceMock = new Mock<ISubscriptionService>();
        //    subscriptionServiceMock.Setup(service => service.List()).Returns(subscriptionList);
            
            

        //    var importServiceMock = new Mock<IImportService>();
        //    importServiceMock.Setup(service => service.Import(subscription, aggregateInformationResource)).Returns(infTestId);

        //    var handler = new Service(null, null, subscriptionServiceMock.Object, timeDemoPictureServiceMock.Object, importServiceMock.Object);
        //    handler.Process(aggregateInformationResource);

        //    subscriptionServiceMock.Verify(service => service.List(), Times.Once);
        //    timeDemoPictureServiceMock.Verify(service => service.Search(query), Times.Once);
        //    importServiceMock.Verify(service => service.Import(subscription, aggregateInformationResource), Times.Once);
        //}

        //[Fact]
        //public void OneImport()
        //{
        //    var subscription = GetSubscription("test");
        //    var subscription2 = GetSubscription("test");
        //    var subscriptionList = new List<Subscription>()
        //    {
        //        subscription,
        //        subscription2
        //    };

        //    var infTestId = 100;
        //    var demoTestId = 101;

        //    var aggregateInformationResource = GetAggregateInformationResource(infTestId, demoTestId);

        //    var query = subscription.Query + "&informationResourceId=" +
        //                aggregateInformationResource.Resource.Id;

        //    var subscriptionServiceMock = new Mock<ISubscriptionService>();
        //    subscriptionServiceMock.Setup(service => service.List()).Returns(subscriptionList);

        //    var timeDemoPictureServiceMock = new Mock<ICheckSubscriptionService>();
        //    timeDemoPictureServiceMock.Setup(service => service.Search(query)).Returns(() => new PagedResult<DemoPicture>() { Results = aggregateInformationResource.DemoPictures.Cast<DemoPicture>().ToList(), TotalCount = aggregateInformationResource.DemoPictures.Count });

        //    var importServiceMock = new Mock<IImportService>();
        //    importServiceMock.Setup(service => service.Import(subscription, aggregateInformationResource)).Returns(infTestId);
        //    importServiceMock.Setup(service => service.Import(subscription2, aggregateInformationResource)).Returns(infTestId);

        //    var handler = new Service(null, null, subscriptionServiceMock.Object, timeDemoPictureServiceMock.Object, importServiceMock.Object);
        //    handler.Process(aggregateInformationResource);

        //    subscriptionServiceMock.Verify(service => service.List(), Times.Once);
        //    timeDemoPictureServiceMock.Verify(service => service.Search(query), Times.Once);
        //    importServiceMock.Verify(service => service.Import(subscription, aggregateInformationResource), Times.Once);
        //    importServiceMock.Verify(service => service.Import(subscription2, aggregateInformationResource), Times.Never);
        //}

        //[Fact]
        //public void OneImportProblemShouldPublishWithRetry()
        //{
        //    var subscription = GetSubscription("test");
        //    var subscription2 = GetSubscription("test");
        //    var subscriptionList = new List<Subscription>()
        //    {
        //        subscription,
        //        subscription2
        //    };

        //    var infTestId = 100;
        //    var demoTestId = 101;

        //    var aggregateInformationResource = GetAggregateInformationResource(infTestId, demoTestId);

        //    var query = subscription.Query + "&informationResourceId=" +
        //                aggregateInformationResource.Resource.Id;

        //    var advancedBusMock = new Mock<IAdvancedBus>();

        //    var subscriptionServiceMock = new Mock<ISubscriptionService>();
        //    subscriptionServiceMock.Setup(service => service.List()).Returns(subscriptionList);

        //    var timeDemoPictureServiceMock = new Mock<ICheckSubscriptionService>();
        //    timeDemoPictureServiceMock.Setup(service => service.Search(query)).Returns(() => new PagedResult<DemoPicture>() { Results = aggregateInformationResource.DemoPictures.Cast<DemoPicture>().ToList(), TotalCount = aggregateInformationResource.DemoPictures.Count });

        //    var importServiceMock = new Mock<IImportService>();
        //    importServiceMock.Setup(service => service.Import(subscription, aggregateInformationResource)).Throws(new Exception("problem occured"));
        //    importServiceMock.Setup(service => service.Import(subscription2, aggregateInformationResource)).Returns(infTestId);
            
        //    var handler = new Service(advancedBusMock.Object, null, subscriptionServiceMock.Object, timeDemoPictureServiceMock.Object, importServiceMock.Object);
        //    handler.Process( aggregateInformationResource );

        //    advancedBusMock.Verify(bus => bus.Publish(It.IsAny<IExchange>(), String.Empty, true, It.Is<IMessage<RedeliveribleInformationResource>>(message => object.Equals(message.Properties.Headers.Single().Value, (int)TimeSpan.FromMinutes(1).TotalMilliseconds))), Times.Once);

        //    subscriptionServiceMock.Verify(service => service.List(), Times.Once);
        //    timeDemoPictureServiceMock.Verify(service => service.Search(query), Times.Once);
        //    importServiceMock.Verify(service => service.Import(subscription, aggregateInformationResource), Times.Once);
        //    importServiceMock.Verify(service => service.Import(subscription2, aggregateInformationResource), Times.Never);
        //}

        //[Fact]
        //public void TwoImport()
        //{
        //    var subscription = GetSubscription("test");
        //    var subscription2 = GetSubscription("test2");
        //    var subscriptionList = new List<Subscription>()
        //    {
        //        subscription,
        //        subscription2
        //    };

        //    var infTestId = 100;
        //    var demoTestId = 101;

        //    var aggregateInformationResource = GetAggregateInformationResource(infTestId, demoTestId);

        //    var query = subscription.Query + "&informationResourceId=" +
        //                aggregateInformationResource.Resource.Id;

        //    var subscriptionServiceMock = new Mock<ISubscriptionService>();
        //    subscriptionServiceMock.Setup(service => service.List()).Returns(subscriptionList);

        //    var timeDemoPictureServiceMock = new Mock<ICheckSubscriptionService>();
        //    timeDemoPictureServiceMock.Setup(service => service.Search(query)).Returns(() => new PagedResult<DemoPicture>() { Results = aggregateInformationResource.DemoPictures.Cast<DemoPicture>().ToList(), TotalCount = aggregateInformationResource.DemoPictures.Count });

        //    var importServiceMock = new Mock<IImportService>();
        //    importServiceMock.Setup(service => service.Import(subscription, aggregateInformationResource)).Returns(infTestId);
        //    importServiceMock.Setup(service => service.Import(subscription2, aggregateInformationResource)).Returns(infTestId);

        //    var handler = new Service(null, null, subscriptionServiceMock.Object, timeDemoPictureServiceMock.Object, importServiceMock.Object);
        //    handler.Process(aggregateInformationResource);

        //    subscriptionServiceMock.Verify(service => service.List(), Times.Once);
        //    timeDemoPictureServiceMock.Verify(service => service.Search(query), Times.Exactly(2));

        //    importServiceMock.Verify(service => service.Import(subscription, aggregateInformationResource), Times.Once);
        //    importServiceMock.Verify(service => service.Import(subscription2, aggregateInformationResource), Times.Once);
        //}

        //[Fact]
        //public void ImportWithoutResourceFile()
        //{
        //    var subscription = GetSubscription("test");
        //    var subscription2 = GetSubscription("test");
        //    subscription2.Query = "test2";
        //    subscription2.IsDownloadResourceFile = false;
        //    var subscriptionList = new List<Subscription>()
        //    {
        //        subscription,
        //        subscription2
        //    };

        //    var infTestId = 100;
        //    var demoTestId = 101;

        //    var aggregateInformationResource = GetAggregateInformationResource(infTestId, demoTestId);

        //    var query = subscription.Query + "&informationResourceId=" +
        //                aggregateInformationResource.Resource.Id;

        //    var query2 = subscription2.Query + "&informationResourceId=" +
        //                 aggregateInformationResource.Resource.Id;

        //    var subscriptionServiceMock = new Mock<ISubscriptionService>();
        //    subscriptionServiceMock.Setup(service => service.List()).Returns(subscriptionList);

        //    var timeDemoPictureServiceMock = new Mock<ICheckSubscriptionService>();
        //    timeDemoPictureServiceMock.Setup(service => service.Search(query)).Returns(() => new PagedResult<DemoPicture>());
        //    timeDemoPictureServiceMock.Setup(service => service.Search(query2)).Returns(() => new PagedResult<DemoPicture>() { Results = aggregateInformationResource.DemoPictures.Cast<DemoPicture>().ToList(), TotalCount = aggregateInformationResource.DemoPictures.Count });

        //    var importServiceMock = new Mock<IImportService>();
        //    importServiceMock.Setup(service => service.Import(subscription, It.IsAny<AggregateInformationResourceDetails>())).Returns(infTestId);

        //    var handler = new Service(null, null, subscriptionServiceMock.Object, timeDemoPictureServiceMock.Object, importServiceMock.Object);
        //    handler.Process(aggregateInformationResource);

        //    subscriptionServiceMock.Verify(service => service.List(), Times.Once);
        //    timeDemoPictureServiceMock.Verify(service => service.Search(query), Times.Once);
        //    timeDemoPictureServiceMock.Verify(service => service.Search(query2), Times.Once);
        //    importServiceMock.Verify(service => service.Import(subscription2, It.Is<AggregateInformationResourceDetails>(
        //        product => product.Resource == aggregateInformationResource.Resource
        //                   && ReferenceEquals(product.DemoPictures, aggregateInformationResource.DemoPictures)
        //                   && ReferenceEquals(product.Previews, aggregateInformationResource.Previews)
        //                   && product.Version == aggregateInformationResource.Version
        //                   && product.ResourceFile == null)), Times.Once);
        //}
    }
}
