﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts
{
    public class People
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Depart { get; set; }
        public string SpecialSys { get; set; }
        public string Note { get; set; }

        public void NoteToSpecialSys()
        {
            if(!string.IsNullOrEmpty(Note) && !Note.Equals("科長") 
                && !Note.Equals("Boss") 
                && !Note.Equals("處長") 
                && !Note.Equals("工讀生"))
            {
                SpecialSys = Note;
            }
        }
    }
}
