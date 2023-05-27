using ChatManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ChatManager.Controllers
{
    public class FriendshipsController : Controller
    {
        FriendshipsRepository F_REP = new FriendshipsRepository();
        public string Path = "~/App_Data/Friends.json";

        public ActionResult Index()
        {
            return View();
        }


        #region PropsFilter
        public int NotFriendsFilterChecked
        {
            set => Session["NotFriendsFilter"] = value;
            get => (int)Session["NotFriendsFilter"];
        }

        public int IsOnRequestFilter
        {
            set { Session["Request"] = value; }
            get
            {
                return (int)Session["Request"];
            }
        }

        public int RefusedFriendshipFilter
        {
            set { Session["Refused"] = value; }
            get
            {
                return (int)Session["Refused"];
            }
        }

        public int FriendsFilter
        {
            set { Session["Friend"] = value; }
            get
            {
                return (int)Session["Friend"];
            }
        }

        public int PendingFriendship
        {
            set => Session["Pending"] = value;
            get
            {
                return (int)Session["Pending"];
            }
        }

        public int BlockedFilter
        {
            set
            {
                Session["BlockedFilter"] = value;
            }
            get => (int)Session["BlockedFilter"];
        }

        public string SearchName
        {
            set => Session["SearchText"] = value;
            get => (string)Session["SearchText"];
        }
        #endregion

        public List<User> ListeUsagers()
        {
            var userSession = OnlineUsers.GetSessionUser();

            var users = new List<User>();

            foreach (var user in DB.Users.SortedUsers().ToList())
            {
                var friendshipInformations = Friendship.IsFriendShipOnWait(user.Id, userSession.Id, Server.MapPath(Path));

                if (!String.IsNullOrEmpty(SearchName))
                {
                    if (user.FirstName.ToLower().Contains(SearchName.ToLower()) || user.LastName.ToLower().Contains(SearchName.ToLower()))
                        users.Add(user);
                }
                else
                    users.Add(user);
                
                if (BlockedFilter == -1 && user.Blocked)
                    users.Remove(user);

                if (NotFriendsFilterChecked == -1 && !friendshipInformations.e && !user.Blocked)
                    users.Remove(user);

                if (PendingFriendship == -1 && friendshipInformations.n == userSession.Id && !Friendship.DeclinedFriendship(friendshipInformations.f))
                    users.Remove(user);

                if (FriendsFilter == -1 && Friendship.IsAccepted(friendshipInformations.f, Server.MapPath(Path)))
                    users.Remove(user);

                if (IsOnRequestFilter == -1 && friendshipInformations.n == user.Id && !Friendship.IsAccepted(friendshipInformations.f, Server.MapPath(Path)))
                    users.Remove(user);

                if (RefusedFriendshipFilter == -1 && Friendship.GetUserDeclinedId(friendshipInformations.f, Server.MapPath(Path)) == user.Id)
                    users.Remove(user);
            }
            return users;
        }

        #region filters

        public ActionResult Search(string text)
        {
            SearchName = text;
            return null;
        }

        public ActionResult NotFriends(bool isChecked)
        {
            NotFriendsFilterChecked = isChecked ? 0 : -1;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Pending(bool isChecked)
        {
            PendingFriendship = isChecked ? 0 : -1;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Blocked(bool isChecked)
        {
            BlockedFilter = isChecked ? 0 : -1;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IsOnRequest(bool isChecked)
        {
            IsOnRequestFilter = isChecked ? 0 : -1;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RefusedFriendship(bool isChecked)
        {
            RefusedFriendshipFilter = isChecked ? 0 : -1;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AreFriends(bool isChecked)
        {
            FriendsFilter = isChecked ? 0 : -1;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion
      

        #region friendship

        [HttpGet]
        public ActionResult CreateFriendship(int userId, int targetUserId, int friendshipId)
        {
            F_REP.Init(Server.MapPath(Path));

            Friendship friendship = DB.Friends.Get(friendshipId);

            if (friendship != null)
            {
                friendship.Declined = false;
                friendship.UserId = userId;
                friendship.TargetUserID = targetUserId;
                friendship.UserDeclinedId = -1;
                friendship.CreationDate = DateTime.Now;
                OnlineUsers.SetHasChanged();
                OnlineUsers.AddNotification(userId, "");
                F_REP.Update(friendship);
                return Json("Amitie cree", JsonRequestBehavior.AllowGet);
            }

            friendship = new Friendship(userId, targetUserId);
            OnlineUsers.SetHasChanged();
            F_REP.Create(friendship);
            return Json("Amitie cree", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ModifyFriendShip(int friendShipId, bool response, int targetUserId)
        {
            F_REP.Init(Server.MapPath(Path));
            
            Friendship friendship = DB.Friends.Get(friendShipId);
            if (friendship != null)
            {
                if (response)
                    friendship.Accepted = true;
                else
                {
                    friendship.Declined = true;
                    friendship.UserDeclinedId = targetUserId;
                }

                friendship.CreationDate = DateTime.Now;
                OnlineUsers.SetHasChanged();
                F_REP.Update(friendship);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult RetireFriendShip(int friendShipId)
        {
            F_REP.Init(Server.MapPath(Path));

            Friendship friendship = DB.Friends.Get(friendShipId);

            if (friendship != null)
            {
                F_REP.Delete(friendShipId);
                OnlineUsers.SetHasChanged();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult GetUsers(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged())
                return PartialView(ListeUsagers());
            return null;
        }
    }
}