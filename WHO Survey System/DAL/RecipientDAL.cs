using WHO_Survey_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHO_Survey_System.DAL
{
    public class RecipientDAL
    {
        #region Recipient
        //public List<Recipient> GetActiveRecipientList(DatabaseEntities de)
        //{
        //    return de.Recipients.Where(x => x.IsActive == 1).ToList();
        //}

        //public List<Recipient> GetAllRecipientList(DatabaseEntities de)
        //{
        //    return de.Recipients.ToList();
        //}

        //public Recipient GetRecipientById(int Id, DatabaseEntities de)
        //{
        //    return de.Recipients.Where(x => x.Id == Id).FirstOrDefault(x => x.IsActive == 1);
        //}

        //public bool AddRecipient(Recipient _recipient, DatabaseEntities de)
        //{
        //    try
        //    {
        //        de.Recipients.Add(_recipient);
        //        de.SaveChanges();

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //public bool UpdateRecipient(Recipient _recipient, DatabaseEntities de)
        //{
        //    try
        //    {
        //        _recipient.UpdatedAt = DateTime.UtcNow;
        //        de.Entry(_recipient).State = System.Data.Entity.EntityState.Modified;
        //        de.SaveChanges();
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        #endregion
    }
}