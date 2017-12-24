using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QnABot.Models
{
    public class QnaMakerModel
    {

        public class QnAMakerQuery
        {
            public string question { get; set; }
        }


        public class QnAMakerResult
        {
            public List<Answer> answers { get; set; }

            public class Answer
            {
                public string answer { get; set; }
                public List<string> questions { get; set; }
                public float score { get; set; }
            }
        }
    }
}