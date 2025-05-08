using AdacaProduct.Model.Command;

namespace AdacaProduct.Service.Interface
{
    public interface ICommandHandler<TCommand, TResult>
    {
        Task<TResult> Handle(AddProductCommand command);
    }
}
