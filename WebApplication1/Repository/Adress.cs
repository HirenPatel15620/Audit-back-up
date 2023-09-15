using Microsoft.EntityFrameworkCore;
using Model.Data;
using Model.Models;
using Model.ViewModel;
using Model.ViewModel.Adress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IAdress
    {
        Task<CreateAdressViewModel> GetAdress(int id);
        Task<Model.Models.Address> PostAdress(CreateAdressViewModel createAdressViewModel);
        Task<Model.Models.Address> PutAdress(CreateAdressViewModel createAdressViewModel);
        Task<CreateAdressViewModel> DeleteAdress(int id);

    }


    public class Adress : IAdress
    {
        private readonly AuditDatabaseContext _auditDatabaseContext;

        public Adress(AuditDatabaseContext auditDatabaseContext)
        {
            _auditDatabaseContext = auditDatabaseContext;

        }

        public async Task<CreateAdressViewModel> GetAdress(int id)
        {
            CreateAdressViewModel userViewModel = new CreateAdressViewModel();

            var user = await _auditDatabaseContext.Addresses.Where(x => x.Id == id).FirstOrDefaultAsync();

            userViewModel.Id = id;
            userViewModel.Address1 = user.Address1;
            userViewModel.Pincode = user.Pincode;

            return userViewModel;

        }
        public async Task<Model.Models.Address> PostAdress(CreateAdressViewModel createAdressViewModel)
        {
            Model.Models.Address Model = new Model.Models.Address();

            if (createAdressViewModel != null)
            {
                Model.Id = createAdressViewModel.Id;
                Model.Address1 = createAdressViewModel.Address1;
                Model.Pincode = createAdressViewModel.Pincode;


                await _auditDatabaseContext.Addresses.AddAsync(Model);
                await _auditDatabaseContext.SaveChangesAsync();
            }

            return Model;
        }
        public async Task<Model.Models.Address> PutAdress(CreateAdressViewModel createAdressViewModel)
        {
            var model = await _auditDatabaseContext.Addresses.Where(x => x.Id == createAdressViewModel.Id).FirstOrDefaultAsync();
            model.Id = createAdressViewModel.Id;
            model.Address1 = createAdressViewModel.Address1;
            model.Pincode = createAdressViewModel.Pincode;


            _auditDatabaseContext.Addresses.Update(model);
            _auditDatabaseContext.SaveChanges();

            return model;
        }
        public async Task<CreateAdressViewModel> DeleteAdress(int id)
        {

            var user = await _auditDatabaseContext.Addresses.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (user != null)
            {
                _auditDatabaseContext.Addresses.Remove(user);
                await _auditDatabaseContext.SaveChangesAsync();
            }

            return null;
        }




    }
}
