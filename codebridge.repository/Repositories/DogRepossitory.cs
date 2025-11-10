using codebridge.Common.Data;
using codebridge.Common.Helpers;
using codebridge.Common.Models;
using codebridge.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codebridge.DAL.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly DogsDbContext _context;

        public DogRepository(DogsDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Dog>> GetDogsAsync(DogQueryParams queryParams)
        {
            IQueryable<Dog> query = _context.Dogs.AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.Attribute))
            {
                query = ApplySorting(query, queryParams.Attribute, queryParams.Order);
            }

            int totalCount = await query.CountAsync();

            int skip = (queryParams.PageNumber - 1) * queryParams.PageSize;
            List<Dog> dogs = await query
                .Skip(skip)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return new PagedResult<Dog>
            {
                Data = dogs,
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<Dog?> GetDogByNameAsync(string name)
        {
            return await _context.Dogs.FindAsync(name);
        }

        public async Task<Dog> CreateDogAsync(Dog dog)
        {
            _context.Dogs.Add(dog);
            await _context.SaveChangesAsync();
            return dog;
        }

        public async Task<bool> DogExistsAsync(string name)
        {
            return await _context.Dogs.AnyAsync(d => d.Name == name);
        }

        private IQueryable<Dog> ApplySorting(IQueryable<Dog> query, string attribute, string? order)
        {
            bool isDescending = order?.ToLower() == "desc";

            return attribute.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name),
                "color" => isDescending ? query.OrderByDescending(d => d.Color) : query.OrderBy(d => d.Color),
                "tail_length" or "taillength" => isDescending ? query.OrderByDescending(d => d.TailLength) : query.OrderBy(d => d.TailLength),
                "weight" => isDescending ? query.OrderByDescending(d => d.Weight) : query.OrderBy(d => d.Weight),
                _ => query.OrderBy(d => d.Name)
            };
        }
    }
}
