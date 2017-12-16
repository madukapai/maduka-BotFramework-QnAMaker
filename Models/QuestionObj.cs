using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QnABot.Models
{
    public class QuestionObj
    {
        QnAMakerDbEntities db = new QnAMakerDbEntities();

        /// <summary>
        /// 找出指定層級的卡片
        /// </summary>
        /// <param name="intParensId"></param>
        /// <returns></returns>
        public List<CardAction> GetQuestionCards(int intParensId)
        {
            List<CardAction> cards = new List<CardAction>();
            List<QuestionFile> questions = db.QuestionFile.Where(x => x.ParentId == intParensId && x.IsShow == true)
                                                          .OrderBy(o=>o.Seq)
                                                          .ToList();

            for (int i=0; i<questions.Count; i++)
            {
                cards.Add(new CardAction()
                {
                    Title = questions[i].Question,
                    Type = ActionTypes.ImBack,
                    Value = questions[i].Question,
                });
            }

            return cards;
        }

        /// <summary>
        /// 找出下一層的卡片
        /// </summary>
        /// <param name="strQuestion"></param>
        /// <returns></returns>
        public List<CardAction> GetNextQuestionCards(string strQuestion)
        {
            List<CardAction> cards = new List<CardAction>();
            List<QuestionFile> questions = db.QuestionFile
                .Where(x =>
                    db.QuestionFile
                    .Where(p => p.Question == strQuestion)
                    .Select(c => c.QuestionId)
                    .Contains(x.ParentId) && x.IsShow == true)
               .OrderBy(o => o.Seq)
               .ToList();

            for (int i = 0; i < questions.Count; i++)
            {
                cards.Add(new CardAction()
                {
                    Title = questions[i].Question,
                    Type = ActionTypes.ImBack,
                    Value = questions[i].Question,
                });
            }

            return cards;
        }

        /// <summary>
        /// 找出指定的問題資料
        /// </summary>
        /// <param name="strQuestion"></param>
        /// <returns></returns>
        public QuestionFile GetQuestionAnswer(string strQuestion)
        {
            return db.QuestionFile.Where(x => x.Question == strQuestion).FirstOrDefault();
        }

        /// <summary>
        /// 取得與這個問題同一個層級的問題清單
        /// </summary>
        /// <param name="strQuestion"></param>
        /// <returns></returns>
        public List<CardAction> GetSameLevelQuestion(string strQuestion)
        {
            List<CardAction> cards = new List<CardAction>();
            List<QuestionFile> questions = db.QuestionFile
                .Where(x =>
                    db.QuestionFile
                    .Where(p => p.Question == strQuestion)
                    .Select(c => c.ParentId)
                    .Contains(x.ParentId) && x.IsShow == true)
               .OrderBy(o => o.Seq)
               .ToList();

            for (int i = 0; i < questions.Count; i++)
            {
                cards.Add(new CardAction()
                {
                    Title = questions[i].Question,
                    Type = ActionTypes.ImBack,
                    Value = questions[i].Question,
                });
            }

            return cards;
        }
    }
}