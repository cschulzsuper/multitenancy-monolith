﻿using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IDistinctionTypeCustomPropertyRequestHandler
{
    Task<DistinctionTypeCustomPropertyResponse> GetAsync(string distinctionType, string uniqueName);

    IAsyncEnumerable<DistinctionTypeCustomPropertyResponse> GetAll(string distinctionType);

    Task<DistinctionTypeCustomPropertyResponse> InsertAsync(string distinctionType, DistinctionTypeCustomPropertyRequest request);

    Task UpdateAsync(string distinctionType, string uniqueName, DistinctionTypeCustomPropertyRequest request);

    Task DeleteAsync(string distinctionType, string uniqueName);

}