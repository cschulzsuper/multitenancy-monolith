﻿using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

namespace ChristianSchulz.MultitenancyMonolith.Configuration;

public interface ISwaggerDocsProvider
{
    ICollection<SwaggerDoc> Get();
}