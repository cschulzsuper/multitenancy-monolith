using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Frontend.DevLog.Services.Documentation.Models;
using ChristianSchulz.MultitenancyMonolith.Objects.Documentation;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.DevLog.Services.Documentation;

public sealed class DevelopmentPostService
{
    private readonly IRepository<DevelopmentPost> _repository;

    public DevelopmentPostService(IRepository<DevelopmentPost> repository)
    {
        _repository = repository;
    }

    public DevelopmentPostModel[] GetAll()
    {
        var models = _repository
            .GetQueryable()
            .OrderByDescending(x => x.Index)
            .Select(x => new DevelopmentPostModel
            {
                Project = x.Project,
                Time = x.Time,
                Link = x.Link,
                Tags = x.Tags,
                Text = x.Text,
                Title = x.Title
            })
            .ToArray();

        return models;
    }
}
