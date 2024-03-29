using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakhaNata_Magic
{
    public class SeekerManager
    {

        //Fields:
        private List<PhysicsAgent> seekers;
        private static SeekerManager instance;

        //Properties:
        /// <summary>
        /// Setting the Singleton Instance property
        /// </summary>
        public static SeekerManager Instance
        {
            get
            {
                //if there is no instance of the class
                if (instance == null)
                {
                    //make an instance
                    instance = new SeekerManager();
                }

                //return the instance
                return instance;
            }
        }

        /// <summary>
        /// get access to the seekers List
        /// </summary>
        public List<PhysicsAgent> Seekers { get{ return seekers; } }

        //Constructor:
        /// <summary>
        /// Default constructor for the SeekerManager class
        /// </summary>
        private SeekerManager()
        {
            seekers = new List<PhysicsAgent>();
            
            for (int i = 0; i < 1000; i++) 
            { 
                seekers.Add(new Seeker());
            }
        }

        //Methods:
        /// <summary>
        /// Adds a seeker into the list
        /// </summary>
        /// <param name="seeker">Seeker being added</param>
        public void Add(Seeker seeker)
        {
            seekers.Add(seeker);
        }

        /// <summary>
        /// Clears the list of seekers
        /// </summary>
        public void Clear()
        {
            seekers.Clear();
        }

        public void Draw()
        {
            foreach (PhysicsAgent agent in seekers)
            {
                if (agent is Seeker)
                {
                    Seeker seeker = (Seeker)agent;

                    seeker.Draw();
                }
            }
        }

        public void Update()
        {
            foreach (PhysicsAgent agent in seekers)
            {
                if (agent is Seeker)
                {
                    Seeker seeker = (Seeker)agent;

                    seeker.Update();
                }
            }
        }

    }
}
