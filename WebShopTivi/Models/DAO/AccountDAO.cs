using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopTivi.Models.EF;

namespace WebShopTivi.Models.DAO
{
    public class AccountDAO
    {
        ShopTiviDBModel db = new ShopTiviDBModel();
        public IEnumerable<Account> ListAllPaging(string searchString, int page, int pagesize)
        {
            IQueryable<Account> model = db.Accounts;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(n => n.Username.Contains(searchString));
            }
            return model.OrderBy(n => n.Id).ToPagedList(page, pagesize);
        }
        public int Insert(Account entity, string path)
        {
            var account = new Account();
            account.Username = entity.Username;
            account.Password = entity.Password;
            account.Image = path;
            account.Fullname = entity.Fullname;
            account.Status = entity.Status;
            db.Accounts.Add(account);
            db.SaveChanges();

            return entity.Id;
        }

        public Account ViewDetail(int id)
        {
            return db.Accounts.Find(id);
        }

        public bool Update(Account entity, string path)
        {
            try
            {
                var taikhoan = db.Accounts.Find(entity.Id);
                taikhoan.Username = entity.Username;
                taikhoan.Password = entity.Password;
                if(path == "-1")
                {
                    taikhoan.Image = entity.Image;
                }
                else
                {
                    taikhoan.Image = path;
                }
                
                taikhoan.Fullname = entity.Fullname;
                taikhoan.Status = entity.Status;
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var taikhoan = db.Accounts.Find(id);
                db.Accounts.Remove(taikhoan);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ChangeStatus(int? id)
        {
            var account = db.Accounts.Find(id);
            account.Status = !account.Status;
            db.SaveChanges();
            return account.Status;
        }
    }
}