# NEMS Solace .NET

This is a consumer application designed to help publishers and subscribers connect to the NEMS broker.

This application has been written in .NET 9.

The project has been structured in a way where consumers directly modify the Application project.

#### NOTE:

The subscriber portion of the program has been desigend to acknowledge all messages. If a subscriber wants to drop a message, don't thow an exception, supress the processing of the message is acknowledged successfully. If the message fails due to a system fault, if that fault hadn't occured it would have processed successfully, then an exception should be raised. The exception will cause the acknowledgement to fail and the message will remain on the queue for future processing.

### Installation Instructions (Docker)

#### Requirements:

- Docker

#### Installation:

1. Clone this repo into a folder of your choice.
2. Open the `source/properties.json` file. Replace the `PLACEHOLDER` variables to meet your connection details.
3. Run `docker-compose up --build` in the root folder.

The application should install itself and start up. Once started it will connect to the Solace instance and start listening on the specified queue. Once a message is sent to the queue the consumer will pick it up and should display the message contents to the terminal.

### Installation Instructions (Local)

#### Requirements:

- .NET Version 9.0
#### Installation:

1.  Clone this repo into a folder of your choice.
2.  Open the `Source/Publisher/Application/appsettings.json` file. Replace the `PLACEHOLDER` variables to meet your connection details.
3.  To run the application, make sure you are in the `Source/Publisher` directory and run the command

    `dotnet run`