using Novel.Entity.Models;
using Novel.Entity.ViewModels;
using Novel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Novel.Service
{
    public class BookTaskService : BaseRepository<BookReptileTask>
    {
        public PaginatedList<BookReptileTask> GetTasks(TaskSearchViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new TaskSearchViewModel
                {
                    ps = 10,
                    pi = 1,
                };
            }
            if (viewModel.pi == 0)
            {
                viewModel.pi = 1;
            }
            if (viewModel.ps == 0)
            {
                viewModel.ps = 10;
            }
            IQueryable<BookReptileTask> d = Db.BookReptileTask.AsQueryable();
            if (!viewModel.k.IsEmpty())
            {
                d = d.Where(m => m.BookName.Contains(viewModel.k));
            }
            if (viewModel.synctype > 0)
            {
                d = d.Where(m => m.SyncType == viewModel.synctype);
            }
            var result = d.Query(viewModel.pi, viewModel.ps, m => m.Updated, m => m.ToList(), false);
            return result;
        }

        public BookReptileTask GetTask(int id)
        {
            return Db.BookReptileTask.FirstOrDefault(m => m.Id == id);
        }
        public BookReptileTask AddBookReptileTask(BookReptileTask bookReptileTask)
        {
            if (bookReptileTask == null)
            {
                return null;
            }
            bookReptileTask.Created = DateTime.Now;
            bookReptileTask.Updated = DateTime.Now;
            Db.BookReptileTask.Add(bookReptileTask);
            return Db.SaveChanges() > 0 ? bookReptileTask : null;
        }

        public bool CheckTaskUrl(int id, string url)
        {
            if (id > 0)
            {
                return Db.BookReptileTask.Any(m => m.Url == url && m.Id != id);
            }
            return Db.BookReptileTask.Any(m => m.Url == url);
        }
    }
}
