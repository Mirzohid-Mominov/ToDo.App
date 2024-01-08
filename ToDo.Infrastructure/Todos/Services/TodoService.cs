using FluentValidation;
using Npgsql.Internal;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ToDo.Application.Todos.Services;
using ToDo.Domain.Entities;
using ToDo.Persistence.Repositories.Interfaces;

namespace ToDo.Infrastructure.Todos.Services;

public class TodoService(ITodoRepository todoRepository, IValidator<TodoItem> todoValidator) : ITodoService
{
    public ValueTask<TodoItem> CreateAsync(TodoItem todoItem, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var validatonResult = todoValidator.Validate(todoItem);
        if(!validatonResult.IsValid)
        {
            throw new ValidationException(validatonResult.Errors);
        }
        return todoRepository.CreateAsync(todoItem, saveChanges, cancellationToken);
    }

    public ValueTask<bool> DeleteByIdAsync(Guid todoId, CancellationToken cancellationToken = default)
    {
        return todoRepository.DeleteByIdAsync(todoId, cancellationToken);
    }

    public IQueryable<TodoItem> Get(Expression<Func<TodoItem, bool>>? predicate = null, bool asNoTracking = false)
    {
        return todoRepository.Get(predicate, asNoTracking);
    }

    public async ValueTask<IList<TodoItem>> GetAsync(bool asNoTracking = false)
    {
        var todos = await todoRepository.Get().ToListAsync();
        return todos
            .Where(todo => !todo.IsDone && todo.DueTime > DateTime.Now).OrderBy(todo => todo.DueTime)
            .Concat(todos.Where(todo => todo.IsDone).OrderByDescending(todo => todo.ModifiedTime))
            .Concat(todos.Where(todo => !todo.IsDone && todo.DueTime <= DateTime.Now)
                .OrderByDescending(todo => todo.DueTime))
            .ToList();
    }
    public ValueTask<TodoItem?> GetByIdAsync(Guid todoId, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        return todoRepository.GetByIdAsync(todoId, asNoTracking, cancellationToken);
    }

    public ValueTask<bool> UpdateAsync(TodoItem todoItem, CancellationToken cancellationToken = default)
    {
        var validationResult = todoValidator.Validate(todoItem);
        if(!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        return todoRepository.UpdateAsync(todoItem, cancellationToken);
    }
}
