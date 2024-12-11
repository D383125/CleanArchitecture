﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Application.Interfaces
{
    //There is some reducnacy of the repostiry pattrn with EFcore dbContgext as this acts as a repositry. Using it to facilate Open-Closed semantics 
    //via the Query Sepcification pattern.
//    The Specification pattern centralizes query logic into reusable classes, promoting DRY principles.
        //Encapsulates query logic.
        //Simplifies unit testing.
        //Supports dynamic composition of queries.
    public interface IGenericRepository<T>
    {
        IEnumerable<T> Specify(ISpecification<T> spec);                
        IEnumerable<T> SpecifyWithPagination(ISpecification<T> spec, int pageSize = 10, int pageIndex = 0);        
        Task<T> AddAsync(T entity);        
        Task<T[]> AddRangeAsync(T[] entity);        
        Task DeleteAsync(T entity);        
        Task<T> Get(Expression<Func<T, bool>> predicate);        
        Task<IReadOnlyList<T>> GetAll();        
        Task<T> GetByIdAsync(int Id);        
        Task UpdateAsync(T entity);        
    }
}