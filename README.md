# scAIentific: AI-powered Research Front Analyzer

## Overview

scAIentific is an application designed to assist researchers and students with their research. By utilizing an advanced AI model, scAIentific simplifies the process of identifying key trends, themes, and findings across a vast array of scientific articles. This tool aims to make the exploration of the research frontier more efficient and accessible, thereby saving time and resources for its users.

### Project Origin

The need for scAIentific arose from the challenges faced by researchers in summarizing extensive previous works, a task particularly daunting when writing dissertations or scientific papers. An example project examining gender differences in networking during college years highlighted the labor-intensive process of manually identifying relevant research trends from a pool of 250 scientific articles.

### Purpose

scAIentific's goal is to explore how AI can facilitate and streamline the process of research front analysis, potentially uncovering insights that manual methods might miss.

### Project Goals

- Develop a prototype/proof-of-concept application capable of compiling up to 250 articles to describe the current research front.
- Provide a user-friendly frontend for uploading articles and receiving summarized results.

## Features

- **Article Upload**: Users can upload files up to 512 MB directly into the AI assistant.
- **Custom AI-Assistant Creation**: Allows users to create a personalized AI assistant. This specialized assistant is proficient in a research field specified by the user.
- **Research Discussion with the AI-Assistant**: Offers the capability to converse with the assistant and discuss the file's content, assisting in the identification of the research front.

## Technical Stack

- **Frontend**: Blazor
- **Backend**: C#, .NET

## Getting Started

### Prerequisites

- .NET core 7 an above.
- An IDE such as Visual Studio or Visual Studio Code
- Access to the provided AI model or equivalent

### Setup

1. Clone the repository to your local machine.
2. Ensure all required software and dependencies are installed.
3. Navigate to the project directory and restore the required packages:

```shell
dotnet restore
```

4. Start the backend server:

```shell
dotnet run --project Path/To/BackendProject
```

5. Start the frontend application:

```shell
cd Path/To/FrontendProject
dotnet run
```

## Usage

- Navigate to the frontend application through your web browser.
- Follow the UI prompts to upload your scientific texts and receive the analysis.  

## Project design
![assistant-page](AppOverview/assistant-page.png)  
![uploadFile-page](AppOverview/uploadFile-page.png) 

## License


