# **Technical Notes**

* Coded in C# .NET Core 2.0 Web API
* Run and build solution in Visual Studio

# **How to Run**

1. Clone repository to your working station
2. Open solution file in Visual Studio
3. Navigate to "Build" menu located at the top
4. Click "Build All" to build entire solution
5. Navigate to "Run" menu located at the top
6. Click "Start Debugging" or "Start Without Debugging"
7. Wait till new browser window opens running the solution

# **How to Use API**

Once the ParkingAPI is built and running. API resource will run on localhost:33337/api/rates.

The two query parameters needed to query a parking rate are:

* startDateTime
* endDateTime

Set the params to date time ISO format and enter into browser. For example:
```
localhost:33337/api/rates?startDateTime=2015-07-01T07:00:00Z&endDateTime=2015-07-01T13:00:00Z
```

# **View Swagger Documentation Page**

Make sure that the solution is running then navigate to "localhost:33337/swagger" in browser.

------------

# **Sample Problem**

------------

#### Build a stack with an iOS app as frontend and .NET Web APIs as backend, that allows a user to enter a date time range, 
and get back the rate at which they would be charged to park for that time span.

## Requirements

* Publish a repo on GitHub that we can clone and run on our local environment.
* Use C#, .NET Standard/Core for backend Web APIs, and Swift for iOS app to complete this.
* Backend API will need documentation.
* Backend API should support JSON & XML over HTTP.
* We should be able to curl against an API that computes a price for a specified datetime range, given a JSON file of rates.

Sample file: 
```json
    {
      "rates": [
        {
          "days": "mon,tues,wed,thurs,fri",
          "times": "0600-1800",
          "price": 1500
        },
        {
          "days": "sat,sun",
          "times": "0600-2000",
          "price": 2000
        }
      ]
    }
```
     
Sample result:

Datetime ranges should be specified in isoformat.  A rate must completely encapsulate a datetime range for it to be available.

Rates will never overlap.

2015-07-01T07:00:00Z to 2015-07-01T12:00:00Z should yield 1500
2015-07-04T07:00:00Z to 2015-07-04T12:00:00Z should yield 2000
2015-07-04T07:00:00Z to 2015-07-04T20:00:00Z should yield unavailable
 
Sample JSON for testing:

```json
{
    "rates": [
        {
            "days": "mon,tues,thurs",
            "times": "0900-2100",
            "price": 1500
        },
        {
            "days": "fri,sat,sun",
            "times": "0900-2100",
            "price": 2000
        },
        {
            "days": "wed",
            "times": "0600-1800",
            "price": 1750
        },
        {
            "days": "mon,wed,sat",
            "times": "0100-0500",
            "price": 1000
        },
        {
            "days": "sun,tues",
            "times": "0100-0700",
            "price": 925
        }
    ]
}
```