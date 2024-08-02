﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_Assignment2
{
    abstract class User
    {
        private int id;
        private string fullName;
        private int contactNum;
        private string email;
        private DateTime dateOfBirth;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }
        public int ContactNum
        {
            get { return contactNum; }
            set { contactNum = value; }
        }
        public string Email
        { 
            get { return email; }
            set { email = value; }
        }
        public DateTime DateOfBirth
        {
            get { return dateOfBirth; }
            set { dateOfBirth = value; }
        }
        public User() { }
        public User(int id, string fullName, int contactNum, string email, DateTime dateOfBirth)
        {
            this.id = id;
            this.fullName = fullName;
            this.contactNum = contactNum;
            this.email = email;
            this.dateOfBirth = dateOfBirth;
        }
    }
}
