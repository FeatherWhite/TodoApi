using TodoApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Data
{
    public class DbInitializer
    {
        public static void Initialize(TodoContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.TodoItems.Any())
            {
                return;   // DB has been seeded
            }

            var todoItems = new TodoItem[]
            {
                new TodoItem{Id=1,Name="Alexander",IsComplete=false,Secret=null},
                new TodoItem{Id=2,Name="Alonso",IsComplete=false,Secret=null},
                new TodoItem{Id=3,Name="Anand",IsComplete=false,Secret=null},
                new TodoItem{Id=4,Name="Barzdukas",IsComplete=false,Secret=null},
                new TodoItem{Id=5,Name="Li",IsComplete=false,Secret=null},
                new TodoItem{Id=6,Name="Justice",IsComplete=false,Secret=null},
                new TodoItem{Id=7,Name="Norman",IsComplete=false,Secret=null},
                new TodoItem{Id=8,Name="Olivetto",IsComplete=false,Secret=null}
            };
            foreach (TodoItem t in todoItems)
            {
                context.TodoItems.Add(t);
            }
            context.SaveChanges();
        }
    }
}
