# gaio

## Description

Collection of 3 Programs, that together allows for registering prompts to be asked to registered AI's, with the AI's responses being saved to a database, for later analysis.

There are also a handful of library projects, that holds code resused between the projects.

The programs are as follows.

### Statistics.Commandline

A console application, which when run will directly extract from the database any registered AI's to prompt, and all prompts, which are then send to all the registered AI's

This program is meant to be run using a CRON job, to periodically prompt the AI's.
The responses can then in turn later be used to generate statistics about how the AI's responds to specific prompts.

### Statistics.Api

A RESTful Api that can be used to Read, Create, Update and Delete any entity created via either of the other programs.

Due to the nature of it being an Api, it will also allow other programs in the future to make use data generated via either of the other programs.

### Statistics.Uno

A Uno Platform frontend application, targeting WebAssembly.
This program is a frontend program developed with a C# Markup, which is capable of running in the webbrowser.

As Uno Platform is incapable of directly interacting with the chosen Postgres database, it therefore makes use of the Api for its CRUD actions.

It is via this frontend, that prompts to be send to AI's are registered.
It is also from this frontend that AI's to prompt are registered.

Gaio does not automatically support all AI's with this setup.
Adding support for other AI's should however be simple for any developer to do.
To add support it should only be a matter of implementing a service that knows how to communicate with the desired AI, and then registering the AI as supported by updating the List (Enumeration) of supported AI's, which is used to select the AI to prompt, when registering AI's.

From this frontend, a manual execution of all prompts against all or a selected few AI's are also possible.

## For Developers

### How To: Add Support for new AI

To add support for prompting a new AI, a few simple additions have to be made to the code.

1. Add new Service under ´Statistics.Shared.Services.ArtificialIntelligence´ that implements the ´Statistics.Shared.Abstraction.Interfaces.Services.IArtificialIntelligencePromptService´ interface.
    1. The 'Statistics.Shared.Services.ArtificialIntelligence.BasePromptService' base class has some helpful methods for implementing the interface.
2. Expand 'Statistics.Shared.Abstraction.Enum.ArtificialIntelligenceType' with a new fittingly named enum member.
3. Add a new entry to the dictionary inside the constructor of ´Statistics.Shared.Services.ArtificialIntelligence.MasterArtificialIntelligencePromptService´.
    1. The key should be the newly added enum member of the 'Statistics.Shared.Abstraction.Enum.ArtificialIntelligenceType' enum.
    2. The value should be a new instance of the the newly created service implementing the 'Statistics.Shared.Abstraction.Interfaces.Services.IArtificialIntelligencePromptService' interface.
4. Verify via the frontend, that a new Artificial Intelligence entity can be created with the new enum meber selected as the desired type.
