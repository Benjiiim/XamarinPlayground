using Microsoft.ProjectOxford.Emotion;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SharedProject
{
    public class Core
    {
        public static async Task<float> GetHappiness(Stream stream)
        {
            string emotionKey = "88f748eefd944a5d8d337a1765414bba";

            EmotionServiceClient emotionClient = new EmotionServiceClient(emotionKey);

            var emotionResults = await emotionClient.RecognizeAsync(stream);

            if (emotionResults == null || emotionResults.Count() == 0)
            {
                throw new Exception("Can't detect face");
            }

            //Average happiness calculation in case of multiple people
            float score = 0;
            foreach (var emotionResult in emotionResults)
            {
                score = score + emotionResult.Scores.Happiness;
            }
            score = score / emotionResults.Count();

            return score;
        }
    }
}