using ElGuerre.Taskin.Domain.Entities;
using MediatR;
using DomainTaskStatus = ElGuerre.Taskin.Domain.Entities.TaskStatus;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class BulkUpdateTaskStatusCommand : IRequest
{
    public List<Guid> TaskIds { get; set; } = [];
    public DomainTaskStatus Status { get; set; }
}