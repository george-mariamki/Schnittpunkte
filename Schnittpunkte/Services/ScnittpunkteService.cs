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
            if (request.Linie1 != null) objs.Add(request.Linie1);
            if (request.Linie2 != null) objs.Add(request.Linie2);
            if (request.Punkt2 != null) objs.Add(request.Punkt2);
            if (request.Kreis2 != null) objs.Add(request.Kreis2);
            if (request.Ellipse2 != null) objs.Add(request.Ellipse2);
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
                if (!request.Linie1.IsValid())
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
                    // isvalid means that the request contains Linie1 and only one another geometric object
                    if (request.Punkt2 != null)
                    {


                        bool intersects = this.CalculateIntersectionWithPoint(request.Linie1, request.Punkt2);
                        var result = new Result
                        {
                            status = "valid",
                            result = intersects ? "Der Punkt liegt auf der Linie." : "Der Punkt liegt nicht auf der Linie.",
                            punkt = intersects ? (Punkt)request.Punkt2 : null
                        };

                        return result;

                    }
                    else if (request.Linie2 != null)
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
                    /*else if (request.Kreis2 != null)
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
                                if (intersects.Length > 1)
                                {
                                    var result = new Result
                                    {
                                        status = "valid",
                                        result = (intersects[1].X == intersects[0].X && intersects[0].Y == intersects[1].Y) ? "Die Linie und der Kreis berühren sich." : "Die Linie und der Kreis schneiden sich.",
                                        punkt = intersects[0],
                                        punkt2 = (intersects[1].X == intersects[0].X && intersects[0].Y == intersects[1].Y) ? null : intersects[1]
                                    };
                                    return result;
                                }
                                else if (intersects.Length == 1)
                                {
                                    var result = new Result
                                    {
                                        status = "valid",
                                        result = "Die Linie und der Kreis berühren sich.",
                                        punkt = intersects[0]
                                    };
                                    return result;
                                }
                                else
                                {
                                    return null;
                                }
                            }
                        }

                    }*/
                    else if (request.Ellipse2 != null || request.Kreis2!=null)
                    {
                        if (request.Ellipse2 != null && request.Ellipse2.IsValid==null)
                        {
                            
                            var result = new Result
                            {
                                status = "Invalid",
                                message = "Ellipse2 ist ungültig; a und b muessen positiv sein."
                            };
                            return result;
                            
                        }
                        if (request.Kreis2 != null && request.Kreis2.IsValid() == false)
                        {
                            var result = new Result
                            {
                                status = "Invalid",
                                message = "Ellipse2 ist ungültig; a und b muessen positiv sein."
                            };
                            return result;
                            
                        }
                        
                        else
                        {
                            Punkt[] intersects = this.CalculateIntersectionLineWithEllipseOrCircle(request.Linie1,ellipse: request.Ellipse2, kreis: request.Kreis2);
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
                                if (intersects.Length > 1)
                                {
                                    var result = new Result
                                    {
                                        status = "valid",
                                        result = (intersects[1].X == intersects[0].X && intersects[0].Y == intersects[1].Y) ? "Sie berühren sich." : "Sie schneiden sich.",
                                        punkt = intersects[0],
                                        punkt2 = (intersects[1].X == intersects[0].X && intersects[0].Y == intersects[1].Y) ? null : intersects[1]
                                    };
                                    return result;
                                }
                                else if (intersects.Length == 1)
                                {
                                    var result = new Result
                                    {
                                        status = "valid",
                                        result = "Sie berühren sich.",
                                        punkt = intersects[0]
                                    };
                                    return result;
                                }
                                else
                                {
                                    return null;
                                }
                                


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

            if (b == 0)
            {
                double A = 1;
                double B = -2 * h;
                double C = (h * h) + (-c/a * -c/a) - 2 * k * -c/a - (r * r) + (k * k);
                
                //double xx = -1 * c / a;
                List<double> YY = this.QuadraticEquation(A, B, C);
                if (YY == null)
                {
                    return null;
                }
                if (YY.Count > 1)
                {
                    return new Punkt[]
                        {
                            new Punkt { X = -1*c/a, Y = YY[0] },
                            new Punkt { X = -1*c/a, Y = YY[1] } // here XX[1] == XX[0] ;)
                        };
                }
                else if (YY.Count == 1)
                {
                    return new Punkt[]
                        {
                            new Punkt { X = -1*c/a, Y = YY[0] }
                        };
                }
                else
                {
                    return null;
                }
            }
            else
            {
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
            



        }

        
        /// <summary>
        /// Calculates the intersection points between a line and an ellipse or circle.
        /// </summary>
        /// <param name="linie">The line.</param>
        /// <param name="ellipse">The ellipse (optional). If not provided, a circle must be provided.</param>
        /// <param name="kreis">The circle (optional). If not provided, an ellipse must be provided.</param>
        /// <returns>
        /// An array of points representing the intersection points between the line and the ellipse,
        /// or null if there are no intersection points.
        /// </returns>
        /// <remarks>
        /// This method calculates the intersection points between a line and an ellipse in a 2D plane.
        /// The line is represented by its coefficients (A, B, C) in the standard form of a linear equation: Ax + By + C = 0.
        /// The ellipse is represented by its center coordinates (H, K) and the lengths of its semi-major and semi-minor axes (A, B).
        /// </remarks>
        public Punkt[]? CalculateIntersectionLineWithEllipseOrCircle(Linie linie, Ellipse ellipse=null, Kreis kreis=null)
        {
            if (ellipse==null && kreis==null)
            { return null; }    
            // null means .... No intersection......

            double a = linie.A;
            double b = linie.B;
            double c = linie.C;
            double h = ellipse!=null? ellipse.H : kreis.H;
            double k = ellipse != null ? ellipse.K : kreis.K;
            double a_ellipse = ellipse != null ? ellipse.A : 1;
            double b_ellipse = ellipse != null ? ellipse.B : 1;
            double r = ellipse != null ? 1: kreis.R;
            Console.WriteLine("A  " + a + "  B  " + b + "  C  " + c + "  h  " + h);
            Console.WriteLine("k  " + k + "  aa  " + a_ellipse + "  b  " + b_ellipse + "  r  " + r);
            if (b == 0)
            {
                /*double A = 1;
                double B = -2 * h;
                double C = (h * h) + (-c/a * -c/a) - 2 * k * -c/a - (r * r) + (k * k);*/

                //double xx = -1 * c / a;
                double A = a_ellipse * a_ellipse;
                double B = -2 * k * (a_ellipse * a_ellipse) ;
                double C = (k * k) * (a_ellipse * a_ellipse) -1 * (r * r) * (b_ellipse * b_ellipse) * (a_ellipse * a_ellipse) + (b_ellipse * b_ellipse) * (-1*c/a - h) * (-1 * c / a - h);
                /*double A = a_ellipse * a_ellipse;
                double B = -2 * k * (a_ellipse * a_ellipse);
                double C = (k * k) * (a_ellipse * a_ellipse) - 1 * (r * r) * (b_ellipse * b_ellipse) * (a_ellipse * a_ellipse) +
                    (b_ellipse * b_ellipse) * (-1 * c / a - h) * (-1 * c / a - h);
*/
                List<double> YY = this.QuadraticEquation(A, B, C);
                if (YY == null)
                {
                    return null;
                }
                if (YY.Count>1)
                {
                    return new Punkt[]
                        {
                            new Punkt { X = -1*c/a, Y = YY[0] },
                            new Punkt { X = -1*c/a, Y = YY[1] } // here XX[1] == XX[0] ;)
                        };
                }
                else if (YY.Count == 1)
                {
                    return new Punkt[]
                        {
                            new Punkt { X = -1*c/a, Y = YY[0] }
                        };
                }
                else
                {
                    return null;
                }
            }
            else
            {
                // coefficients for the quadratic equation
                double A = (a * a * a_ellipse * a_ellipse 
                    + b * b * b_ellipse * b_ellipse);
                double B = -2 * h * (b * b) * (b_ellipse * b_ellipse) 
                    + 2 * a * c * (a_ellipse * a_ellipse) //* b 
                    + 2 * a * k * b * (a_ellipse * a_ellipse);
                double C = h * (b * b) * (b_ellipse * b_ellipse) + (c * c) * (a_ellipse * a_ellipse) 
                    + (a_ellipse * a_ellipse) * (k * k) * (b * b) 
                    + 2 * k * c * b * (a_ellipse * a_ellipse) 
                    - (a_ellipse * a_ellipse) * (b_ellipse * b_ellipse) * (b * b) * (r * r);
                
                // discriminant
                List<double> XX = this.QuadraticEquation(A, B, C);
                Console.WriteLine("A  " + A + "  B  " + B + "  C  " + C + "  XX  " + XX);
                // null means that discriminant is negative => No intersection
                if (XX == null )
                {
                    return null;
                }
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
        }
    }
}