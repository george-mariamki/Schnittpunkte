using Microsoft.AspNetCore.Rewrite;
using System.Drawing;

namespace Schnittpunkte.Models
{
    public class RequestForm
    {
        
        public Linie Linie1 { get; set; }

        public Punkt? Punkt2 { get; set; }
        public Linie? Linie2 { get; set; }
        public Kreis? Kreis2 { get; set; }

    }

    public class Linie
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        public bool IsValid()
        {
            if  (this.A == 0 && this.B == 0)
            { 
                return false; 
            }    
            return true;
        }
    }

    public class Punkt
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class Kreis
    {
        public double H { get; set; }
        public double K { get; set; }
        public double R { get; set; }

        public bool IsValid()
        {
            if (this.R <= 0 )
            {
                return false;
            }
            return true;
        }
    }
    public class Result
    {
        public string? status { get; set; }
        public string? message { get; set; }
        public string? result { get; set; }
        public Punkt? punkt { get; set; }
        public Punkt? punkt2 { get; set; }
        public RequestForm? request { get; set; }
    }
}
