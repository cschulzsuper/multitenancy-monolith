﻿using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Business.BusinessObjectResource;

public sealed class Put 
{
    [Fact]
    public async Task Put_ShouldSucceed_WithoutCustomProperties()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingBusinessObject = new BusinessObject
        {
            UniqueName = $"existing-business-object-{Guid.NewGuid()}"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/business/business-objects/{existingBusinessObject.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putBusinessObject = new
        {
            UniqueName = $"put-business-object-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
        {
            var changedBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingBusinessObject.Snowflake &&
                    x.UniqueName == putBusinessObject.UniqueName);

            Assert.NotNull(changedBusinessObject);
        }
    }

    [Fact]
    public async Task Put_ShouldSucceed_WhenCustomPropertyUnknown()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingBusinessObject = new BusinessObject
        {
            UniqueName = $"existing-business-object-{Guid.NewGuid()}"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/business/business-objects/{existingBusinessObject.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putBusinessObject = new
        {
            UniqueName = $"put-business-object-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>
            {
                ["customProperty"] = "custom-property-value"
            }
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
        {
            var changedBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingBusinessObject.Snowflake &&
                    x.UniqueName == putBusinessObject.UniqueName &&
                    !x.CustomProperties.Any());

            Assert.NotNull(changedBusinessObject);
        }
    }

    [Fact]
    public async Task Put_ShouldSucceed_WhenCustomPropertyIsString()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingObjectTypeCustomProperty = new ObjectTypeCustomProperty
        {
            UniqueName = $"custom-property-{Guid.NewGuid()}",
            DisplayName = "Custom Property",
            PropertyName = "customProperty",
            PropertyType = "string"
        };

        var existingObjectType = new ObjectType
        {
            UniqueName = "business-object",
            CustomProperties = new List<ObjectTypeCustomProperty>
        {
            existingObjectTypeCustomProperty
        }
        };

        var existingBusinessObject = new BusinessObject
        {
            UniqueName = $"existing-business-object-{Guid.NewGuid()}"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<ObjectType>>()
                .Insert(existingObjectType);

            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/business/business-objects/{existingBusinessObject.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putBusinessObject = new
        {
            UniqueName = $"put-business-object-{Guid.NewGuid()}",
            CustomProperties = new Dictionary<string, object>
            {
                [existingObjectTypeCustomProperty.PropertyName] = "custom-property-value"
            }
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        var x = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
        {
            var changedBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingBusinessObject.Snowflake &&
                    x.UniqueName == putBusinessObject.UniqueName &&
                    x.CustomProperties.Count == 1 &&
                    x.CustomProperties.Any(y =>
                        y.Key == existingObjectTypeCustomProperty.PropertyName &&
                        y.Value.Equals("custom-property-value")));

            Assert.NotNull(changedBusinessObject);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var invalidBusinessObject = "Invalid";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/business/business-objects/{invalidBusinessObject}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putBusinessObject = new
        {
            UniqueName = "put-business-object",
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenAbsent()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var absentBusinessObject = "absent-business-object";

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/business/business-objects/{absentBusinessObject}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putBusinessObject = new
        {
            UniqueName = "put-business-object",
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameExists()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingBusinessObject = new BusinessObject
        {
            UniqueName = $"existing-business-object-{Guid.NewGuid()}"
        };

        var additionalBusinessObject = new BusinessObject
        {
            UniqueName = $"additional-business-object-{Guid.NewGuid()}"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject, additionalBusinessObject);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/business/business-objects/{existingBusinessObject.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putBusinessObject = new
        {
            additionalBusinessObject.UniqueName,
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        using (var scope = application.CreateMultitenancyScope())
        {
            var unchangedBusinessObject = scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .GetQueryable()
                .SingleOrDefault(x =>
                    x.Snowflake == existingBusinessObject.Snowflake &&
                    x.UniqueName == existingBusinessObject.UniqueName);

            Assert.NotNull(unchangedBusinessObject);
        }
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameNull()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingBusinessObject = new BusinessObject
        {
            UniqueName = $"existing-business-object-{Guid.NewGuid()}"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/business/business-objects/{existingBusinessObject.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putBusinessObject = new
        {
            UniqueName = (string?)null,
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameEmpty()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingBusinessObject = new BusinessObject
        {
            UniqueName = $"existing-business-object-{Guid.NewGuid()}"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/business/business-objects/{existingBusinessObject.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putBusinessObject = new
        {
            UniqueName = string.Empty,
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameTooLong()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingBusinessObject = new BusinessObject
        {
            UniqueName = $"existing-business-object-{Guid.NewGuid()}"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/business/business-objects/{existingBusinessObject.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putBusinessObject = new
        {
            UniqueName = new string(Enumerable.Repeat('a', 141).ToArray()),
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Put_ShouldFail_WhenUniqueNameInvalid()
    {
        // Arrange
        using var application = MockWebApplication.Create();

        var existingBusinessObject = new BusinessObject
        {
            UniqueName = $"existing-business-object-{Guid.NewGuid()}"
        };

        using (var scope = application.CreateMultitenancyScope())
        {
            scope.ServiceProvider
                .GetRequiredService<IRepository<BusinessObject>>()
                .Insert(existingBusinessObject);
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/a1/business/business-objects/{existingBusinessObject.UniqueName}");
        request.Headers.Authorization = application.MockValidMemberAuthorizationHeader();

        var putBusinessObject = new
        {
            UniqueName = "Invalid",
            CustomProperties = new Dictionary<string, object>()
        };

        request.Content = JsonContent.Create(putBusinessObject);

        var client = application.CreateClient();

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}