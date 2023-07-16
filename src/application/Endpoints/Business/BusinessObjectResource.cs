using ChristianSchulz.MultitenancyMonolith.Application.Business.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

public static class BusinessObjectResource
{
    private const string CouldNotQueryBusinessObjects = "Could not query business objects";
    private const string CouldNotQueryBusinessObject = "Could not query business object";
    private const string CouldNotCreateBusinessObject = "Could not create business object";
    private const string CouldNotUpdateBusinessObject = "Could not update business object";
    private const string CouldNotDeleteBusinessObject = "Could not delete business object";

    public static IEndpointRouteBuilder MapBusinessObjectResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/business-objects")
            .WithTags("Business Object API")
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "member")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("member", "member-observer"))
            .WithErrorMessage(CouldNotQueryBusinessObjects);

        resource
            .MapGet("{businessObject}", Get)
            .RequireAuthorization(policy => policy
                .RequireRole("member", "member-observer"))
            .WithErrorMessage(CouldNotQueryBusinessObject);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
            .WithErrorMessage(CouldNotCreateBusinessObject);

        resource
            .MapPut("{businessObject}", Put)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
            .WithErrorMessage(CouldNotUpdateBusinessObject);

        resource
            .MapDelete("{businessObject}", Delete)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
            .WithErrorMessage(CouldNotDeleteBusinessObject);

        return endpoints;
    }

    private static Delegate GetAll =>
        (IBusinessObjectRequestHandler requestHandler)
            => requestHandler.GetAll();

    private static Delegate Get =>
        (IBusinessObjectRequestHandler requestHandler, string businessObject)
            => requestHandler.GetAsync(businessObject);

    private static Delegate Post =>
        (IBusinessObjectRequestHandler requestHandler, BusinessObjectRequest request)
            => requestHandler.InsertAsync(request);

    private static Delegate Put =>
        (IBusinessObjectRequestHandler requestHandler, string businessObject, BusinessObjectRequest request)
            => requestHandler.UpdateAsync(businessObject, request);

    private static Delegate Delete =>
        (IBusinessObjectRequestHandler requestHandler, string businessObject)
            => requestHandler.DeleteAsync(businessObject);
}