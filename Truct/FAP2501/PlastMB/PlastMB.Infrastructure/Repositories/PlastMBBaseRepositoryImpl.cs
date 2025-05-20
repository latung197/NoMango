using PlastMB.Infrastructure.Context;

namespace PlastMB.Infrastructure.Repositories
{
    public abstract class PlastMBBaseRepositoryImpl<T> : BaseRepositoryImpl<T> where T : class
    {
        protected PlastMBContext PlastMBContext { get; set; }

        public PlastMBBaseRepositoryImpl(PlastMBContext repositoryContext) : base(repositoryContext)
        {
            PlastMBContext = (PlastMBContext)base.Context;
        }
    }
}
