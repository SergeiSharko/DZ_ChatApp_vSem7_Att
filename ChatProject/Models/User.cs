﻿namespace DZ_ChatApp_vSem7.ChatProject.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<Message>? MessagesToUser { get; set; }
        public virtual ICollection<Message>? MessagesFromUser { get; set; }
    }
}
