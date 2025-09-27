using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace BarnesHut
{
    // Class to contain a single simulation.
    public class Simulation
    {
        public StreamWriter writer = new StreamWriter("animation.csv");

        public Frame lastFrame;
        readonly Vec3D[] lastForces;

        public Frame prevLastFrame;
        readonly Vec3D[] prevLastForces;

        // Constructor.
        public Simulation(Frame firstFrame)
        {
            this.lastFrame = new Frame(firstFrame);
            lastForces = new Vec3D[firstFrame.bodyList.Count];
            prevLastForces = new Vec3D[firstFrame.bodyList.Count];
        }

        // Write the last frame to file.
        public void WriteToFile()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");
            lastFrame.WriteToFile(writer);
        }

        // Calulate the positions and velocities for all the particles given the current frame following a set integration method.
        public void NextStep(float deltaT, string forceMethod, string integrationMethod)
        {
            Vec3D[] newForces = new Vec3D[lastFrame.bodyList.Count()];

            Frame newFrame = new Frame(lastFrame);
            Frame oldFrame = new Frame(lastFrame);

            switch (forceMethod)
            {
                case ("Brute"):
                    {
                        if (integrationMethod != "Beeman")
                        {
                            newForces = lastFrame.ForcesBrute();

                            for (int i = 0; i < oldFrame.bodyList.Count; i++)
                            {
                                prevLastForces[i] = lastForces[i];
                                lastForces[i] = newForces[i];
                            }
                        }

                        break;
                    }

                case ("BarnesHut"):
                    {
                        lastFrame.BuildTree();
                        newForces = lastFrame.ForcesBHTree();


                        for (int i = 0; i < oldFrame.bodyList.Count; i++)
                        {
                            prevLastForces[i] = lastForces[i];
                            lastForces[i] = newForces[i];
                        }

                        break;
                    }
            }

            switch (integrationMethod)
            {
                case ("Euler"):
                    {
                        for (int i = 0; i < oldFrame.bodyList.Count; i++)
                        {
                            newFrame.bodyList[i].position += deltaT * oldFrame.bodyList[i].velocity;
                            newFrame.bodyList[i].velocity += (deltaT / oldFrame.bodyList[i].mass) * newForces[i];
                        }

                        lastFrame = new Frame(newFrame);
                        prevLastFrame = new Frame(oldFrame);

                        break;
                    }

                case ("Beeman"):
                    {
                        for (int i = 0; i < oldFrame.bodyList.Count; i++)
                        {
                            newFrame.bodyList[i].position += (deltaT * oldFrame.bodyList[i].velocity + (deltaT * deltaT / oldFrame.bodyList[i].mass) * ((2f / 3f) * lastForces[i] - (1f / 6f) * prevLastForces[i]));
                        }

                        newFrame.BuildTree();
                        newForces = newFrame.ForcesBHTree();

                        for (int i = 0; i < oldFrame.bodyList.Count; i++)
                        {
                            prevLastForces[i] = lastForces[i];
                            lastForces[i] = newForces[i];
                        }

                        for (int i = 0; i < oldFrame.bodyList.Count; i++)
                            newFrame.bodyList[i].velocity += (deltaT / oldFrame.bodyList[i].mass) * ((5f / 12f) * newForces[i] + (2f / 3f) * lastForces[i] - (1f / 12f) * prevLastForces[i]);

                        lastFrame = new Frame(newFrame);
                        prevLastFrame = new Frame(oldFrame);

                        break;
                    }
            }
        }
    }

    // Class to contain a frame of the simulation.
    public class Frame
    {
        public List<Particle> bodyList;
        readonly Random randomGenerator;
        TreeNode rootNode;

        readonly float boundingBoxWidth;

        // Constructor.
        public Frame(float boundingboxwidth)
        {
            bodyList = new List<Particle>();
            randomGenerator = new Random();
            this.boundingBoxWidth = boundingboxwidth;
        }

        // Constructor.
        public Frame(Frame frame)
        {
            randomGenerator = new Random();

            this.bodyList = new List<Particle>();
            this.boundingBoxWidth = frame.boundingBoxWidth;

            for (int i = 0; i < frame.bodyList.Count(); i++)
            {
                bodyList.Add(new Particle(frame.bodyList[i].position, frame.bodyList[i].velocity, frame.bodyList[i].mass, frame.bodyList[i].colour));
            }
        }

        // Add a group of bodies to the current head frame according to some initialisation parameters.
        public void AddBodies(List<JSONObject> objs, int nBodies, string method, Vec3D CenterOfMass = null, Vec3D CenterVelocity = null)
        {
            switch (method)
            {
                case ("Kepler"):
                    {
                        float blackHoleMass = (float)Math.Pow(10, 10);

                        bodyList.Add(new Particle(CenterOfMass, CenterVelocity, blackHoleMass, "white"));

                        for (int i = 0; i < nBodies; i++)
                        {
                            float radius = (float)(randomGenerator.NextDouble() * 35) + 10f;
                            float phi = (float)(randomGenerator.NextDouble() * 2.0 * Math.PI);

                            Vec3D posvec = new Vec3D((float)(radius * Math.Cos(phi)), (float)(radius * Math.Sin(phi)), 0f);

                            float velocity = (float)Math.Sqrt(blackHoleMass / radius);
                            Vec3D velvec = new Vec3D((float)(velocity * Math.Sin(phi)), (float)(-1f * velocity * Math.Cos(phi)), 0f);

                            posvec += CenterOfMass;
                            velvec += CenterVelocity;

                            bodyList.Add(new Particle(posvec, velvec, 0.005f, "white"));
                        }

                        break;
                    }
                case ("Custom"):
                    {
                        float blackHoleMass = 8.54f;
                        bodyList.Add(new Particle(new Vec3D(-5, 0, 0), new Vec3D(0, 0, 0), blackHoleMass, "white"));

                        int c = 0;

                        foreach (JSONObject obj in objs) {
                            Vec3D posvec = new Vec3D(obj.rX, obj.rY, obj.rZ);
                            Vec3D velvec = new Vec3D(obj.velX, obj.velY, obj.velZ);

                            posvec += CenterOfMass;
                            velvec += CenterVelocity;

                            if (obj.solutionID.Equals("Rogue"))
                            {
                                Console.WriteLine("Blue");
                                bodyList.Add(new Particle(posvec, velvec, obj.mass, "blue"));
                            }
                            else {
                                bodyList.Add(new Particle(posvec, velvec, obj.mass, "white"));
                            }
                            c++;
                        }

                        break;
                    }
            }
        }

        // Build the BH tree recursively.
        public void BuildTree()
        {
            rootNode = new TreeNode(new Oct(new Vec3D(-boundingBoxWidth, -boundingBoxWidth, -boundingBoxWidth), 2f * boundingBoxWidth), null);
            rootNode.InitializeChildren();

            for (int i = 0; i < bodyList.Count(); i++)
                AddParticle(bodyList[i], rootNode);
        }

        // Add a particle to a TreeNode.
        public static void AddParticle(Particle particle, TreeNode node)
        {
            if (!node.oct.Contains(particle))
                return;

            if (node.children.Count > 0)
            {
                for (int i = 0; i < 8; i++)
                    if (node.children[i].oct.Contains(particle))
                        AddParticle(particle, node.children[i]);
            }
            else
            {
                if (node.body.mass > 0)
                {
                    node.InitializeChildren();

                    for (int i = 0; i < 8; i++)
                    {
                        if (node.children[i].oct.Contains(particle))
                            AddParticle(particle, node.children[i]);

                        if (node.children[i].oct.Contains(node.body))
                            AddParticle(node.body, node.children[i]);
                    }
                }
            }

            node.body.position *= node.body.mass;
            node.body.position += particle.position * particle.mass;

            node.body.mass += particle.mass;
            node.body.position *= (1f / node.body.mass);
        }

        // Calculate the forces on all particles in the simulation using brute force.
        public Vec3D[] ForcesBrute()
        {
            Vec3D[] forces = new Vec3D[this.bodyList.Count()];

            for (int i = 0; i < forces.Length; i++)
                forces[i] = new Vec3D(0f, 0f, 0f);

            for (int i = 0; i < this.bodyList.Count(); i++)
                for (int j = 0; j < this.bodyList.Count(); j++)
                {
                    if (i == j) { continue; }

                    Vec3D force = ForcePair(this.bodyList[i], this.bodyList[j]);

                    forces[i] += force;
                }

            return forces;
        }

        // Calculate the forces on all particles in the simulation using the BH tree.
        public Vec3D[] ForcesBHTree()
        {
            Vec3D[] forces = new Vec3D[this.bodyList.Count()];

            for (int i = 0; i < this.bodyList.Count(); i++)
                forces[i] = ForceTreeNode(this.rootNode, this.bodyList[i]);

            return forces;
        }

        // Calculate the force between a treenode and a particle.
        public Vec3D ForceTreeNode(TreeNode node, Particle part)
        {
            float theta = node.oct.edgeLength / (float)Math.Sqrt((node.body.position - part.position).MagnitudeSquard());

            if ((node.body.position - part.position).MagnitudeSquard() == 0)
                return new Vec3D(0, 0, 0);

            if (theta < 2.5f) { return ForcePair(part, node.body); }

            if (node.children.Count() == 0)
                return ForcePair(part, node.body);

            Vec3D force = new Vec3D(0, 0, 0);

            for (int i = 0; i < 8; i++)
                force += ForceTreeNode(node.children[i], part);

            return force;
        }

        // Calculate the force between 2 particles.
        public static Vec3D ForcePair(Particle a, Particle b)
        {
            Vec3D r = 3.08567758e16f*(a.position - b.position);

            float eps = 1f;

            if (Math.Sqrt(r.MagnitudeSquard()) > 1f)
                return -6.6743f * ((a.mass * b.mass) / (float)Math.Pow(r.MagnitudeSquard() + eps * eps, 2 / 2)) * (r * (float)(1.0f / Math.Pow(r.MagnitudeSquard(), 1.0 / 2.0)));

            return new Vec3D(0, 0, 0);
        }

        // Writes all the frames in this simulation to an injected streamwriter.
        public void WriteToFile(StreamWriter writer)
        {
            List<double> xList = new List<double>();
            List<double> yList = new List<double>();
            List<double> zList = new List<double>();
            List<string> colour = new List<string>();

            foreach (Particle body in this.bodyList)
            {
                xList.Add(body.position.xCoord);
                yList.Add(body.position.yCoord);
                zList.Add(body.position.zCoord);
                colour.Add(body.colour);
            }

            writer.WriteLine(String.Join(",", xList.Select(i => i.ToString()).ToArray()));
            writer.WriteLine(String.Join(",", yList.Select(i => i.ToString()).ToArray()));
            writer.WriteLine(String.Join(",", zList.Select(i => i.ToString()).ToArray()));
            writer.WriteLine(String.Join(",", colour.Select(i => i.ToString()).ToArray()));
        }
    }

}
