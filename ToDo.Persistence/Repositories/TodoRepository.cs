using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;
using ToDo.Persistence.DataContexts;
using ToDo.Persistence.Repositories.Interfaces;

namespace ToDo.Persistence.Repositories;

public class TodoRepository : EntityRepositoryBase<TodoItem, AppDbContext>, ITodoRepository
{
    public TodoRepository(AppDbContext dbContext) : base(dbContext) { }

    public new ValueTask<TodoItem> CreateAsync(TodoItem todoItem, bool saveChanges, CancellationToken cancellationToken)
    {   
        return base.CreateAsync(todoItem, saveChanges, cancellationToken);
    }

    public async ValueTask<bool> DeleteByIdAsync(Guid todoId, CancellationToken cancellationToken)
    {
        var result = await DbContext.Todos
                .Where(x => x.Id == todoId)
                .ExecuteDeleteAsync(cancellationToken);

        return result > 0;
    }

    public new IQueryable<TodoItem> Get(Expression<Func<TodoItem, bool>>? predicate, bool asNoTracking)
    {
        return base.Get(predicate, asNoTracking);
    }

    public new ValueTask<TodoItem?> GetByIdAsync(Guid todoId, bool asNoTracking, CancellationToken cancellationToken)
    {
        return base.GetByIdAsync(todoId, asNoTracking, cancellationToken);
    }

    public async ValueTask<bool> UpdateAsync(TodoItem todoItem, CancellationToken cancellationToken)
    {
        var result = await DbContext.Todos
            .Where(x => x.Id == todoItem.Id)
            .ExecuteUpdateAsync(propertySetter => propertySetter
                    .SetProperty(todo => todo.Title, todoItem.Title)
                    .SetProperty(todo => todo.IsDone, todoItem.IsDone)
                    .SetProperty(todo => todo.IsFavorite, todoItem.IsFavorite)
                    .SetProperty(todo => todo.DueTime, todoItem.DueTime)
                    .SetProperty(todo => todo.ReminderTime, todoItem.ReminderTime)
                    .SetProperty(todo => todo.ModifiedTime, DateTimeOffset.UtcNow),
                cancellationToken
                );
        return result > 0;
    }
}
