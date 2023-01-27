using ChristianSchulz.MultitenancyMonolith.Application.Business.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Business.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

internal sealed class BusinessObjectRequestHandler : IBusinessObjectRequestHandler
{
    public ValueTask<BusinessObjectResponse> GetAsync(string uniqueName)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<BusinessObjectResponse> GetAll()
    {
        throw new NotImplementedException();
    }

    public ValueTask<BusinessObjectResponse> InsertAsync(BusinessObjectRequest request)
    {
        throw new NotImplementedException();
    }

    public ValueTask UpdateAsync(string uniqueName, BusinessObjectRequest request)
    {
        throw new NotImplementedException();
    }

    public ValueTask DeleteAsync(string uniqueName)
    {
        throw new NotImplementedException();
    }
}