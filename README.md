# XamarinPlayground

### The Challenge
In this challenge, you will use one of the Microsoft Cognitive Services API to bring intelligence in a Xamarin-based cross-platform application.

The goal is to use the Emotion API to get your happiness percentage after getting a picture of you with the device/emulator camera.

The walkthrough below should help you with the callenge, but you can also get in touch with @benjiiim or @aspenwilder with questions--both of whom are on site at Evolve and happy to help!

The walkthrough has been written with Visual Studio 2015 Update 2 (with Xamarin tools installed) but should work in a similar way with Xamarin Studio.

### Challenge Walkthrough

##### Get a Cognitive Service Emotion API trial key
 
To get a trial key and be able to use the Emotion API, go to www.microsoft.com/cognitive-services, use the My account link on the top-right corner and login with a Microsoft Account (ex Live ID).

Click on the "Request new trials" button, choose the Emotion API product ans accept the services terms and the privacy statement to subscribe.

Keep one of the generated key with you as you will need it later.

##### Create a Android blank project in Visual Studio 2015 with updates and Xamarin setup

With Visual Studio 2015, create a new project with the Templates > Visual C# > Android > Blank App (Android) project template. Use a name like "AndroidApp" and a new solution with a name like "XamarinCognitive".

Add the following Nuget packages to your solution (in the correct order) :
* Microsoft.Bcl.Build
* Microsoft.ProjectOxford.Emotion

##### Write (copy/paste) the code for the UI layer to get the camera stream, take a picture and send the stream to the shared code

TODO

##### Create a shared project with the logic to call the Cognitive Service API

Create a new project in your solution, with the Templates > Visual C# > Shared Project template. Use a name like "SharedProject".

Create a new class in the project, with the name Core.cs.

Copy/Paste the following code, by replacing the placeholder with your Emotion API key.

    private static async Task<Emotion[]> GetHappiness(Stream stream)
    {
        string emotionKey = "YourKeyHere";

        EmotionServiceClient emotionClient = new EmotionServiceClient(emotionKey);

        var emotionResults = await emotionClient.RecognizeAsync(stream);

        if (emotionResults == null || emotionResults.Count() == 0)
        {
            throw new Exception("Can't detect face");
        }

        return emotionResults;
    }

    //Average happiness calculation in case of multiple people
    public static async Task<float> GetAverageHappinessScore(Stream stream)
    {
        Emotion[] emotionResults = await GetHappiness(stream);

        float score = 0;
        foreach (var emotionResult in emotionResults)
        {
            score = score + emotionResult.Scores.Happiness;
        }

        return score / emotionResults.Count();
    }

    public static string GetHappinessMessage(float score)
    {
        score = score * 100;
        double result = Math.Round(score, 2);

        if (score >= 50)
            return result + " % :-)";
        else
            return result + "% :-(";
    }

#### Bonus Challenge #1 Walkthrough
##### Create a Windows UWP project in the same solution

TODO

##### Write (copy/paste) the code for the UI layer to get the camera stream, take a picture and send the stream to the shared code

TODO
