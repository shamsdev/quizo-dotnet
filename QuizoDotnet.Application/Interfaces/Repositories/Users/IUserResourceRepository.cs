using KarizmaPlatform.Core.Logic;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Application.Interfaces.Repositories.Users;

public interface IUserResourceRepository : IRepository<UserResource>
{
    Task<UserResource> Get(long userId); 
}