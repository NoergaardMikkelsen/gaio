using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Uno.Endpoints;

namespace Statistics.Uno.Presentation.Core;

public abstract class BaseDialogLogic<TViewModel, TEntity, TSearchable> where TViewModel : class
    where TEntity : class, IEntity
    where TSearchable : class, ISearchable
{
    protected readonly IEntityEndpoint<TEntity, TSearchable> endpoint;
    protected readonly TEntity entity;
    protected readonly TViewModel viewModel;

    protected BaseDialogLogic(TEntity entity, TViewModel viewModel, IEntityEndpoint<TEntity, TSearchable> endpoint)
    {
        this.viewModel = viewModel;
        this.endpoint = endpoint;
        this.entity = entity;
    }

    public async void PrimaryButtonClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (!HasChanges())
        {
            return;
        }

        CollectUpdatedFieldsFromViewModel();

        if (entity.Id == 0)
        {
            await endpoint.AddSingle(CancellationToken.None, entity);
        }
        else
        {
            await endpoint.UpdateSingle(CancellationToken.None, entity);
        }
    }

    protected abstract void CollectUpdatedFieldsFromViewModel();
    protected abstract bool HasChanges();
}
