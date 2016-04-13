using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Microsoft.ProjectOxford.Emotion;

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