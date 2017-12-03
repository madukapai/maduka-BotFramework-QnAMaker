using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;

namespace Microsoft.Bot.Sample.QnABot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-qnamaker
    [Serializable]
    public class BasicQnAMakerDialog : QnAMakerDialog
    {
        // Go to https://qnamaker.ai and feed data, train & publish your QnA Knowledgebase.
        public BasicQnAMakerDialog() : base(
            new QnAMakerService(
                new QnAMakerAttribute(
                    ConfigurationManager.AppSettings["QnASubscriptionKey"],
                    ConfigurationManager.AppSettings["QnAKnowledgebaseId"],
                    ""
                )
            )
        )
        {
            // If you're running this bot locally, make sure you have these appSettings in youe web.config
        }

        protected override async Task QnAFeedbackStepAsync(IDialogContext context, QnAMakerResults qnaMakerResults)
        {
            // responding with the top answer when score is above some threshold
            if (qnaMakerResults.Answers.Count > 0 && qnaMakerResults.Answers.FirstOrDefault().Score > 0.75)
            {
                await context.PostAsync(qnaMakerResults.Answers.FirstOrDefault().Answer);
            }
            else
            {
                await base.QnAFeedbackStepAsync(context, qnaMakerResults);
            }
        }


        protected override bool IsConfidentAnswer(QnAMakerResults qnaMakerResults)
        {
            return base.IsConfidentAnswer(qnaMakerResults);
        }

        protected override Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            if (result.Answers.Count == 0)
            {
                return base.DefaultWaitNextMessageAsync(context, message, result);
            }
            else
            {
                return base.DefaultWaitNextMessageAsync(context, message, result);
            }
        }
    }
}