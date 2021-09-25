using System.Collections.Generic;
using System.Collections;
using System;

namespace Interaction.Types {
    public abstract class Book {
        public static readonly int MAX_PAGE_LENGTH = 100;

        // ---- // Constructors

        public static Book<T> Create<T>(IEnumerable<T> values, int pageLength) {
            return Book<T>.Create(values, pageLength);
        }
        public static bool TryCreate<T>(IEnumerable<T> values, int pageLength, out Book<T> res) {
            try {
                res = Book<T>.Create(values, pageLength);
                return true;
            } catch (ArgumentException e) { Console.WriteLine(e); }

            res = null;
            return false;
        }
    }

    /*
		Class represents a book containing objects of type T.

		Books are used to split longer collections of elements into pages and display individual pages of them.
	*/
    public class Book<T> : Book, IEnumerable<T> {
        private readonly List<T> _list;
        private int _pageLength;
        private int _pageCount;

        protected Book(List<T> list, int pageLength) {
            _list = list;
            _pageLength = pageLength;
            _pageCount = GetPageCount();
        }

        public static Book<T> Create(IEnumerable<T> values, int pageLength) {
            if (pageLength < 1)
                throw new ArgumentException("Pages must contain at least 1 element", "pageLength");
            if (pageLength > Book.MAX_PAGE_LENGTH)
                throw new ArgumentException($"Pages cannot contain more than {Book.MAX_PAGE_LENGTH} elements", "pageLength");
            if (values == null)
                throw new ArgumentException("Parameter cannot be null", "values");

            var list = new List<T>(values);

            if (list.Count < 1)
                throw new ArgumentException("Collection cannot be empty", "values");

            return new Book<T>(list, pageLength);

        }

        public IReadOnlyList<T> List => _list;
        public int Count => _list.Count;
        public int PageLength => _pageLength;
        public int PageCount => _pageCount;

        // ---- // Methods 

        public int GetPageCount()
            => GetPageCount(_pageLength);
        public int GetPageCount(int pageLength)
            => (int)Math.Ceiling((double)_list.Count / (double)pageLength);

        public int GetPageNumber(int pageNr)
            => Math.Clamp(pageNr, 0, _pageCount - 1);

        public IEnumerable<T> GetPageEnumerable(int pageNr) {
            pageNr = GetPageNumber(pageNr);

            int start = pageNr * _pageLength;
            int end = Math.Min((pageNr + 1) * _pageLength, Count);

            for (int i = start; i < end; ++i) {
                yield return _list[i];
            }
        }

        public void GetPageEnumerable(int pageNr, Action<T> action) {
            var pageEnum = GetPageEnumerable(pageNr);
            foreach (var elem in pageEnum) {
                action(elem);
            }
        }

        /// <summary>
        /// 	Gets a Page object representing the desired page of the book.
        /// </summary>
        /// <param name="pageNr">The desired page index</param>
        /// <param name="pageLenght">The length of pages</param>
        /// <param name="closestIfOutOfRange">Declares that we want to round the pageNr up or down if out of range</param>
        /// <exception cref="IndexOutOfRangeException">
        /// 	The pageNr indicates a page that isn't valid with the given pageNr and closestIfOutOfRange is set to false.
        /// </exception>
        /// <returns>
        /// 	An IPage<T> object representing the desired page of the book.
        /// </returns>
        public IPage<T> GetPage(int pageNr, int pageLength, bool closestIfOutOfRange = false) {
            var maxPage = GetPageCount(pageLength);
            if (closestIfOutOfRange) pageNr = Math.Clamp(pageNr, 0, maxPage - 1);
            else if (pageNr < 0 || pageNr >= maxPage) {
                throw new IndexOutOfRangeException($"parameter {nameof(pageNr)} ({pageNr}) out of range [0, {maxPage}). ({nameof(pageLength)} = {pageLength})");
            }

            int start = pageNr * pageLength;
            int length = Math.Min(pageLength, Count - start);

            var page = new Page<T>(this, _list.GetRange(start, length), pageNr, pageLength, maxPage);
            return page;
        }
        public IPage<T> GetPage(int pageNr, bool closestIfOutOfRange = false)
            => GetPage(pageNr, _pageLength, closestIfOutOfRange);

        // ---- // Standard Methods 

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        class Enumerator : IEnumerator<T> {
            public T Current { get; }

            object IEnumerator.Current => Current;
            public bool MoveNext() {
                return true;
            }
            public void Reset() { }

            public void Dispose() { }
        }
    }

    /*
		Represents a page in a book.
	*/
    public class Page<T> : IPage<T> {

        public int PageNr { get; }
        public int PageLength { get; }
        public int PageCount { get; }
        public Book<T> Book { get; }

        public int Count => _list.Count;
        public bool HasNextPage => PageNr < PageCount - 1;
        public bool HasPrevPage => PageNr > 0;

        private readonly List<T> _list;

        public Page(Book<T> book, List<T> list, int pageNr, int pageLength, int pageCount) {
            Book = book;
            _list = list;
            PageNr = pageNr;
            PageLength = pageLength;
            PageCount = pageCount;
        }

        /// <summary>
        ///     Get an element at the ith index of a page.
        /// </summary>
        /// <param name="i">The index of the element in the page we want to access.</param>
        /// <exception cref="ArgumentException">This module has already been added.</exception>
        /// <exception cref="IndexOutOfRangeException">
        /// 	The desired index is outside of the range of the page.
        /// </exception>
        /// <returns>
        ///     The T object that is in the desired index
        /// </returns>
        public T this[int i] {
            get {
                return _list[i];
            }
        }

        public IPage<T> GetNextPage() {
            return Book.GetPage(PageNr + 1, PageLength);
        }

        public IPage<T> GetPrevPage() {
            return Book.GetPage(PageNr - 1, PageLength);
        }

        public void ForEach(Action<T> action) {
            foreach (var elem in this) {
                action(elem);
            }
        }

        public IEnumerator<T> GetEnumerator() {
            return _list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }


    public interface IPage<T> : IReadOnlyList<T> {
        int PageNr { get; }
        int PageLength { get; }
        int PageCount { get; }

        bool HasNextPage { get; }
        bool HasPrevPage { get; }

        void ForEach(Action<T> action);

        IPage<T> GetNextPage();
        IPage<T> GetPrevPage();
    }
}