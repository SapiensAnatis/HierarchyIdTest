// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using HierarchyIdTest;
using Microsoft.EntityFrameworkCore;

var context = new MyContext();
await context.Database.EnsureDeletedAsync();
await context.Database.EnsureCreatedAsync();

Service root = new Service() { Name = "Root", PathFromRoot = HierarchyId.Parse("/47337/") };
context.Add(root);

Service root2 = new Service() { Name = "Root 2", PathFromRoot = HierarchyId.Parse("/47337/") };
context.Add(root2);
context.SaveChanges();

Service[] children =
[
    new Service() { Name = "Child" },
    new Service() { Name = "Child 2" },
    new Service() { Name = "Child 3" },
];

foreach (Service child in children)
{
    await AddChildService(child, root.Id);
}

Service[] grandchildren =
[
    new Service() { Name = "Grandchild 1" },
    new Service() { Name = "Grandchild 2" },
];

foreach (Service grandchild in grandchildren)
{
    await AddChildService(grandchild, children[1].Id);
}

var grandchild2Hierarchy = context
    .Services.Where(ancestor =>
        context
            .Services.Single(descendent => descendent.Id == grandchildren[1].Id)
            .PathFromRoot.IsDescendantOf(ancestor.PathFromRoot)
    )
    .OrderByDescending(ancestor => ancestor.PathFromRoot.GetLevel())
    .ToList();

Console.WriteLine("Hierarchy of grandchildren[1]:");
Console.WriteLine(
    JsonSerializer.Serialize(
        grandchild2Hierarchy,
        new JsonSerializerOptions() { WriteIndented = true }
    )
);

async Task AddChildService(Service service, Guid parentId)
{
    Service parent = context.Services.First(x => x.Id == parentId);
    HierarchyId? newestChild = await context
        .Services.Where(x =>
            x.PathFromRoot.GetAncestor(1)
            == context.Services.Single(y => y.Id == parentId).PathFromRoot
        )
        .Select(x => x.PathFromRoot)
        .DefaultIfEmpty()
        .MaxAsync();

    service.PathFromRoot = parent.PathFromRoot.GetDescendant(newestChild);
    context.Add(service);
    await context.SaveChangesAsync();
}
