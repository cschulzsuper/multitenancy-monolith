using ChristianSchulz.MultitenancyMonolith.Frontend.DevDiary.Models;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.DevDiary.Services;

public sealed class PostService
{
    private readonly PostModel[] _posts = new PostModel[]
    {
        new() {
            Title="Initial commit",
            DateTime=DateTime.Parse("2022-11-06T12:55:58Z"),
            Text="In the initial commit I only created a single Visual Studio project from the built-in template and the GitHub repository.",
            Link="https://github.com/cschulzsuper/multitenancy-monolith/commit/0a18987ba7518789da227fce88915b55f3635d78",
            Tag="Initial template"
        },
        new() {
            Title="Endpoint and transport layer",
            DateTime=DateTime.Parse("2022-11-07T15:51:19Z"),
            Text=" I took the application logic from the template and moved it into an application layer. This layer is split into endpoints and transport. Endpoints only maps routes, while transport contains the logic to handle requests.",
            Link="https://github.com/cschulzsuper/multitenancy-monolith/commit/91a308bdb5b3593ac53061d309caf45837e36c45",
            Tag="Initial template"
        }
    };

    public PostModel[] GetAll()
        => _posts;
}
