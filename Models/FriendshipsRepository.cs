using System;

namespace ChatManager.Models
{
    public class FriendshipsRepository:Repository<Friendship>
    {
        public Friendship Create(Friendship friendship)
        {
            try
            {
                friendship.CreationDate = DateTime.Now;
                friendship.Id = base.Add(friendship);
                return friendship;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Add frienship failed : Message - {ex.Message}");
            }
            return null;
        }

        public override bool Update(Friendship friendship)
        {
            try
            {
                return base.Update(friendship);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Update friendship failed : Message - {e.Message}");
            }
            return false;
        }

        public override bool Delete(int friendshipId)
        {
            try
            {
                Friendship friendship = DB.Friends.Get(friendshipId);
                if(friendship != null)
                {
                    BeginTransaction();
                    base.Delete(friendshipId);
                    EndTransaction();
                    return true;
                }
                return false;
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Remove friendship failed : Message - {e.Message}");
                EndTransaction();
                return false;
            }
        }
    }
}