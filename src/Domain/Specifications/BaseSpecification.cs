using Application.Interfaces;
using Domain.Entities;
using System.Linq.Expressions;

namespace Infrastructure
{
    //Use Query Sepcifiction - addInlcude etc
    //so linq query syntact is not littered everywhere
    //Also facilates Opne-closed in Data Access like Repositry pattern as it doesnt need to change - just add gthe specification
    public abstract class BaseSpecification<T> : ISpecification<T> where T : BaseEntity
    {
        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>> Criteria { get; }

        public List<Expression<Func<T, object>>> Includes { get; } = new();

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
    }
}
