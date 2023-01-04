using Bunit;
using Hydra.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperUI.Blazor.UnitTests.Components;

/// <summary>
/// <see cref="KanbanBoard"/> tests.
/// </summary>
public class KanbanBoardTests : TestContext
{
    public void Render_Multiple_RendersExpectedNumberOfSwimlanes()
    {
        // Arrange

        Workflow workflow = new()
        {
            Process = new[] { 
                "Backlog",
                "In progress",
                "Pending approval",
                "Approved",
                "Rejected"
            },

        };


    }
    
    class Work
    {
        public string? Id { get; set; }

        IEnumerable<Template>? Links { get; set; }
    }

    class Workflow
    {
        public IEnumerable<string>? Process { get; set; }
    }

    class WorkflowItem
    {
        public string? Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }


    }

    class KanbanBoard
    {

    }

    class KanbanSwimlane
    {
        public string Title { get; set; }
    }

    class KanbanCard
    {
        public string? Title { get; set; }


    }
}
