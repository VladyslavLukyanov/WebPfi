using System;
using System.Collections.Generic;
using System.Linq;
using ChatManager.Models;
using System.Web.Mvc;

namespace ChatManager.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int IdReceveur { get; set; } // Celui qui recoit le message
        public int IdExpediteur { get; set; } // Celui qui envoie le message
        public string Contenu { get; set; }

        public bool Lu { get; set; }

        public DateTime Date { get; set; }

        static public List<Message> Messages { get; set; } = new List<Message>();

        public Message (int idReceveur, int idExpediteur, string contenu)
        {
            IdReceveur = idReceveur;
            IdExpediteur = idExpediteur;
            Contenu = contenu;
            Date = DateTime.Now;
            Lu = false;
        }

        public Message () { }

        static public bool IsCorrectMessage (string message)
        {
            return message.Trim().Length > 0;
        }

        static public void RemplirListeMessages(string chemin)
        {
            DB.Messages.Init(chemin);
            Messages = DB.Messages.ToList(); 
        }

        static public bool SessionUserMessage(int idUser)
        {
            return Messages.Any(m => m.IdExpediteur == idUser);
        }

        static public int NbMessages (int sessionUserId, int receveurId)
        {
            int compteur = 0;
            foreach (var message in Messages)
            {
                if (message.IdReceveur == receveurId && message.IdExpediteur == sessionUserId)
                    compteur++;
            } 
            return compteur;
        }
    }
}