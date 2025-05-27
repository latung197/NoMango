using Core.Infrastructure.Context;

namespace Core.Infrastructure.Repositories
{
    public abstract class CoreBaseRepositoryImpl<T> : BaseRepositoryImpl<T> where T : class
    {
        protected CoreContext CoreContext { get; set; }

        public CoreBaseRepositoryImpl(CoreContext repositoryContext) : base(repositoryContext)
        {
            CoreContext = (CoreContext)base.Context;
        }
    }
}
