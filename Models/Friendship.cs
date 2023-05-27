using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace ChatManager.Models
{
    public class Friendship
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TargetUserID { get; set; }
        public int UserDeclinedId { get; set; }
        public bool Declined { get; set; }
        public bool Accepted { get; set; }
        public DateTime CreationDate { get; set; }
        static public List<Friendship> Friendships { get; private set; } = new List<Friendship>();

        public Friendship(int userId, int targetUserId)
        {
            CreationDate = DateTime.Now;
            Declined = false;
            Accepted = false;
            UserId = userId;
            UserDeclinedId = -1;
            TargetUserID = targetUserId;
        }

        public Friendship() { }

        public Friendship Clone()
        {
            return JsonConvert.DeserializeObject<Friendship>(JsonConvert.SerializeObject(this));
        }

        public static bool IsAccepted(int idF, string chemin)
        {
            RemplirListe(chemin);
            return Friendships.Any(f => f.Id == idF && f.Accepted);
        }


        static public int GetUserDeclinedId(int friendshipId, string chemin)
        {
            RemplirListe(chemin);
            Friendship friendship = DB.Friends.Get(friendshipId);

            if (friendship != null && friendship.Declined)
                return friendship.UserDeclinedId;
            
            return -1;
        }


        public static void RemplirListe(string path)
        {
            DB.Friends.Init(path);
            Friendships = DB.Friends.ToList(); // liste des amities
        }

        public static (bool e, int n, int f) IsFriendShipOnWait(int idUser, int sessionUserId, string chemin)
        {
            // demande envoyée 
            RemplirListe(chemin);
            foreach (var friendship in Friendships)
            {
                if (friendship.TargetUserID == sessionUserId && friendship.UserId == idUser)
                {
                    return (true, sessionUserId, friendship.Id);
                }
                else if (friendship.TargetUserID == idUser && friendship.UserId == sessionUserId)
                {
                    return (true, idUser, friendship.Id);
                }
            }

            return (false, -1, -1);
        }

        public static bool DeclinedFriendship (int idF)
        {
            return Friendships.Any(f => f.Id == idF && f.Declined);
        }

    }
}