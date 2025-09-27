using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace BarnesHut
{
    public class Program
    {
        // Entry point from where the simulation starts and where parameters can be set.
        static void Main()
        {
            string rogue = @"{
                            'solutionID': 'Rogue',
                            'rX': 1.98254,
                            'rY': 0.38105,
                            'rZ': -0.44909,
                            'velX': 3.610665,
                            'velY': 6.92736,
                            'velZ': -8.22501,
                            'mass': 0.001
                            }";

            JSONObject roguePlanet = JsonConvert.DeserializeObject<JSONObject>(rogue);
            string json = ConvertCsvFileToJsonObject(@"C:\Users\Ericb\AppData\Roaming\JetBrains\PyCharmCE2021.3\scratches\masses.csv");

            List<JSONObject> masses = JsonConvert.DeserializeObject<List<JSONObject>>(json);

            // Add groups of particles to initialise here.
            List<BodyGroupProps3D> bodyPropList = new List<BodyGroupProps3D>
            {
                new BodyGroupProps3D() { numBodies = masses.Count, initialisationMethod = "Custom", centerOfMass = new Vec3D(0f, 0f, 0f), centerVelocity = new Vec3D(0f, 0f, 0f) }
            };

            roguePlanet.mass *= 2.0902665e-10f;
            masses.Add(roguePlanet);

            RunSim(masses, bodyPropList, "BarnesHut", "Beeman", 0.0005f, 100);
        }

        static void RunSim(List<JSONObject> objs, List<BodyGroupProps3D> bodyProps, string forceMethod, string integrationMethod, float deltaT, int numFrames)
        {
            Frame firstFrame = new Frame(10000);

            foreach (BodyGroupProps3D props in bodyProps)
            {
                firstFrame.AddBodies(objs, props.numBodies, props.initialisationMethod, props.centerOfMass, props.centerVelocity);
            }

            Simulation simulation = new Simulation(firstFrame);
            simulation.WriteToFile();

            int frameCounter = 0;

            if (integrationMethod == "Beeman")
            {
                simulation.NextStep(deltaT, forceMethod, "Custom");
                simulation.WriteToFile();
                frameCounter++;

                simulation.NextStep(deltaT, forceMethod, "Custom");
                simulation.WriteToFile();
                frameCounter++;
                Console.WriteLine(frameCounter);
            }

            for (int i = frameCounter; i < numFrames; i++)
            {
                frameCounter++;
                simulation.NextStep(deltaT, forceMethod, integrationMethod);
                simulation.WriteToFile();
            }

            simulation.writer.Close();
        }

        public static string ConvertCsvFileToJsonObject(string path)
        {
            var csv = new List<string[]>();
            var lines = File.ReadAllLines(path);

            for (int i = 0; i < lines.Length; i++) // PLACEHOLDER
            {
                csv.Add(lines[i].Split(','));
            }

            var properties = new string[8] { "solutionID", "rX", "rY", "rZ", "velX", "velY", "velZ", "mass" };

            var listObjResult = new List<Dictionary<string, dynamic>>();

            for (int i = 0; i < lines.Length; i++)
            {
                var objResult = new Dictionary<string, dynamic>();
                for (int j = 0; j < properties.Length; j++)
                {
                    if (j > 0)
                    {
                        objResult.Add(properties[j], float.Parse(csv[i][j]));
                    }
                    else {
                        objResult.Add(properties[j], csv[i][j].ToString());
                    }
                }

                listObjResult.Add(objResult);
            }

            return JsonConvert.SerializeObject(listObjResult);
        }
    }

    public struct BodyGroupProps3D
    {
        public int numBodies;
        public string initialisationMethod;
        public Vec3D centerOfMass;
        public Vec3D centerVelocity;
    }

    public class JSONObject {
        public string solutionID { get; set; }
        public float rX { get; set; }
        public float rY { get; set; }
        public float rZ { get; set; }
        public float velX { get; set; }
        public float velY { get; set; }
        public float velZ { get; set; }
        public float mass { get; set; }
    }
}