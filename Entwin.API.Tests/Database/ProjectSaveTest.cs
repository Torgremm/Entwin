using Xunit;
using Entwin.API.Data;
using Entwin.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

[Collection("DatabaseTests")]
public class ProjectSaveLogicTest
{
    [Fact]
    public async Task SaveProject_ShouldPersistInDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("ProjectSaveTestDb")
            .Options;

        using (var context = new AppDbContext(options))
        {
            var project = new Project
            {
                UserId = 123,
                ProjectData = true,
                SavedTime = DateTime.UtcNow
            };

            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }

        using (var context = new AppDbContext(options))
        {
            var savedProject = await context.Projects.FirstOrDefaultAsync(p => p.UserId == 123);
            Assert.NotNull(savedProject);
            Assert.True(savedProject.ProjectData);
        }
    }
}
