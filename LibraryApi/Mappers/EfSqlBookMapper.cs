using LibraryApi.Domain;
using LibraryApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace LibraryApi.Mappers
{
    public class EfSqlBookMapper : IMapBooks
    {
        LibraryDataContext Context;
        IMapper Mapper;
        public EfSqlBookMapper(LibraryDataContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }

        private IQueryable<Book> GetBooksInInventory()
        {
            return Context.Books.Where(b => b.InInventory);
        }
        public async Task<GetABookResponse> GetBookById(int id)
        {
            var response = await GetBooksInInventory()
                .Where(b => b.Id == id)
                .AsNoTracking()
                .Select(b => Mapper.Map<GetABookResponse>(b)).SingleOrDefaultAsync();

            return response;
        }

        public async Task<GetBooksResponse> GetAllBooks(string genre)
        {

            var response = new GetBooksResponse();
            var data = GetBooksInInventory();

            if (genre != "all")
            {
                data = data.Where(b => b.Genre == genre);
            }
            response.Data = await data.Select(b => Mapper.Map<BookSummaryItem>(b))
              .ToListAsync();
            response.Genre = genre;
            return response;
        }

        public async Task<GetABookResponse> AddABook(PostBooksRequest bookToAdd)
        {
            var book = new Book
            {
                Title = bookToAdd.Title,
                Author = bookToAdd.Author,
                Genre = bookToAdd.Genre,
                InInventory = true,
                NumberOfPages = bookToAdd.NumberOfPages
            };
            Context.Books.Add(book);
            await Context.SaveChangesAsync();
            var response = Mapper.Map<GetABookResponse>(book);
            return response;
        }

        public async Task<bool> UpdateNumberOfPages(int id, int newPages)
        {
            var book = await GetBooksInInventory()
                     .Where(b => b.Id == id)
                     .SingleOrDefaultAsync();
            if (book == null)
            {
                return false;
            }
            else
            {
                book.NumberOfPages = newPages;
                await Context.SaveChangesAsync();
                return true;
            }
        }
        public async Task Remove(int id)
        {
            var book = await GetBooksInInventory()
                .Where(b => b.Id == id)
                .SingleOrDefaultAsync();

            if (book != null)
            {
                book.InInventory = false;
                await Context.SaveChangesAsync();
            }
        }
    }
}
