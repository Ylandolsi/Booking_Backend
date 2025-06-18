namespace IntegrationsTests.Abstractions;
// Share the same instance of the IntegrationTestsWebAppFactory
// across all tests in this assembly ( containers , config .. ) 

[CollectionDefinition(nameof(IntegrationTestsCollection))] // essential for xunit to recognize this as a collection definition
public sealed class IntegrationTestsCollection :
    ICollectionFixture<IntegrationTestsWebAppFactory>
{
}
