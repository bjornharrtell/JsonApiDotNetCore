using System;
using JsonApiDotNetCore.Builders;
using JsonApiDotNetCore.Internal.Contracts;
using JsonApiDotNetCore.Models;
using JsonApiDotNetCore.Query;
using Moq;

namespace Benchmarks
{
    internal static class DependencyFactory
    {
        public static IResourceGraph CreateResourceGraph()
        {
            IResourceGraphBuilder builder = new ResourceGraphBuilder();
            builder.AddResource<BenchmarkResource>(BenchmarkResourcePublicNames.Type);
            return builder.Build();
        }

        public static IResourceDefinitionProvider CreateResourceDefinitionProvider(IResourceGraph resourceGraph)
        {
            var resourceDefinition = new ResourceDefinition<BenchmarkResource>(resourceGraph);

            var resourceDefinitionProviderMock = new Mock<IResourceDefinitionProvider>();
            resourceDefinitionProviderMock.Setup(provider => provider.Get(It.IsAny<Type>())).Returns(resourceDefinition);
            
            return resourceDefinitionProviderMock.Object;
        }
    }
}
