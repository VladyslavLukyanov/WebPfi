using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class MessageRepository : Repository<Message>
    {
        public Message Create(Message message)
        {
            try
            {
                message.Date = DateTime.Now;
                message.Id = base.Add(message);
                return message;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Add frienship failed : Message - {ex.Message}");
            }
            return null;
        }

        public override bool Update(Message message)
        {
            try
            {
                return base.Update(message);
            } 
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Update friendship failed : Message - {e.Message}");
            }
            return false;
        }

        public override bool Delete(int Id)
        {
            try
            {
                Message message = DB.Messages.Get(Id);
                if (message != null)
                {
                    BeginTransaction();
                    base.Delete(Id);
                    EndTransaction();
                    return true;
                }
                return false;

            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Remove friendship failed : Message - {e.Message}");
                return false;
            }
        }

    }
}