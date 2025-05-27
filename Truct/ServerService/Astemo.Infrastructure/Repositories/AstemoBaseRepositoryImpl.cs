using Core.Infrastructure.Context;

namespace Core.Infrastructure.Repositories
{
    public abstract class AstemoBaseRepositoryImpl<T> : BaseRepositoryImpl<T> where T : class
    {
        protected AstemoContext AstemoContext { get; set; }

        public AstemoBaseRepositoryImpl(AstemoContext repositoryContext) : base(repositoryContext)
        {
            AstemoContext = (AstemoContext)base.Context;
        }
    }
}
