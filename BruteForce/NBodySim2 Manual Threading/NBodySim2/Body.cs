using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NBodySim2
{
    public class Body
    {
        private static readonly double GConst = 6.673e-11;
        private static readonly double Solarmass = 1.98892e30;

        public double posX, posY;           //position
        public double velX, velY;           //velocity
        public double forcX, forcY;         //force
        public double mass;

        public Body(double _posX, double _posY, double _velX, double _velY, double _mass)
        {
            posX = _posX;
            posY = _posY;
            velX = _velX;
            velY = _velY;
            mass = _mass;
        }

        public void Update(double delT)
        {
            //update velocity and position based on the change in time as well as force and mass
            velX += delT * forcX / mass;
            velY += delT * forcY / mass;
            posX += delT * velX;
            posY += delT * velY;
        }

        public double calcDistance(Body body)
        {
            //calculate distance between this and another body
            double distanceX = posX - body.posX;
            double distanceY = posY - body.posY;
            return Math.Sqrt(posX * posX + posY * posY);
        }

        public void resetForce()
        {
            forcX = 0.0;
            forcY = 0.0;
        }

        public void addForce(Body bodyB)
        {
            //Calculations to update the force of the current body based on another body
            Body bodyA = this;
            double epsilon = 3E4; //Softening parameter yo
            double distanceX = bodyB.posX - bodyA.posX;
            double distanceY = bodyB.posY - bodyA.posY;
            double dist = Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
            double F = (GConst * bodyA.mass * bodyB.mass) / (dist * dist + epsilon * epsilon);
            bodyA.forcX += F * distanceX / dist;
            bodyA.forcY += F * distanceY / dist;
        }
    }
}
