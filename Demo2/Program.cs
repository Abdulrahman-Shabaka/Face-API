
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;


namespace Demo2
{
    class Program
    {
        //your subscription data
        const string SUBSCRIPTION_KEY = "<your subscription key>";
        const string ENDPOINT = "<your api endpoint>";
        static string sourceImageUrl;
        //your subscription data

        static void Main(string[] args)
        {

            Console.WriteLine("\n\n========================================This project developed by Abdulrahman Hassan Shabaka========================================\n\n");

            //set the recognition model
            const string RECOGNITION_MODEL3 = RecognitionModel.Recognition03;
            //set the recognition model


            //set authentication data
            IFaceClient client = Authenticate(ENDPOINT, SUBSCRIPTION_KEY);
            //set authentication data

            //programe options

            xx:
            Console.WriteLine("\n===================================================================================================================\n1-Detection Model 1\n1-Detection Model 2\n3-Find similar Faces\n4-Verify Faces\n\nenter the number of the option you want to run.");
            string selection = Console.ReadLine();
            try
            {
                switch (selection)
                {
                    //detection model 1
                    case "1":
                        {
                            Console.WriteLine("Enter url for an image you want to detected.");
                            sourceImageUrl = Console.ReadLine();
                            DetectFaceExtract(client, sourceImageUrl, RECOGNITION_MODEL3).Wait();

                            goto xx;
                        }
                    //detection model 1

                    //detection model 2
                    case "2":
                        {
                            Console.WriteLine("Enter url for an image you want to detected.");
                            sourceImageUrl = Console.ReadLine();
                            DetectFaceRecognize(client, sourceImageUrl, RECOGNITION_MODEL3).Wait();

                            goto xx;
                        }
                    //detection model 2

                    //find similar
                    case "3":
                        {
                            FindSimilar(client, RECOGNITION_MODEL3).Wait();

                            goto xx;
                        }
                    //find similar

                    //verify
                    case "4":
                        {
                            Verify(client, RECOGNITION_MODEL3).Wait();

                            goto xx;
                        }
                    //verify

                    default:
                        Console.WriteLine("you entered a wrong option, please try again.");
                        goto xx;
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                goto xx;
            }

            //programe options


        }


        //authenticate the key
        public static IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }
        //authenticate the key


        //detection model one
        public static async Task DetectFaceExtract(IFaceClient client, string url, string recognitionModel)
        {
            Console.WriteLine("======== DETECT FACE ========\n======== Detection Mode 1 ======== ");
            Console.WriteLine();




            IList<DetectedFace> detectedFaces;
                detectedFaces = await client.Face.DetectWithUrlAsync(url,
                        returnFaceAttributes: new List<FaceAttributeType?> { FaceAttributeType.Accessories, FaceAttributeType.Age,
                        FaceAttributeType.Blur, FaceAttributeType.Emotion, FaceAttributeType.Exposure, FaceAttributeType.FacialHair,
                        FaceAttributeType.Gender, FaceAttributeType.Glasses, FaceAttributeType.Hair, FaceAttributeType.HeadPose,
                        FaceAttributeType.Makeup, FaceAttributeType.Noise, FaceAttributeType.Occlusion, FaceAttributeType.Smile },
                        // We specify detection model 1 because we are retrieving attributes.
                        detectionModel: DetectionModel.Detection01,
                        recognitionModel: recognitionModel);

                Console.WriteLine($"{detectedFaces.Count} face detected from the link.");
               
                foreach (var face in detectedFaces)
                {
                    Console.WriteLine($"Face attributes for the link:");

                    // Get bounding box of the faces
                    Console.WriteLine($"Rectangle(Left/Top/Width/Height) : {face.FaceRectangle.Left} {face.FaceRectangle.Top} {face.FaceRectangle.Width} {face.FaceRectangle.Height}");

                    // Get accessories of the faces
                    List<Accessory> accessoriesList = (List<Accessory>)face.FaceAttributes.Accessories;
                    int count = face.FaceAttributes.Accessories.Count;
                    string accessory; string[] accessoryArray = new string[count];
                    if (count == 0) { accessory = "NoAccessories"; }
                    else
                    {
                        for (int i = 0; i < count; ++i) { accessoryArray[i] = accessoriesList[i].Type.ToString(); }
                        accessory = string.Join(",", accessoryArray);
                    }
                    Console.WriteLine($"Accessories : {accessory}");

                    // Get face other attributes
                    Console.WriteLine($"Age : {face.FaceAttributes.Age}");
                    Console.WriteLine($"Blur : {face.FaceAttributes.Blur.BlurLevel}");

                    // Get emotion on the face
                    string emotionType = string.Empty;
                    double emotionValue = 0.0;
                    Emotion emotion = face.FaceAttributes.Emotion;
                    if (emotion.Anger > emotionValue) { emotionValue = emotion.Anger; emotionType = "Anger"; }
                    if (emotion.Contempt > emotionValue) { emotionValue = emotion.Contempt; emotionType = "Contempt"; }
                    if (emotion.Disgust > emotionValue) { emotionValue = emotion.Disgust; emotionType = "Disgust"; }
                    if (emotion.Fear > emotionValue) { emotionValue = emotion.Fear; emotionType = "Fear"; }
                    if (emotion.Happiness > emotionValue) { emotionValue = emotion.Happiness; emotionType = "Happiness"; }
                    if (emotion.Neutral > emotionValue) { emotionValue = emotion.Neutral; emotionType = "Neutral"; }
                    if (emotion.Sadness > emotionValue) { emotionValue = emotion.Sadness; emotionType = "Sadness"; }
                    if (emotion.Surprise > emotionValue) { emotionType = "Surprise"; }
                    Console.WriteLine($"Emotion : {emotionType}");

                    // Get more face attributes
                    Console.WriteLine($"Exposure : {face.FaceAttributes.Exposure.ExposureLevel}");
                    Console.WriteLine($"FacialHair : {string.Format("{0}", face.FaceAttributes.FacialHair.Moustache + face.FaceAttributes.FacialHair.Beard + face.FaceAttributes.FacialHair.Sideburns > 0 ? "Yes" : "No")}");
                    Console.WriteLine($"Gender : {face.FaceAttributes.Gender}");
                    Console.WriteLine($"Glasses : {face.FaceAttributes.Glasses}");

                    // Get hair color
                    Hair hair = face.FaceAttributes.Hair;
                    string color = null;
                    if (hair.HairColor.Count == 0) { if (hair.Invisible) { color = "Invisible"; } else { color = "Bald"; } }
                    HairColorType returnColor = HairColorType.Unknown;
                    double maxConfidence = 0.0f;
                    foreach (HairColor hairColor in hair.HairColor)
                    {
                        if (hairColor.Confidence <= maxConfidence) { continue; }
                        maxConfidence = hairColor.Confidence; returnColor = hairColor.Color; color = returnColor.ToString();
                    }
                    Console.WriteLine($"Hair : {color}");

                    // Get more attributes
                    Console.WriteLine($"HeadPose : {string.Format("Pitch: {0}, Roll: {1}, Yaw: {2}", Math.Round(face.FaceAttributes.HeadPose.Pitch, 2), Math.Round(face.FaceAttributes.HeadPose.Roll, 2), Math.Round(face.FaceAttributes.HeadPose.Yaw, 2))}");
                    Console.WriteLine($"Makeup : {string.Format("{0}", (face.FaceAttributes.Makeup.EyeMakeup || face.FaceAttributes.Makeup.LipMakeup) ? "Yes" : "No")}");
                    Console.WriteLine($"Noise : {face.FaceAttributes.Noise.NoiseLevel}");
                    Console.WriteLine($"Occlusion : {string.Format("EyeOccluded: {0}", face.FaceAttributes.Occlusion.EyeOccluded ? "Yes" : "No")} " +
                        $" {string.Format("ForeheadOccluded: {0}", face.FaceAttributes.Occlusion.ForeheadOccluded ? "Yes" : "No")}   {string.Format("MouthOccluded: {0}", face.FaceAttributes.Occlusion.MouthOccluded ? "Yes" : "No")}");
                    Console.WriteLine($"Smile : {face.FaceAttributes.Smile}");
                    Console.WriteLine();
                }

        }
        //detection model one


        //detection model two

        //list for any detection model two faces
        static IList<DetectedFace> detectedFaces;
        private static async Task DetectFaceRecognize(IFaceClient faceClient, string url, string recognition_model)
        {
            Console.WriteLine("======== DETECT FACE ========\n======== Detection Mode 2 ======== ");
            Console.WriteLine();

            detectedFaces = await faceClient.Face.DetectWithUrlAsync(url, recognitionModel: recognition_model, detectionModel: DetectionModel.Detection02);
            Console.WriteLine($"{detectedFaces.Count} face detected from the link");

            //get face id and bounding box
            Console.WriteLine("Face ID : " + detectedFaces[0].FaceId.Value + "\n"
                + "Face Rectangle : { \n" +"Top : "+detectedFaces[0].FaceRectangle.Top.ToString()+"\n"
                + "Left : " + detectedFaces[0].FaceRectangle.Left.ToString() + "\n"
                + "Width : " + detectedFaces[0].FaceRectangle.Width.ToString() + "\n"
                + "Height : " + detectedFaces[0].FaceRectangle.Height.ToString() + " }");
        }
        //detection model two



        // we use detection mode 2 for the next option because it provide us with face id

        //find similar
        static int i=0;
        public static async Task FindSimilar(IFaceClient client, string recognition_model)
        {
            Console.WriteLine("======== FIND SIMILAR ========");
            Console.WriteLine();

            //list that will contain the url of target images
            List<string> targetImageUrls = new List<string> { };

            Console.WriteLine("enter no of faces you want to find similar in.");
            i = int.Parse(Console.ReadLine());
            for(int j=0;j<i;j++)
            {
                Console.WriteLine($"enter url of image {j + 1}");
                targetImageUrls.Add(Console.ReadLine());
            }

            //string that will contain the url of source image
            string sourceImageUrl;

            Console.WriteLine("enter url for source Image");
            sourceImageUrl = Console.ReadLine();

            //list that will contain target faces id
            List<Guid?> targetFaceIds = new List<Guid?>();

            int l = 1;
            foreach(var targetImageUrl in targetImageUrls)
            {
                Console.WriteLine($"\n======== detection for image {l++} ========");

                //calling detection model two method for detect the target faces
                await DetectFaceRecognize(client,targetImageUrl, recognition_model);
                var faces = detectedFaces.ToList();

                //store the detection detail of target faces
                targetFaceIds.Add(faces[0].FaceId.Value);
            }

            Console.WriteLine($"\n======== detection for source image ========");

            //calling detection model two method for detect the source face
            await DetectFaceRecognize(client, sourceImageUrl, recognition_model);

            //the list where any detected faces stored
            List<DetectedFace> detectedFace = detectedFaces.ToList();

            //find similar faces and store it in a list
            List <SimilarFace> similarResults = (List<SimilarFace>)await client.Face.FindSimilarAsync(detectedFace[0].FaceId.Value, null, null, targetFaceIds);

            //print the similar faces and the confidence on respectively that you enter the target width
            foreach (var similarResult in similarResults)
            {
                Console.WriteLine($"Faces from source & ID:{similarResult.FaceId} are similar with confidence: {similarResult.Confidence}.");
            }
        }
        //find similar


        //verify faces
        public static async Task Verify(IFaceClient client , string recognition_model)
        {
            Console.WriteLine("======== VERIFY ========");
            Console.WriteLine();

            //list that will contain the url of target images
            List<string> targetImageUrls= new List<string> { };

            Console.WriteLine("enter no of faces you want to verify with.");
            i = int.Parse(Console.ReadLine());
            for (int j = 0; j < i; j++)
            {
                Console.WriteLine($"enter url of image {j + 1}");
                targetImageUrls.Add(Console.ReadLine());
            }

            //string that will contain the url of source image
            string sourceImageUrl;

            Console.WriteLine("enter url for source Image");
            sourceImageUrl = Console.ReadLine();

            //list that will contain target faces id
            List<Guid> targetFaceIds = new List<Guid>();

            int l=1;
            foreach (var targetImageUrl in targetImageUrls)
            {
                Console.WriteLine($"\n======== detection for image {l++} ========");

                //calling detection model two method for detect the target faces
                await DetectFaceRecognize(client, targetImageUrl, recognition_model);
                var faces = detectedFaces.ToList();

                //store the detection detail of target faces
                targetFaceIds.Add(faces[0].FaceId.Value);
            }

            Console.WriteLine($"\n======== detection for source image ========");

            //calling detection model two method for detect the source face
            await DetectFaceRecognize(client, sourceImageUrl, recognition_model);

            //the list where any detected faces stored
            List<DetectedFace> detectedFace = detectedFaces.ToList();

            //loop on detectec faces
            for (int r = 0; r < targetFaceIds.Count; r++) 
            
            { 
                //verify the target faced with source face
                VerifyResult verifyResult = await client.Face.VerifyFaceToFaceAsync(detectedFace[0].FaceId.Value, targetFaceIds[r]);

                //print the verified faces and the confidence on respectively that you enter the target width

                //print the result
                Console.WriteLine(
                   verifyResult.IsIdentical
                       ? $"Faces from source & ID:{targetFaceIds[r]} are of the same (Positive) person, similarity confidence: {verifyResult.Confidence}."
                       : $"Faces from source & ID:{targetFaceIds[r]} are of different (Negative) persons, similarity confidence: {verifyResult.Confidence}.");
            }
        }
        //verify faces
    }
}
