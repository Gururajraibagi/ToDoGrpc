using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ToDoGrpc;
using ToDoGrpc.Data;
using ToDoGrpc.Models;

namespace ToDoGrpc.Services
{
    public class ToDoSerice : ToDo.ToDoBase
    {
        private readonly AppDbContext _appDbContext;
        //private readonly ILogger<ToDoSerice> _logger;
        public ToDoSerice(AppDbContext appDbContext)//ILogger<ToDoSerice> logger)
        {
            _appDbContext = appDbContext;
           // _logger = logger;
        }

        public override async Task<CreateToDoResponse> CreateToDO(CreateToDoRequest request, ServerCallContext context)
        {
            if(request.Title == string.Empty || request.Description == string.Empty)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You Must supply parameters"));
            }
            var todoitem = new ToDoItem
            {
                Title = request.Title,
                Description = request.Description
            };

            await _appDbContext.AddAsync(todoitem);
            await _appDbContext.SaveChangesAsync();

            return await Task.FromResult(new CreateToDoResponse
            {
                // "Hello " + request.Title +""// "Hello " + request.Title +""
                Id = todoitem.Id
            });
        }
        public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
        {
            if (request.Id == null)// || request.Description == string.Empty)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You Must supply ID"));
            }
            //var todoitem = new ToDoItem
            //{
            //    Title = request.Title,
            //    Description = request.Description
            //};
            //var todoitem = await _appDbContext.FindAsync<ToDoItem>(request.Id);
            var todoitem = await _appDbContext.ToDoItems.FirstOrDefaultAsync<ToDoItem>(t=> t.Id== request.Id);

            if(todoitem != null)
            {

            return await Task.FromResult(new ReadToDoResponse
            {
                
                // "Hello " + request.Title +""// "Hello " + request.Title +""
                Id = todoitem.Id,
                Description = todoitem.Description,
                Title = todoitem.Title,
            });
            }
            throw new RpcException(new Status(StatusCode.NotFound, $"Can not find the ID {request.Id}"));
        }
        public override async Task<GetAllResponse> GetAll(GetAllRequest request, ServerCallContext context)
        {

            //var todoitem = new ToDoItem
            //{
            //    Title = request.Title,
            //    Description = request.Description
            //};
            var todoitems = await _appDbContext.ToDoItems.ToListAsync<ToDoItem>();
            var response = new GetAllResponse();
            foreach(var todoitem in todoitems)
            {
                response.ToDo.Add(new ReadToDoResponse
                {
                    Id = todoitem.Id,
                    Description = todoitem.Description,
                    Title = todoitem.Title,
                });
            }
            return await Task.FromResult(response);
        }

        public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
        {
            if (request.Title == string.Empty || request.Description == string.Empty || request.Id == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You Must supply parameters"));
            }
            var todoitem = new ToDoItem
            {
                Title = request.Title,
                Description = request.Description,
                TodoStatus = "Edited",
            };

            if(todoitem !=null)
            {
            //await _appDbContext.ToDoItems.ExecuteUpdateAsync(todoitem);
            //await _appDbContext.AddAsync(todoitem);
            await _appDbContext.SaveChangesAsync();
            }

            return await Task.FromResult(new UpdateToDoResponse
            {
                Id = todoitem.Id
            }) ;
        }
        public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
        {
            if (request.Id == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You Must supply parameters"));
            }



            var todoitem = await _appDbContext.ToDoItems.FirstOrDefaultAsync<ToDoItem>(t => t.Id == request.Id);
            if (todoitem != null)
            {

                _appDbContext.Remove(todoitem);
                await _appDbContext.SaveChangesAsync();
            }
            return await Task.FromResult(new DeleteToDoResponse
            {
                // "Hello " + request.Title +""// "Hello " + request.Title +""
                Id = todoitem.Id
            });
        }
    }
}