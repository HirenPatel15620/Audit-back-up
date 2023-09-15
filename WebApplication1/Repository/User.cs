using Microsoft.EntityFrameworkCore;
using Model.Data;
using Model.Models;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IUser
    {
        Task<GetUserViewModel> GetUser(int id);
        Task<Model.Models.User> PostUser(CreateUserViewModel createUserViewModel);
        Task<Model.Models.User> PutUser(UpdateUserViewModel updateUserViewModel);
        Task<DeleteUserViewModel> DeleteUser(int id);

    }


    public class User : IUser
    {
        private readonly AuditDatabaseContext _auditDatabaseContext;

        public User(AuditDatabaseContext auditDatabaseContext)
        {
            _auditDatabaseContext = auditDatabaseContext;

        }

        public async Task<GetUserViewModel> GetUser(int id)
        {
            GetUserViewModel userViewModel = new GetUserViewModel();

            var user = await _auditDatabaseContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();

            userViewModel.Id = id;
            userViewModel.Email = user.Email;
            userViewModel.Password = user.Password;
            userViewModel.Contact = user.Contact;

            return userViewModel;

        }
        public async Task<Model.Models.User> PostUser(CreateUserViewModel createUserViewModel)
        {
            Model.Models.User Model = new Model.Models.User();

            if (createUserViewModel != null)
            {
                Model.Email = createUserViewModel.Email;
                Model.Password = createUserViewModel.Password;
                Model.Contact = createUserViewModel.Contact;


                await _auditDatabaseContext.Users.AddAsync(Model);
                await _auditDatabaseContext.SaveChangesAsync();
            }

            return Model;
        }
        public async Task<Model.Models.User> PutUser(UpdateUserViewModel updateUserViewModel)
        {
            var model = await _auditDatabaseContext.Users.Where(x => x.Id == updateUserViewModel.Id).FirstOrDefaultAsync();
            model.Email = updateUserViewModel.Email;
            model.Password = updateUserViewModel.Password;
            model.Contact = updateUserViewModel.Contact;


            _auditDatabaseContext.Users.Update(model);
            //await _auditDatabaseContext.SaveChangesAsync();
            _auditDatabaseContext.SaveChanges();


            return model;
        }
        public async Task<DeleteUserViewModel> DeleteUser(int id)
        {

            var user = await _auditDatabaseContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (user != null)
            {
                _auditDatabaseContext.Users.Remove(user);
                await _auditDatabaseContext.SaveChangesAsync();
            }

            return null;
        }




    }
}
