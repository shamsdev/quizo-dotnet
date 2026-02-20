using KarizmaPlatform.Core.Logic;
using QuizoDotnet.Domain.Models.Users;

namespace QuizoDotnet.Application.Interfaces.Repositories.Users;

public interface IUserEnergyRepository : IRepository<UserEnergy>
{
    Task<UserEnergy> Get(long userId);
}