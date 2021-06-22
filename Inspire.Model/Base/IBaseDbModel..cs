namespace Inspire.Model.Base
{
    using System;

    public interface IBaseDbModel : IModel
    {
        Guid? Id { get; }
    }
}