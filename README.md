# gaio

Collection of 3 Programs, that together allows for registering prompts to be asked to registered AI's, with the AI's responses being saved to a database, for later analysis.

There are also a handful of library projects, that holds code resused between the projects.

The programs are as follows.

## Statistics.Commandline

A console application, which when run will directly extract from the database any registered AI's to prompt, and all prompts, which are then send to all the registered AI's

This program is meant to be run using a CRON job, to periodically prompt the AI's.
The responses can then in turn later be used to generate statistics about how the AI's responds to specific prompts.

## Statistics.Api

A RESTful Api that can be used to Read, Create, Update and Delete any entity created via either of the other programs.

Due to the nature of it being an Api, it will also allow other programs in the future to make use data generated via either of the other programs.

## Statistics.Uno

A Uno Platform frontend application, targeting WebAssembly.
This program is a frontend program developed with a C# Markup, which is capable of running in the webbrowser.

As Uno Platform is incapable of directly interacting with the chosen Postgres database, it therefore makes use of the Api for its CRUD actions.

It is via this frontend, that prompts to be send to AI's are registered.
It is also from this frontend that AI's to prompt are registered.

Gaio does not automatically support all AI's with this setup.
Adding support for other AI's should however be simple for any developer to do.
To add support it should only be a matter of implementing a service that knows how to communicate with the desired AI, and then registering the AI as supported by updating the List (Enumeration) of supported AI's, which is used to select the AI to prompt, when registering AI's.

From this frontend, a manual execution of all prompts against all or a selected few AI's are also possible.
