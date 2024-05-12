# Geometric Intersection API
==========================

This is a RESTful API designed to calculate intersections between geometric objects such as lines, circles, and points. It provides endpoints to receive input data for geometric objects and computes the intersection points accordingly.

## Features
--------

- Receive geometric objects data via API endpoints.
- Compute intersection points between:
    - Line and Line
    - Line and Circle
    - Line and Ellipse
    - Line and Point
- Well-documented API using Swagger.
- Follows RESTful principles for a structured and easy-to-use interface.

## Installation and Setup
----------------------

### Prerequisites

- .NET Core SDK installed
- Git installed (optional for cloning the repository)

### Setup Instructions

1. Clone the repository:
    ```bash
    git clone https://github.com/george-mariamki/Schnittpunkte.git
    ```

2. Navigate to the project directory:
    ```bash
    cd Schnittpunkte\Schnittpunkte
    ```
    - You can navigate through the project files using Schnittpunkte.sln. Additionally, you can debug and execute the project through Visual Studio.
  
3. Build the project:
    ```bash
    dotnet build
    ```

4. Run the project:
    ```bash
    dotnet run
    ```

5. Access the Swagger documentation:
    Open a web browser and go to `https://localhost:7114/swagger` to view the API documentation.


## API Endpoints
-------------

- **POST /api/intersection**: Calculate the intersection of geometric objects based on the provided request.
    - Request Body: (the valid request contains Only 2 Objects Linie1 and one of Linie2, Punkt2, Kreis2 or Ellipse2)
        ```json
        {
            "Linie1": {
                "A": 0,
                "B": 0,
                "C": 0
            },
            "Linie2": {
                "A": 0,
                "B": 0,
                "C": 0
            },
            "Punkt2": {
                "X": 0,
                "Y": 0
            },
            "Kreis2": {
                "H": 0,
                "K": 0,
                "R": 0
            },
            "Ellipse2": {
                "H": 0,
                "K": 0,
                "R": 0,
                "A": 0,
                "B": 0
            }
        }
        ```
    - Example Request Body:
        ```json
        {
            "Linie1": {
                "A": 1,
                "B": 1,
                "C": 1
            },
            "Linie2": {
                "A": 2,
                "B": 1,
                "C": 2
            }
        }
        ```

    - Exapmle Response: (it contains the status, result and the intersection point(s) )
        ```json
        {
            "status": "valid",
            "result": "Die Linien schneiden sich.",
            "punkt": {
                "x": -1,
                "y": 0
              },
        }
        ```


    - InvalidResponse: (it contains the status and message )
        ```json
        {
            "status": "Invalid",
            "message": "Invalid Request"
        }
        ```  
