using Microsoft.AspNetCore.Rewrite;
using Schnittpunkte.Models;
using System.Drawing;
using System.Security.Cryptography.Xml;
namespace Schnittpunkte.Services
{
    public class SchnittpunkteService
    {
        public SchnittpunkteService() { }
        /// <summary>
        /// Validates the request by ensuring it contains exactly two geometric objects: one line (Linie1) and one of the following objects: Linie2, Punkt2, or Kreis2.
        /// </summary>
        /// <param name="request">The request containing geometric objects.</param>
        /// <returns>
        /// If the request is valid, returns a Result object with status "valid".
        /// If the request is invalid (e.g., does not contain exactly two geometric objects or Linie1 is missing), returns a Result object with status "Invalid" and a message indicating the reason for invalidity.
        /// </returns>
        public Result? RequestValidation(RequestForm request) 
        {
            List<object> objs = new List<object>();
            if(request.Linie1 != null) objs.Add(request.Linie1);
            if(request.Linie2 != null) objs.Add(request.Linie2);
            if(request.Punkt2 != null) objs.Add(request.Punkt2);
            if(request.Kreis2 != null) objs.Add(request.Kreis2);
            if (objs.Count != 2 || request.Linie1 == null)
            {
                var result = new Result
                {
                    // Status Invalid and the Message
                    status = "Invalid",
                    message = "Die Request sollte nur 2 geometrische Objekte enthalten: das erste sollte 'Linie1' sein, und das zweite muss eines der folgenden Objekte sein: 'Linie2', 'Punkt2' oder 'Kreis2'."
                };
                return result;
            }
            var result1 = new Result
            {
                // valid then the request can be handeled (in another functions
                status = "valid"
            };
            return result1;
        }
        /// <summary>
        /// Calculates the intersection of geometric objects based on the request.
        /// </summary>
        /// <param name="request">The request containing geometric objects.</param>
        /// <returns>
        /// If the request is valid and the geometric objects are properly defined, returns a Result object indicating the intersection.
        /// If the request is invalid or the geometric objects are not properly defined, returns a Result object indicating the reason for the failure.
        /// </returns>
        public Result CalculateIntersection(RequestForm request)
        {
            if (this.RequestValidation(request).status == "valid")
            {
                if (request.Linie1.IsValid() == false)
                {
                    var result = new Result
                    {
                        status = "Invalid",
                        message = "'Linie1' ist ungültig."
                    };
                    return result;
                }

                else 
                {
                    if (request.Punkt2 != null && request.Linie2 == null && request.Kreis2 == null)
                    {


                        bool intersects = this.CalculateIntersectionWithPoint(request.Linie1, request.Punkt2);
                        var result = new Result
                        {
                            status = "valid",
                            result = intersects ? "Der Punkt liegt auf der Linie." : "Der Punkt liegt nicht auf der Linie.",
                            punkt = intersects ? (Punkt)request.Punkt2: null
                        };

                        return result;

                    }
                    else if (request.Punkt2 == null && request.Linie2 != null && request.Kreis2 == null)
                    {
                        if (request.Linie2.IsValid() == false)
                        {
                            var result = new Result
                            {
                                status = "Invalid",
                                message = "'Linie2' ist ungültig."
                            };
                            return result;
                        }

                        else
                        {

                            object intersects = this.CalculateIntersectionLineWithLine(request.Linie1, request.Linie2);
                            if (intersects == null)
                            {
                                var result = new Result
                                {
                                    status = "valid",
                                    result = "Die Linien sind parallel und nicht deckungsgleich, daher gibt es keinen Schnittpunkt."
                                };
                                return result;
                            }
                            else if (intersects.GetType() == typeof(Linie))
                            {
                                var result = new Result
                                {
                                    status = "valid",
                                    result = "Linien sind kongruent."
                                };
                                return result;
                            }
                            else if (intersects.GetType() == typeof(Punkt))
                            {
                                var result = new Result
                                {
                                    status = "valid",
                                    result = "Die Linien schneiden sich.",
                                    punkt = (Punkt)intersects
                                };

                                return result;
                            }
                        }
                    }
                    else if (request.Punkt2 == null && request.Linie2 == null && request.Kreis2 != null)
                    {
                        if (request.Kreis2.IsValid() == false)
                        {
                            var result = new Result
                            {
                                status = "Invalid",
                                message = "Kreis2 ist ungültig; r muss positiv sein."
                            };
                            return result;
                        }
                        else
                        {
                            Punkt[] intersects = this.CalculateIntersectionLineWithCircle(request.Linie1, request.Kreis2);
                            if (intersects == null)
                            {
                                var result = new Result
                                {
                                    status = "valid",
                                    result = "Es gibt keine Schnittpunkt."
                                };
                                return result;
                            }
                            else
                            {
                                
                                var result = new Result
                                {
                                    status = "valid",
                                    result = (intersects.Length>1 && intersects[1].X == intersects[0].X && intersects[0].Y == intersects[1].Y) ? "Die Linie und der Kreis berühren sich." : "Die Linie und der Kreis schneiden sich.",
                                    punkt = intersects[0],
                                    punkt2 = (intersects.Length > 1 && intersects[1].X == intersects[0].X && intersects[0].Y == intersects[1].Y) ? null : intersects[1]
                                };
                                return result;
                                

                            }
                        }
                        
                    }
                    else
                    {
                        var result = new Result
                        {
                            status = "Invalid",
                            message = "Invalid request"
                        };

                        return result;
                    }
                }
                
            }
            else
            {
                return this.RequestValidation(request);
                
            }
            return null;
        }

        /// <summary>
        /// Determines if a line intersects with a point.
        /// </summary>
        /// <param name="linie">The line represented as Ax + By + C = 0.</param>
        /// <param name="punkt">The point with coordinates (x, y).</param>
        /// <returns>
        /// Returns true if the line intersects with the point (i.e., the point lies on the line), otherwise returns false.
        /// </returns>
        public bool CalculateIntersectionWithPoint(Linie linie, Punkt punkt)
        {
            double value = linie.A * punkt.X + linie.B * punkt.Y + linie.C;
            return Math.Abs(value) < double.Epsilon;
        }
        /// <summary>
        /// Calculates the intersection point or line between two lines.
        /// </summary>
        /// <param name="linie1">The first line represented as Ax + By + C = 0.</param>
        /// <param name="linie2">The second line represented as Ax + By + C = 0.</param>
        /// <returns>
        /// If the lines are parallel and non-congruent, returns null (no intersection).
        /// If the lines are congruent, returns one of the lines.
        /// If the lines intersect at a single point, returns the intersection point.
        /// </returns>
        public object? CalculateIntersectionLineWithLine(Linie linie1, Linie linie2)
        {
            // determinant
            double determinant = linie1.A * linie2.B - linie2.A * linie1.B;

            // determinant is zero => the lines are parallel
            if (Math.Abs(determinant) < double.Epsilon)
            {
                //  congruent ?
                bool isCongruent = Math.Abs(linie1.A * linie2.C - linie2.A * linie1.C) < double.Epsilon &&
                                   Math.Abs(linie1.B * linie2.C - linie2.B * linie1.C) < double.Epsilon;

                if (isCongruent)
                {
                    // Return one of the lines as the intersection result
                    return linie1;
                }
                else
                {
                    // Lines are parallel and non-congruent, so there's no intersection
                    return null;
                }
            }

            // intersection point
            double x = -(linie2.B * linie1.C - linie1.B * linie2.C) / determinant;
            double y = -(linie1.A * linie2.C - linie2.A * linie1.C) / determinant;

            return new Punkt { X = x, Y = y };
        }
        /// <summary>
        /// Solves a quadratic equation of the form Ax^2 + Bx + C = 0 and returns its roots.
        /// </summary>
        /// <param name="A">The coefficient of x^2.</param>
        /// <param name="B">The coefficient of x.</param>
        /// <param name="C">The constant term.</param>
        /// <returns>
        /// A list containing the roots of the quadratic equation.
        /// If the discriminant is negative, returns null (no real roots).
        /// If the discriminant is zero, returns a list with one root.
        /// If the discriminant is positive, returns a list with two roots.
        /// </returns>
        public List<double>? QuadraticEquation(double A, double B, double C)
        {
            double discriminant = B * B - 4 * A * C;

            //  discriminant is negative => No intersection
            if (discriminant < 0)
            {
                return null;
            }
            else if (discriminant == 0) 
            {
                return new List<double> { (-B) / (2 * A) };

            }
            return new List<double> { (-B + Math.Sqrt(discriminant)) / (2 * A), (-B - Math.Sqrt(discriminant)) / (2 * A) };
            
            
        }
        /// <summary>
        /// Calculates the intersection points between a line and a circle.
        /// </summary>
        /// <param name="linie">The line represented as Ax + By + C = 0.</param>
        /// <param name="kreis">The circle represented by its center coordinates (H, K) and radius R. as (x - H)^2 + (y - K)^2 = R^2</param>
        /// <returns>
        /// An array containing the intersection points between the line and the circle.
        /// If there are no intersection points (i.e., the discriminant is negative), returns null.
        /// If there is one intersection point (i.e., the discriminant is zero), returns an array with one point.
        /// If there are two intersection points (i.e., the discriminant is positive), returns an array with two points.
        /// </returns>
        public Punkt[]? CalculateIntersectionLineWithCircle(Linie linie, Kreis kreis)
        {
            // null means .... No intersection......

            double a = linie.A;
            double b = linie.B;
            double c = linie.C;
            double h = kreis.H;
            double k = kreis.K;
            double r = kreis.R;
           

            // coefficients for the quadratic equation
            double A = (a * a + b * b);
            double B = -2 * h * (b * b) + 2 * a * c + 2 * a * k * b;
            double C = (h * h) * (b * b) + (c * c) + (k * k) * (b * b) + 2 * k * c * b - (r * r) * (b * b);

            // discriminant
            List<double> XX = this.QuadraticEquation(A, B, C);

            // null means that discriminant is negative => No intersection
            if (XX == null)
            {
                return null; 
            }
            if (b!=0) // Calculate the point/points
            {
                if (XX.Count > 1)
                { // Two Points
                    double y1 = (-a * XX[0] - c) / b;
                    double y2 = (-a * XX[1] - c) / b;
                    return new Punkt[]
                            {
                                new Punkt { X = XX[0], Y = y1 },
                                new Punkt { X = XX[1], Y = y2 }
                            };
                }
                else if (XX.Count == 1)
                { // One Point
                    double y1 = (-a * XX[0] - c) / b;
                    return new Punkt[]
                            {
                                new Punkt { X = XX[0], Y = y1 }
                            };
                }
                else return null;

            }
            else 
            {
                //this case b=0 to avoid dividing by 0
                double AA = 1;
                double BB = -2 * h ;
                double CC = (h * h) + (XX[0] * XX[0]) - 2 * k * XX[0] - (r * r) + (k * k);
                List<double> YY = this.QuadraticEquation(AA, BB, CC);
                
                double discriminant1 = BB * BB - 4 * AA * CC;
                if (YY != null)
                {
                    if (YY.Count > 1)
                    {
                        //Two Points
                        double y1 = (-BB + Math.Sqrt(discriminant1)) / (2 * AA);
                        double y2 = (-BB - Math.Sqrt(discriminant1)) / (2 * AA);
                        return new Punkt[]
                            {
                                new Punkt { X = XX[0], Y = y1 },
                                new Punkt { X = XX[1], Y = y2 }
                            };
                    }
                    else if (YY.Count == 1)
                    {
                        // One Point
                        double y1 = (-BB + Math.Sqrt(discriminant1)) / (2 * AA);
                        return new Punkt[]
                            {
                                new Punkt { X = XX[0], Y = y1 }
                            };
                    }
                    else return null;


                }
                else return null;

            }

            

        }
    
    }
}