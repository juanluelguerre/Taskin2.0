using MediatR;
using DomainTaskStatus = ElGuerre.Taskin.Domain.Entities.TaskStatus;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class UpdateTaskStatusCommand : IRequest
{
    public Guid Id { get; set; }
    public DomainTaskStatus Status { get; set; }
}