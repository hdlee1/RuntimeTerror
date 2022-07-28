using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectTemplate
{
    public class Post
    {
        public int id;
        public int uid;
        public string userName;
        public string postText;
        public string department;
        public string postDate;
        public int likes;
        public int dislikes;
        public bool hasComments;
        public bool isSolved;
        public bool isRejected;
    }
}