using ChatManager.Models;
using System.Web.Mvc;

namespace ChatManager.Controllers
{
    public class MessageController : Controller
    {
        MessageRepository M_REP = new MessageRepository();
        public string Path { get; } = "~/App_Data/Messages.json";
        // GET: Message
        public ActionResult Index()
        {
            return View();
        }

        public int Vu
        {
            get { return (int)Session["Vu"]; }
            set
            {
                Session["Vu"] = value;
            }
        }
        public int FriendId
        {
            get { return (int)Session["FriendId"]; }
            set
            {
                Session["FriendId"] = value;
            }
        }

        [HttpPost]
        public ActionResult SendMessage(int expediteurId, int receveurId, string contenu)
        {
            string path = Server.MapPath(Path);

            M_REP.Init(path);

            if (Message.IsCorrectMessage(contenu))
            {
                Message message = new Message(receveurId, expediteurId, contenu);
                OnlineUsers.SetHasChanged();
                M_REP.Create(message);
                return Json("Message sended", JsonRequestBehavior.AllowGet);
            }
            return Json("Incorrect message", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ModifyMessage(int idMessage, string newContent)
        {
            M_REP.Init(Server.MapPath(Path));

            Message message = DB.Messages.Get(idMessage);
            if (message != null)
            {
                message.Contenu = newContent;
                M_REP.Update(message);
                OnlineUsers.SetHasChanged();
                return Json($"sms changed : {message.Contenu}", JsonRequestBehavior.AllowGet);
            }
            return Json("sms has not changed", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteMessage(int idMessage)
        {
            M_REP.Init(Server.MapPath(Path));
            
            if (M_REP.Delete(idMessage))
            {
                OnlineUsers.SetHasChanged();
                return Json("sms deleted", JsonRequestBehavior.AllowGet);
            }
            return Json("sms has not deleted", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMessages(bool forceRefresh = false)
        {
            if (OnlineUsers.HasChanged() || forceRefresh)
                return PartialView(DB.Messages.ToList());
            return null;
        }

        [HttpGet]
        public ActionResult DeleteMessagesFor(int idUser)
        {
            Message.RemplirListeMessages(Server.MapPath(Path));
            var copy = Message.Messages;
            foreach (var message in copy)
            {
                if(message.IdReceveur == idUser || message.IdExpediteur == idUser)
                {
                    DeleteMessage(message.Id);
                }
            }
            return Json("Deleted", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAllMessages(bool forceRefresh = false)
        {
            return PartialView(DB.Messages.ToList());
        }

        public ActionResult GetFriends(bool forceRefresh = false)
        {
            if(forceRefresh || OnlineUsers.HasChanged())
                return PartialView(DB.Users.SortedUsers());
            return null;
        }

        [HttpGet]
        public ActionResult ChangeFriend(int friendId) 
        {
            FriendId = friendId;
            User friend = DB.Users.Get(FriendId);

            if (friend != null && FriendId == friendId)
            {
                Vu = 1;
                return Json("Friend !! changed", JsonRequestBehavior.AllowGet);
            }
            return Json("Friend not !! changed", JsonRequestBehavior.AllowGet);
        }
    }
}